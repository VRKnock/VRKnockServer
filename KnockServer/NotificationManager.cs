using System;
using SteamVR_HUDCenter;

namespace KnockServer
{
    public class NotificationManager
    {
        private static NotificationManager instance;

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
            HUDCenterController.GetInstance().Init();
            
            
        }
        
        public void ShowNotification(string message)
        {
            var controller = HUDCenterController.GetInstance();
            controller.ClearNotifications();
            TestOverlay testOverlay = new TestOverlay("Knock Knock", 100f);
            controller.RegisterNewItem(testOverlay);
            testOverlay.showMessage(message);
        }
        
        


    }
}