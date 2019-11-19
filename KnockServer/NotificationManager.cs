using System;
using System.IO;
using System.Linq;
using SteamVR_HUDCenter;

namespace KnockServer
{
    public class NotificationManager
    {
        private static NotificationManager instance;

        private TestOverlay overlayInstance;

        private string connectionCode;

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
            controller.Init();

            overlayInstance = new TestOverlay("Knock Knock!", 100f);
            controller.RegisterNewItem(overlayInstance);
        }

        public bool CheckCode(string code)
        {
            return this.connectionCode == code;
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



    }
}