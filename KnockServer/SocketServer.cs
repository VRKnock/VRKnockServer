using System;
using System.Reflection;
using System.Web.Script.Serialization;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace KnockServer
{
    public class SocketServer : WebSocketBehavior
    {
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            Console.WriteLine("A Client Disconnected.");
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            Console.WriteLine("New Client Connected!");
        }
        

        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            Request request = ParseRequest(e.Data);

            Status status = null;
            switch (request.action)
            {
                case "knock":
                   status =  HandleKnock(request);
                    break;
                case "status":
                default:
                   status =  HandleStatus(request);
                    break;
                
            }
            
            SendStatus(status);
        }

        Status HandleStatus(Request request)
        {
            Console.WriteLine("HandleStatus");
            
            
            Status status = new Status();
            status.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var notificationManager = NotificationManager.GetInstance();
            if (!notificationManager.CheckCode(request.code))
            {
                Console.WriteLine("Wrong Code!");
                status.status = 1;
                status.msg = "Wrong code";

                return status;
            }
            
            status.host = Environment.MachineName;

            bool running = true;
            if (running)
            {
                status.status = 0;
                status.msg = "Server and VR Controller Running!";
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

        Status HandleKnock(Request request)
        {
            Console.WriteLine("HandleKnock");
            
            Console.WriteLine(request.code);
            Console.WriteLine(request.message);

            Status status = new Status();
            status.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var notificationManager = NotificationManager.GetInstance();
            if (!notificationManager.CheckCode(request.code))
            {
                Console.WriteLine("Wrong Code!");
                status.status = 1;
                status.msg = "Wrong code";

                return status;
            }
            
            Console.WriteLine("Correct Code!");

            status.host = Environment.MachineName;


            try
            {
                notificationManager.ShowNotification(request.message);
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




            return status;
        }

        Request ParseRequest(string data)
        {
            return new JavaScriptSerializer().Deserialize<Request>(data);
        }

        void SendStatus(Status status)
        {
            string json = new JavaScriptSerializer().Serialize(status);
            Send(json);
        }
        
    }

    public class Request
    {
        public string action { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string version { get; set; }
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