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
            
            var error = OpenVR.Applications.SetApplicationAutoLaunch("org.inventivetalent.vrknock", Properties.Settings.Default.AutoStart);
            Console.WriteLine("ApplicationAutoLaunch Error:");
            Console.WriteLine(error);
            if (error == EVRApplicationError.UnknownApplication)
            {
                error = OpenVR.Applications.AddApplicationManifest(
                    Path.Combine(System.Environment.CurrentDirectory, "manifest.vrmanifest"), false);
                Console.WriteLine("AddApplicationManifest Error:");
                Console.WriteLine(error);
            }

            error = OpenVR.Applications.SetApplicationAutoLaunch("org.inventivetalent.vrknock", Properties.Settings.Default.AutoStart);
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
                icfMgr = (INetFwMgr)Activator.CreateInstance(TicfMgr);
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
                portClass = (INetFwOpenPort)Activator.CreateInstance(TportClass);

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
        WebSocketServer server;

        public CustomApplicationContext()
        {
            trayIcon = new NotifyIcon()
            {
                Text = "VRKnockServer",
                Icon = Properties.Resources.AppIcon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Info", Info),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
            trayIcon.Click += Info;


            Console.WriteLine("Adding Firewall Rule...");
            Program.AddFirewallRule();


            try
            {
                Console.WriteLine("Starting Web Service...");

                var port = 16945;
                server = new WebSocketServer(port);
                server.AddWebSocketService<SocketServer>("/");
                server.Start();
                
                Console.WriteLine("Web Service Running!");
                Console.WriteLine(server.Address+":"+port);
                
                // Console.WriteLine(localIp);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                trayIcon.ShowBalloonTip(2000, "VRKnock", "Failed to start Server!", ToolTipIcon.Error);
                return;
            }

            try
            {
                Console.WriteLine("Initializing VR...");
                NotificationManager.GetInstance().Init();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                trayIcon.ShowBalloonTip(2000, "VRKnock", "Failed to init VR! Please make sure SteamVR is running!", ToolTipIcon.Error);
                return;
            }
        
            Program.RegisterAutoLaunchApp();
                 

            trayIcon.ShowBalloonTip(2000, "VRKnock", "Server Running!", ToolTipIcon.Info);

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

            server.Stop();
        }

        void Exit(object sender, EventArgs e)
        {


            Application.Exit();
        }
    }

}