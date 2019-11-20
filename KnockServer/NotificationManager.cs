using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using QRCoder;
using SteamVR_HUDCenter;
using Valve.VR;

namespace KnockServer
{
    public class NotificationManager
    {
        private static NotificationManager instance;

        private TestOverlay overlayInstance;

        public string connectionCode;
        private QRCode qrCode;

        public static NotificationManager GetInstance()
        {
            if (instance == null)
                instance = new NotificationManager();
            return instance;
        }

        private NotificationManager()
        {
        }

        [STAThread]
        public void Init()
        {
            this.connectionCode = Properties.Settings.Default.ConnectionCode;
            if (this.connectionCode == null || this.connectionCode.Length == 0)
            {
                this.connectionCode = RandomString(16);
                Properties.Settings.Default.ConnectionCode = this.connectionCode;
                Properties.Settings.Default.Save();
            }
            Console.WriteLine("Connection Code: " + this.connectionCode);


            var controller = HUDCenterController.GetInstance();
            controller.Init(EVRApplicationType.VRApplication_Background);

            overlayInstance = new TestOverlay("Knock Knock!", 100f);
            controller.RegisterNewItem(overlayInstance);
        }

        public bool CheckCode(string code)
        {
            return this.connectionCode == code;
        }

        public QRCode GetQRCode()
        {
            if (qrCode == null)
            {
                qrCode = new QRCode(new QRCodeGenerator().CreateQrCode("http://"+GetLocalIPAddress()+"/"+connectionCode,QRCodeGenerator.ECCLevel.Q));
            }

            return qrCode;
        }

        public void ShowNotification(string message)
        {
            overlayInstance.showMessage(message);
        }


        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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