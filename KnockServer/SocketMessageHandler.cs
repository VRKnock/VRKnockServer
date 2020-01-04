using System;
using System.Reflection;
using System.Web.Script.Serialization;
using WebSocketSharp;

namespace KnockServer
{
    public class SocketMessageHandler
    {
        public int clientCount = 0;
        
        public void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            Request request = ParseRequest(e.Data);

            if (request._state != null)
            {
                if (request._state == "CONNECT")
                {
                    clientCount++;
                }

                if (request._state == "DISCONNECT")
                {
                    clientCount--;
                }

                Console.WriteLine("ClientCount: "+clientCount);
                return;
            }

            Status status = null;
            switch (request.action)
            {
                case "triggerKnock":
                    status = HandleKnock(request);
                    break;
                case "status":
                    status = HandleStatus(request);
                    break;
            }

            if (status != null)
                SendStatus(status,request._source);
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

        void SendStatus(Status status, string target)
        {
            string json = new JavaScriptSerializer().Serialize(status);
            Send(json,target);
        }

        public Func<string,string, string> Send { get; set; }

        protected virtual string SendImpl(string text, string target)
        {
            throw new NotImplementedException();
        }
    }

    public class Request
    {
        public string _state { get; set; }
        public string action { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string version { get; set; }
        public string _source { get; set; }
    }

    public class Status
    {
        public string evt = "status";
        public int status { get; set; }
        public string msg { get; set; }
        public string host { get; set; }
        public string device { get; set; }
        public string game { get; set; }
        public string version { get; set; }
    }
}