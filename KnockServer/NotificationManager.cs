using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using QRCoder;
using Valve.VR;

namespace KnockServer
{
    public class NotificationManager
    {
        private static NotificationManager instance;


        public string connectionCode;
        private QRCode qrCode;

        private ulong notificationHandle;
        private uint currentNotificationId;

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


            //var controller = HUDCenterController.GetInstance();
            //controller.Init(EVRApplicationType.VRApplication_Overlay);
            
            EVRInitError error = EVRInitError.None;
            OpenVR.Init(ref error, EVRApplicationType.VRApplication_Overlay);

            if (error != EVRInitError.None)
                throw new Exception("An error occured while initializing OpenVR!");

            OpenVR.GetGenericInterface(OpenVR.IVRCompositor_Version, ref error);
            if (error != EVRInitError.None)
                throw new Exception("An error occured while initializing Compositor!");

            OpenVR.GetGenericInterface(OpenVR.IVROverlay_Version, ref error);
            if (error != EVRInitError.None)
                throw new Exception("An error occured while initializing Overlay!");
            
            InitOverlay();
        }

        void InitOverlay()
        {
            var overlayError =  OpenVR.Overlay.CreateOverlay("VRKnock_Overlay", "VRKnock", ref notificationHandle);
            if (overlayError != EVROverlayError.None)
                throw new Exception(overlayError.ToString());

            OpenVR.Overlay.SetOverlayWidthInMeters(notificationHandle, 100);
            OpenVR.Overlay.SetOverlayInputMethod(notificationHandle, VROverlayInputMethod.None);
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
            OpenVR.Notifications.RemoveNotification(currentNotificationId);

       
            
            //overlayInstance.showMessage(message);
            uint ID = GetRandomID();
            var bitmap = new NotificationBitmap_t();
            OpenVR.Notifications.CreateNotification(notificationHandle, 0, EVRNotificationType.Transient, message,
                EVRNotificationStyle.None, ref bitmap, ref ID);
            currentNotificationId = ID;

            OpenVR.Overlay.ShowOverlay(notificationHandle);
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

        private static uint GetRandomID()
        {
            Random rnd = new Random();
            return  (uint)rnd.Next();
        }

    }
}