using System;
using System.Net.Sockets;
using System.Reflection;
using System.Web.Script.Serialization;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace KnockServer
{
    public class SocketServer : WebSocketBehavior
    {
        private SocketMessageHandler _messageHandler;

        public int clientCount = 0;
        
        public SocketServer()
        {
            _messageHandler = new SocketMessageHandler()
            {
                Send = (s,t) =>
                {
                    Send(s);
                    return null;
                }
            };
        }


        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            Console.WriteLine("A Client Disconnected.");
            clientCount--;
            Console.WriteLine("ClientCount: "+clientCount);
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            Console.WriteLine("New Client Connected!");
            clientCount++;
            Console.WriteLine("ClientCount: "+clientCount);
        }


        protected override void OnMessage(MessageEventArgs e)
        {
            _messageHandler.OnMessage(e);
        }
    }
}