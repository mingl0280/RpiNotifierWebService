using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;
using System.Data.SQLite;
using System.Configuration;
using System.Web.Configuration;
using static System.Text.Encoding;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace RpiWebService.Api
{
    /// <summary>
    /// WeatherHandler 的摘要说明
    /// </summary>
    public class WeatherHandler : IHttpHandler
    {
        private static Thread weatherUpdateThread = new Thread(weatherUpdateWorker);
        private static Thread textUpdateThread = new Thread(textUpdateWorker);
        private static Thread MsgRecvThread = new Thread(OnMsgReceived);
        private static SQLiteConnection sqlConn;
        private static CurrentWeather cw;
        private static string cwQueryString;
        private static int intMetricType;
        private static string apikey;

        public void ProcessRequest(HttpContext context)
        {
            string setting = ConfigurationManager.AppSettings["ApiKey"];
            string strFormat = "", strValue = "";
            if (!string.IsNullOrEmpty(setting))
            {
                apikey = setting;
            }
            else
            {
                Console.WriteLine("Invalid or unset api key.");
                return;
            }
            setting = ConfigurationManager.AppSettings["ConnStringFormat"];
            if (!string.IsNullOrEmpty(setting))
            {
                strFormat = setting;
            }
            else
            {
                Console.WriteLine("Invalid connection string format.");
                return;
            }
            setting = ConfigurationManager.AppSettings["SQLiteFile"];
            if (!string.IsNullOrEmpty(setting))
            {
                strValue = setting;
            }
            else
            {
                Console.WriteLine("Invalid connection string filename.");
                return;
            }

            string connstr = string.Format(strFormat, context.Server.MapPath(strValue));
            sqlConn = new SQLiteConnection(connstr);
            try
            {
                sqlConn.Close();
            }
            catch (Exception) { }

            sqlConn.Open();
            if (context.IsWebSocketRequest)
                context.AcceptWebSocketRequest(weatherSocketHandler);
        }

        private static void weatherUpdateWorker(object s)
        {
            WebSocket socket = (WebSocket)s;
            while (true)
            {
                cw = new CurrentWeather();
                cw.fetchWeatherData(cwQueryString, apikey, intMetricType, "zh_cn");
                if (textUpdateThread.ThreadState != System.Threading.ThreadState.Running)
                {
                    try
                    {
                        textUpdateThread = new Thread(textUpdateWorker);
                        textUpdateThread.Start(socket);
                    }
                    catch (Exception)
                    { }
                }
                Thread.Sleep(30 * 60 * 1000);
            }
        }

        private static void textUpdateWorker(object s)
        {
            WebSocket socket = (WebSocket)s;
            int intervalSeconds = 30;
            while (true)
            {
                int counter = 0;
                while (cw.IsReady == false)
                {
                    Thread.Sleep(50);
                    counter++;
                    if (counter >= 10)
                    {
                        intervalSeconds = 1;
                        break;
                    }
                    else
                    {
                        intervalSeconds = 30;
                    }
                }
                sendMsg(socket);
                Thread.Sleep(intervalSeconds * 1000);
            }
        }

        private static void sendMsg(WebSocket socket)
        {
            if (cw.WeatherInfo == null)
            {
                Dictionary<string, string> msgDic = new Dictionary<string, string>();
                msgDic.Add("icon", "");
                msgDic.Add("loc", "Initializing...");
                msgDic.Add("weatherTextUpper", "Initializing...");
                msgDic.Add("weatherTextLower", "Initializing...");
                socket.SendAsync(new ArraySegment<byte>(UTF8.GetBytes(new JavaScriptSerializer().Serialize(msgDic))), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                Dictionary<string, string> msgDic = new Dictionary<string, string>();
                msgDic.Add("icon", cw.WeatherInfo.IconId);
                msgDic.Add("loc", (cw.CityInfo.CityName + " " + cw.CityInfo.CityCountry).Trim());
                msgDic.Add("weatherTextUpper",
                    cw.WeatherInfo.WeatherDescription +
                    " 当前温度: " + cw.AtomosphereInfo.TempNow.ToString() + (
                        cw.MetricType == 2 ? "°F" : ((cw.MetricType == 1) ? "°C" : " K"))
                        );
                msgDic.Add("weatherTextLower",
                    "湿度: " + cw.AtomosphereInfo.Humdity.ToString() + "%  气压: " + cw.AtomosphereInfo.Pressure.ToString() + " " + cw.AtomosphereInfo.PressureUnit);
                msgDic.Add("weatherOthInfo",
                    "风向: " + cw.WindStatus.Direction.ToString() + "° " + cw.WindStatus.DirectionCode + " " + cw.WindStatus.Speed.ToString() + (cw.MetricType == 2 ? "mph" : @" m/s"));
                msgDic.Add("LastUpdate", "最近更新: " + cw.LastUpdate.ToLocalTime().ToLongDateString() + " " + cw.LastUpdate.ToLocalTime().ToLongTimeString());


                socket.SendAsync(new ArraySegment<byte>(UTF8.GetBytes(new JavaScriptSerializer().Serialize(msgDic))), WebSocketMessageType.Text, true, CancellationToken.None);
                //await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fetch Succeed", new CancellationToken(true));
                //textUpdateThread.Abort();
                //weatherUpdateThread.Abort();
            }
        }

        private static void OnMsgReceived(object s)
        {
            MsgRecvStruct msg = (MsgRecvStruct)s;
            ArraySegment<byte> buffer = msg.buf;
            byte[] nbuf = msg.nbuf;
            WebSocket socket = msg.sock;
            var data = buffer.Array.Where(x => x != 0).ToArray();
            string initString = UTF8.GetString(data, 0, data.Length);
            string[] initOptions = initString.Split(';');
            Dictionary<string, string> Options = new Dictionary<string, string>();
            bool isGeo = false;
            string queryKey = "";
            foreach (string option in initOptions)
            {
                string[] KeyValues = option.Split('=');

                switch (KeyValues[0])
                {
                    case "queryType":
                        if (KeyValues[1] == "geo")
                        {
                            Options.Add("lat", "");
                            Options.Add("lon", "");
                            isGeo = true;
                        }
                        else
                        {
                            Options.Add(KeyValues[1], "");
                            queryKey = KeyValues[1];
                        }
                        break;
                    case "query":
                        if (isGeo)
                        {
                            string[] LatLon = KeyValues[1].Split(',');
                            Options["Lat"] = LatLon[0];
                            Options["Lon"] = LatLon[1];
                        }
                        else
                        {
                            int id = 0;
                            bool isint = int.TryParse(KeyValues[1], out id);
                            if (queryKey == "id" && !isint)
                            {
                                if (KeyValues[1].IndexOf(',') > 0)
                                {
                                    string[] CityNation = KeyValues[1].Split(',');
                                    SQLiteCommand query = new SQLiteCommand("SELECT id FROM CityList WHERE name Like @city AND country = @country ORDER BY name ASC LIMIT 0,1", sqlConn);

                                    query.Parameters.AddWithValue("@city", CityNation[0] + '%');
                                    query.Parameters.AddWithValue("@country", CityNation[1]);
                                    try
                                    {
                                        query.VerifyOnly();
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine(e.Message);
                                    }
                                    var queryResult = query.ExecuteScalar();
                                    if (queryResult != null)
                                        if (int.TryParse(queryResult.ToString(), out id))
                                        {
                                            Options[queryKey] = id.ToString();
                                        }
                                }
                                else
                                {
                                    SQLiteCommand query = new SQLiteCommand("SELECT id FROM CityList WHERE name Like @CITY ORDER BY name ASC LIMIT 0,1", sqlConn);
                                    query.Prepare();
                                    query.Parameters.AddWithValue("@CITY", KeyValues[1] + "%");
                                    if (int.TryParse(query.ExecuteScalar().ToString(), out id))
                                    {
                                        Options[queryKey] = id.ToString();
                                    }
                                }
                            }
                            else
                            {
                                Options[queryKey] = KeyValues[1];
                            }
                        }
                        break;
                    case "metricType":
                        int.TryParse(KeyValues[1], out intMetricType);
                        break;
                }
            }
            cwQueryString = @"http://api.openweathermap.org/data/2.5/weather?";
            string tmpQueryString = "";
            foreach (KeyValuePair<string, string> UrlOpt in Options)
            {
                tmpQueryString += @"&" + UrlOpt.Key + @"=" + UrlOpt.Value;
            }
            tmpQueryString = tmpQueryString.TrimStart('&');
            cwQueryString = cwQueryString + tmpQueryString;
            try
            {
                weatherUpdateThread = new Thread(weatherUpdateWorker);
                weatherUpdateThread.Start(socket);
            }
            catch (Exception) { }

        }

        private struct MsgRecvStruct
        {
            public WebSocket sock;
            public ArraySegment<byte> buf;
            public byte[] nbuf;
        }

        private async static Task weatherSocketHandler(AspNetWebSocketContext context)
        {
            var socket = context.WebSocket;
            
            while (true)
            {
                byte[] nbuf = new byte[2048];
                ArraySegment<byte> buffer = new ArraySegment<byte>(nbuf);
                if (socket.State == WebSocketState.Open)
                {
                    //Dictionary<string, string> JsonValueDict = new Dictionary<string, string>();
                    var initResults = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    MsgRecvStruct Msg;
                    Msg.sock = socket;
                    Msg.nbuf = nbuf;
                    Msg.buf = buffer;
                    MsgRecvThread = new Thread(OnMsgReceived);
                    MsgRecvThread.Start(Msg);
                    /*
                    if (initResults.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }*/

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