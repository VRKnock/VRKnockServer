using System;
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
            var controller = HUDCenterController.GetInstance();
            controller.Init();

            overlayInstance = new TestOverlay("Knock Knock!", 100f);
            controller.RegisterNewItem(overlayInstance);
        }
        
        public void ShowNotification(string message)
        {
           overlayInstance.showMessage(message);
        }
        
        


    }
}