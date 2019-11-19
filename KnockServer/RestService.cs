using System;
using SteamVR_HUDCenter;
using Valve.VR;

namespace KnockServer
{
    public class RestService : IService
    {
        
        
        public Status GetStatus()
        {
            Console.WriteLine("GetStatus");
            
            var controller = HUDCenterController.GetInstance();
            bool running = controller._IsRunning;
            
            Status status = new Status();
            if (running)
            {
                status.status = 0;
                status.msg = "Server & VR Controller Running!";
            }
            else
            {
                status.status = 1;
                status.msg = "VR Controller not running";
            }

            status.host = Environment.MachineName;
            
            return status;
        }

        public Status TriggerKnock(string code, string message="Knock Knock!")
        {
            Console.WriteLine("TriggerKnock");
            Console.WriteLine(code);
            Console.WriteLine(message);

            Status status = new Status();

            var notificationManager = NotificationManager.GetInstance();
            if (!notificationManager.CheckCode(code))
            {
                Console.WriteLine("Wrong Code!");
                status.status = 1;
                status.msg = "Wrong code";

                return status;
            }
            Console.WriteLine("Correct Code!");

            notificationManager.ShowNotification(message);

            status.status = 0;
            status.msg = "Notification sent!";
            
            return status;
        }

     
    }
}