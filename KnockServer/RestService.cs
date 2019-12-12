using System;
using System.Diagnostics;
using Valve.VR;

namespace KnockServer
{
    public class RestService : IService
    {
        private Stopwatch _stopwatch = new Stopwatch();

        public RestService()
        {
            _stopwatch.Start();
        }

        public Status GetStatus(string code = "")
        {
            Console.WriteLine("GetStatus");

            // var controller = HUDCenterController.GetInstance();
            bool running = true;

            Status status = new Status();

            var notificationManager = NotificationManager.GetInstance();
            if (!notificationManager.CheckCode(code))
            {
                Console.WriteLine("Wrong Code!");
                status.status = 1;
                status.msg = "Wrong code";

                return status;
            }

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

            if (!notificationManager.IsHmdPresent())
            {
                status.status = 1;
                status.msg = "VR Headset is not connected";
                return status;
            }

            status.host = Environment.MachineName;
            status.game = notificationManager.GetCurrentGame();


            return status;
        }

        public Status TriggerKnock(string code, string message = "Knock Knock!")
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

            /*
            if (_stopwatch.ElapsedMilliseconds < 1000 * 10)
            {
                Console.WriteLine("Too Soon!");
                status.status = 1;
                status.msg = "Too soon";

                return status;
            }
            */


            try
            {
                notificationManager.ShowNotification(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                status.status = 2;
                status.msg = "Unexpected Error";

                return status;
            }

            status.status = 0;
            status.msg = "Notification sent!";

            Console.WriteLine("Notification sent!");

            status.host = Environment.MachineName;
            status.game = notificationManager.GetCurrentGame();

            _stopwatch.Restart();


            return status;
        }
    }
}