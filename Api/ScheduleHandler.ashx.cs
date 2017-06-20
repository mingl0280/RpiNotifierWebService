using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;


namespace RpiWebService.Api
{
    /// <summary>
    /// ScheduleHandler 的摘要说明
    /// </summary>
    public class ScheduleHandler : IHttpHandler
    {
        private Thread schedUpdateThread = new Thread(scheduleUpdateWorker) ;
        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            schedUpdateThread.Start();
            context.AcceptWebSocketRequest(scheduleSocketHandler);
        }

        private static void scheduleUpdateWorker()
        {
            while(true)
            {

                Thread.Sleep(1000);
            }
        }

        private async static Task scheduleSocketHandler(AspNetWebSocketContext context)
        {
            var socket = context.WebSocket;
            while(true)
            {
                byte[] nbuf = new byte[2048];
                ArraySegment<byte> buffer = new ArraySegment<byte>();
                if (socket.State == WebSocketState.Open)
                {
                    Dictionary<string, string> JsonValueDict = new Dictionary<string, string>();

                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}