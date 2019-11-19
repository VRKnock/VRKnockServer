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

        public static NotificationManager GetInstance()
        {
            if(instance==null)
                instance = new NotificationManager();
            return instance;
        }

        private NotificationManager()
        {
        }

        [STAThread]
        public void Init()
        {
            var varsFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "VRKnockSettings.txt");
            
            
            
            var controller = HUDCenterController.GetInstance();
            controller.Init();

            overlayInstance = new TestOverlay("Knock Knock!", 100f);
            controller.RegisterNewItem(overlayInstance);
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