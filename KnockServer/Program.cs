using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Windows.Forms;
using NetFwTypeLib;
using SteamVR_HUDCenter;
using SteamVR_HUDCenter.Elements;
using SteamVR_HUDCenter.Elements.Forms;
using Valve.VR;

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

            Application.Run(new CustomApplicationContext());

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


    }

    public class CustomApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;

        WebServiceHost hostWeb;

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
            AddFirewallRule();


            try
            {
                Console.WriteLine("Starting Web Service...");
                /*
                var restServiceInstance = new RestService();
                WebServiceHost hostWeb = new WebServiceHost(restServiceInstance);
                */
                hostWeb = new WebServiceHost(typeof(RestService));
                ServiceEndpoint ep = hostWeb.AddServiceEndpoint(typeof(KnockServer.IService), new WebHttpBinding(), "");
                ServiceDebugBehavior stp = hostWeb.Description.Behaviors.Find<ServiceDebugBehavior>();
                stp.HttpHelpPageEnabled = false;
                hostWeb.Open();

                //var localIp = GetLocalIPAddress()

                Console.WriteLine("Web Service Running!");
                Console.WriteLine(ep.Address);
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                trayIcon.ShowBalloonTip(2000, "VRKnock", "Failed to init VR!", ToolTipIcon.Error);
                return;
            }


            trayIcon.ShowBalloonTip(2000, "VRKnock", "Server Running!",ToolTipIcon.Info);

        }


        static bool AddFirewallRule()
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

            hostWeb.Close();
            HUDCenterController.GetInstance().Stop();
        }

        void Exit(object sender, EventArgs e)
        {


            Application.Exit();
        }
    }

}