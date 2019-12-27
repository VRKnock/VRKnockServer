using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using Valve.VR;

namespace KnockServer
{
    [ServiceContract]
    public class RestService 
    {
        private Stopwatch _stopwatch = new Stopwatch();

        public RestService()
        {
            _stopwatch.Start();
        }

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "status")]
        [return:MessageParameter(Name="Status")]
        public Status GetStatus(string code = "")
        {
            Console.WriteLine("GetStatus");

            // var controller = HUDCenterController.GetInstance();
            bool running = true;

            Status status = new Status();
            status.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var notificationManager = NotificationManager.GetInstance();
            if (!notificationManager.CheckCode(code))
            {
                Console.WriteLine("Wrong Code!");
                status.status = 1;
                status.msg = "Wrong code";

                return status;
            }
            
            status.host = Environment.MachineName;

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

            if (Properties.Settings.Default.ShowActivity)
            {
                status.game = notificationManager.GetCurrentGame();
            }


            return status;
        }

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "triggerKnock")]
        [return:MessageParameter(Name="Status")]
        public Status TriggerKnock(string code, string message = "Knock Knock!")
        {
            Console.WriteLine("TriggerKnock");
            Console.WriteLine(code);
            Console.WriteLine(message);

            Status status = new Status();
            status.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var notificationManager = NotificationManager.GetInstance();
            if (!notificationManager.CheckCode(code))
            {
                Console.WriteLine("Wrong Code!");
                status.status = 1;
                status.msg = "Wrong code";

                return status;
            }
            
            Console.WriteLine("Correct Code!");

            status.host = Environment.MachineName;
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

            if (Properties.Settings.Default.ShowActivity)
            {
                status.game = notificationManager.GetCurrentGame();
            }


            _stopwatch.Restart();


            return status;
        }
    }
    
    public class Status
    {
        public int status { get; set; }
        public string msg { get; set; }
        public string host { get; set; }
        public string device { get; set; }
        public string game { get; set; }
        public string version { get; set; }
    }
    
}