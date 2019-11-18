using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
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
            AddFirewallRule();
            
            NotificationManager.GetInstance().Init();
            
            /*
            var restServiceInstance = new RestService();
            WebServiceHost hostWeb = new WebServiceHost(restServiceInstance);
            */
            WebServiceHost hostWeb = new WebServiceHost(typeof(RestService));
            ServiceEndpoint ep = hostWeb.AddServiceEndpoint(typeof(KnockServer.IService), new WebHttpBinding(), "");
            ServiceDebugBehavior stp = hostWeb.Description.Behaviors.Find<ServiceDebugBehavior>();
            stp.HttpHelpPageEnabled = false;
            hostWeb.Open();

            var localIp = GetLocalIPAddress();
            
            Console.WriteLine("Web Service Running!");
            Console.WriteLine(ep.Address);
            Console.WriteLine(localIp);

            Console.WriteLine("");
            Console.WriteLine("Press any key to stop");
            

            Console.ReadLine();
            Console.WriteLine("Stopping...");
            hostWeb.Close();
            HUDCenterController.GetInstance().Stop();
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
        
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}