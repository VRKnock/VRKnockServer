using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Windows.Forms;
using NetFwTypeLib;
using Valve.VR;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace KnockServer
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if ("console" == args[0])
                    ShowConsoleWindow();
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*
            if (!IsRunAsAdmin())
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Assembly.GetEntryAssembly().CodeBase;

                proc.Verb = "runas";
                proc.Arguments = string.Join(" ", args);

                try
                {
                    Process.Start(proc);
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("This program must be run as an administrator! \n\n" + ex.ToString());
                }
            }
            else
            {*/
            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version.ToString());

            Application.Run(new CustomApplicationContext());
            //}
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }


        private static bool IsRunAsAdmin()
        {
            try
            {
                WindowsIdentity id = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(id);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void RegisterAutoLaunchApp()
        {
            var error = OpenVR.Applications.SetApplicationAutoLaunch("org.inventivetalent.vrknock",
                Properties.Settings.Default.AutoStart);
            Console.WriteLine("ApplicationAutoLaunch Error:");
            Console.WriteLine(error);
            if (error == EVRApplicationError.UnknownApplication)
            {
                error = OpenVR.Applications.AddApplicationManifest(
                    Path.Combine(System.Environment.CurrentDirectory, "manifest.vrmanifest"), false);
                Console.WriteLine("AddApplicationManifest Error:");
                Console.WriteLine(error);
            }

            error = OpenVR.Applications.SetApplicationAutoLaunch("org.inventivetalent.vrknock",
                Properties.Settings.Default.AutoStart);
            Console.WriteLine("ApplicationAutoLaunch Error:");
            Console.WriteLine(error);
        }

        public static bool AddFirewallRule()
        {
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/a3e390d1-4383-4f23-bad9-b725bef33499/add-firewall-rule-programatically?forum=wcf
            INetFwMgr icfMgr = null;
            try
            {
                Type TicfMgr = Type.GetTypeFromProgID("HNetCfg.FwMgr");
                icfMgr = (INetFwMgr) Activator.CreateInstance(TicfMgr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            try
            {
                INetFwProfile profile;
                INetFwOpenPort portClass;
                Type TportClass = Type.GetTypeFromProgID("HNetCfg.FWOpenPort");
                portClass = (INetFwOpenPort) Activator.CreateInstance(TportClass);

                // Get the current profile
                profile = icfMgr.LocalPolicy.CurrentProfile;

                // Set the port properties
                portClass.Scope = NetFwTypeLib.NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
                portClass.Enabled = true;
                portClass.Protocol = NetFwTypeLib.NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                portClass.Name = "RemoteVRKnockServer";
                portClass.Port = 16945;

                // Add the port to the ICF Permissions List
                profile.GloballyOpenPorts.Add(portClass);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }

    public class CustomApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;

        WebServiceHost hostWeb;
        WebSocketServer socketServer;
        static SocketServer innerSocketServer;
         static WebSocket socketClient;
         static SocketMessageHandler _messageHandler;

        public CustomApplicationContext()
        {
            trayIcon = new NotifyIcon()
            {
                Text = "VRKnockServer",
                Icon = Properties.Resources.AppIcon,
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    new MenuItem("Info", Info),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
            trayIcon.Click += Info;


            if (string.IsNullOrEmpty(Properties.Settings.Default.ServerId))
            {
                Properties.Settings.Default.ServerId = Guid.NewGuid().ToString();
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("ServerId: " + Properties.Settings.Default.ServerId);

            Console.WriteLine("Adding Firewall Rule...");
            Program.AddFirewallRule();


            Console.WriteLine("ConnectionMethod: " + Properties.Settings.Default.ConnectionMethod);
            if (Properties.Settings.Default.ConnectionMethod == "DIRECT")
            {
                try
                {
                    Console.WriteLine("Starting WebSocket Server...");

                    var port = 16945;
                    socketServer = new WebSocketServer(port);
                    socketServer.AddWebSocketService<SocketServer>("/", () =>
                    {
                        return innerSocketServer=  new SocketServer();
                    });
                    socketServer.Start();
                    
                    Console.WriteLine("Web Service Running!");
                    Console.WriteLine(socketServer.Address + ":" + port);

                    // Console.WriteLine(localIp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    trayIcon.ShowBalloonTip(2000, "VRKnock", "Failed to start Server!", ToolTipIcon.Error);
                    return;
                }
            }
            else if (Properties.Settings.Default.ConnectionMethod == "BRIDGE")
            {
                try
                {
                    Console.WriteLine("Starting WebSocket Client...");


                    socketClient = new WebSocket("wss://bridge.vrknock.app")
                    {
                        Origin ="ws://"+Properties.Settings.Default.ServerId+".servers.vrknock.app:16945"
                    };
                    
                    _messageHandler = new SocketMessageHandler()
                    {
                        Send = (s,t) =>
                        {
                            socketClient.Send("{\"_type\":\"forward\",\"source\":\""+Properties.Settings.Default.ServerId+"\",\"target\":\""+t+"\",\"payload\":"+s+'}');
                            return null;
                        }
                    };
                        
                    socketClient.OnOpen += (sender, args) =>
                    {
                        Console.WriteLine("Connected to bridge!");
                        
                        socketClient.Send("{\"_type\":\"register\",\"payload\":{\"type\":\"server\",\"serverId\":\""+Properties.Settings.Default.ServerId+"\"}}");
                    };
                    socketClient.OnMessage += (sender, args) =>
                    {
                        Console.WriteLine(args.Data);
                        _messageHandler.OnMessage(args);
                    };
                    
                    socketClient.ConnectAsync();



                    // Console.WriteLine(localIp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    trayIcon.ShowBalloonTip(2000, "VRKnock", "Failed to connect to bridge!", ToolTipIcon.Error);
                    return;
                }
            }

            try
            {
                Console.WriteLine("Initializing VR...");
                NotificationManager.GetInstance().Init();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                trayIcon.ShowBalloonTip(2000, "VRKnock", "Failed to init VR! Please make sure SteamVR is running!",
                    ToolTipIcon.Error);
                return;
            }

            Program.RegisterAutoLaunchApp();


            trayIcon.ShowBalloonTip(2000, "VRKnock", "Server Running!", ToolTipIcon.Info);
        }

        public static int GetClientCount()
        {
            if (Properties.Settings.Default.ConnectionMethod == "DIRECT" && innerSocketServer != null)
            {
                return innerSocketServer.clientCount;
            }else if (Properties.Settings.Default.ConnectionMethod == "BRIDGE" && _messageHandler != null)
            {
                return _messageHandler.clientCount;
            }

            return 0;
        }

        void Info(object sender, EventArgs e)
        {
            ShowInfoForm();
        }

        public static void ShowInfoForm()
        {
            InfoForm form = new InfoForm();
            form.Show();
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;

            socketServer?.Stop();
            socketClient?.Close();
        }

        void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}