using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

        public string GetDevice()
        {
            //TODO
            return "";
        }

        public bool IsHmdPresent()
        {
            return OpenVR.IsHmdPresent();
        }
        
        public string GetCurrentGame()
        {
            uint processId = OpenVR.Applications.GetCurrentSceneProcessId();
            Console.WriteLine("Current Process ID: " + processId);

            if (processId != 0)
            {
                try
                {
                    StringBuilder stringBuilder = new StringBuilder()
                    {
                        Length = (int) byte.MaxValue
                    };
                    EVRApplicationError error =
                        OpenVR.Applications.GetApplicationKeyByProcessId(processId, stringBuilder, (uint) byte.MaxValue);
                    if (error == EVRApplicationError.None)
                    {
                        string appKey = stringBuilder.ToString();
                        Console.WriteLine("App Key: " + appKey);
                        if (appKey.Length > 0)
                        {
                            OpenVR.Applications.GetApplicationPropertyString(appKey, EVRApplicationProperty.Name_String,
                                stringBuilder, (uint) byte.MaxValue, ref error);
                            if (error != EVRApplicationError.None)
                            {
                                Console.WriteLine("GetApplicationPropertyString Error:");
                                Console.WriteLine(error);
                                return appKey;
                            }
                            string appName = stringBuilder.ToString();

                            Console.WriteLine("App Name: " + appName);
                            return appName;
                        }
                    }
                    else
                    {
                        Console.WriteLine(error);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to get app name via OpenVR");
                    Console.WriteLine(ex);
                }
            }

            var process = Process.GetProcessById((int)processId);
            Console.WriteLine(process.ProcessName);
            Console.WriteLine(process.MainWindowTitle);
            
            if (process.MainWindowTitle.Length > 0)
            {
                return process.MainWindowTitle;
            }
           
            return process.ProcessName;
        }

        public bool CheckCode(string code)
        {
            return this.connectionCode == code;
        }

        public QRCode GetQRCode()
        {
            if (qrCode == null)
            {
                if (Properties.Settings.Default.ConnectionMethod == "DIRECT")
                {
                    qrCode = new QRCode(new QRCodeGenerator().CreateQrCode(
                        "https://vrknock.app/" + GetLocalIPAddress() + "/" + connectionCode+"?cm=direct",
                        QRCodeGenerator.ECCLevel.Q));
                }else if (Properties.Settings.Default.ConnectionMethod == "BRIDGE")
                {
                    qrCode = new QRCode(new QRCodeGenerator().CreateQrCode(
                        "https://vrknock.app/" + Properties.Settings.Default.ServerId + "/" + connectionCode+"?cm=bridge",
                        QRCodeGenerator.ECCLevel.Q));
                }
            }

            return qrCode;
        }

        public void ShowNotification(string message)
        {
            OpenVR.Notifications.RemoveNotification(currentNotificationId);


            
            //overlayInstance.showMessage(message);
            uint ID = GetRandomID();
            
            /// https://github.com/ValveSoftware/openvr/issues/1133#issuecomment-460575906
            // TODO: allow for custom images (e.g. emojis) specified in mobile app
            var image = new NotificationBitmap_t();
            var bitmap = new Bitmap("Resources/vrknock-x512.png");
            
            System.Drawing.Imaging.BitmapData TextureData =
                bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );

            image.m_pImageData = TextureData.Scan0;
            image.m_nWidth = TextureData.Width;
            image.m_nHeight = TextureData.Height;
            image.m_nBytesPerPixel = 4;

            OpenVR.Notifications.CreateNotification(notificationHandle, 0, EVRNotificationType.Transient, message,
                EVRNotificationStyle.None, ref image, ref ID);
            currentNotificationId = ID;
            
            //Unlocks Image Data
            bitmap.UnlockBits(TextureData);

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