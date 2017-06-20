using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;
using System.Xml;
using static System.Text.Encoding;
using Microsoft.Win32;

namespace RpiWebService.Api
{
    /// <summary>
    /// WeatherHandler 的摘要说明
    /// </summary>
    public class WeatherHandler : IHttpHandler
    {
        private static Thread schedUpdateThread = new Thread(weatherUpdateWorker);
        private static Thread textUpdateThread = new Thread(textUpdateWorker);
        private static CurrentWeather cw;
        private string apikey;

        public WeatherHandler()
        {
            System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);
            if (rootWebConfig1.AppSettings.Settings.Count > 0)
            {
                System.Configuration.KeyValueConfigurationElement customSetting =
                    rootWebConfig1.AppSettings.Settings["ApiKey"];
                if (customSetting != null)
                    apikey = customSetting.Value;
                else
                    Console.WriteLine("Invalid or unset api key.");
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            if (context.IsWebSocketRequest)
                context.AcceptWebSocketRequest(weatherSocketHandler);
        }

        private static void weatherUpdateWorker()
        {
            while (true)
            {

                Thread.Sleep(30*60*1000);
            }
        }

        private static void textUpdateWorker()
        {
            while (true)
            {
                Thread.Sleep(30 * 1000);
            }
        }

        private async static Task weatherSocketHandler(AspNetWebSocketContext context)
        {
            var socket = context.WebSocket;
            while (true)
            {
                byte[] nbuf = new byte[2048];
                ArraySegment<byte> buffer = new ArraySegment<byte>(); 
                if (socket.State == WebSocketState.Open)
                {
                    schedUpdateThread.Start();
                    //Dictionary<string, string> JsonValueDict = new Dictionary<string, string>();
                    var initResults = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    
                    var data = buffer.Array.Where(x => x != 0).ToArray();
                    string initString = UTF8.GetString(data, 0, data.Length);
                    
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

    public class CurrentWeather
    {
        public int cityId { get; set; }
        public cityinfo CityInfo { get { return cityInfo; } }
        public metricType MetricType;
        public atominfo AtomosphereInfo { get { return atomInfo; } }
        public sunTime SunTime { get { return sTime; } }
        public wind WindStatus { get { return wStatus; } }

        private cityinfo cityInfo;
        private atominfo atomInfo;
        private sunTime sTime;
        private wind wStatus;
        private long visibility;
        
        public enum metricType
        {
            metric = 1,
            imperial=2,
            kelvin=0
        }

        public class cityinfo
        {
            public string CityName { get { return cityName; } }
            public float CityLon { get { return cityLon; } }
            public float CityLat { get { return cityLat; } }
            public string CityCountry { get { return cityCountry; } }

            private string cityName;
            private float cityLon;
            private float cityLat;
            private string cityCountry;

            public void setCityName(string cName)
            {
                cityName = cName;
            }
            public void setCityCoord(float lon, float lat)
            {
                cityLon = lon;
                cityLat = lat;
            }

            public void setCityCountry(string country)
            {
                cityCountry = country;
            }

            public cityinfo()
            {

            }
            public cityinfo(string name)
            {
                cityName = name;
            }
        }

        public class sunTime
        {
            public String sunRise { get; }
            public String sunSet { get; }

            public sunTime(string Rise, string Set)
            {
                sunRise = DateTime.Parse(Rise).ToLocalTime().ToString();
                sunSet = DateTime.Parse(Set).ToLocalTime().ToString();
            }

            public sunTime()
            {
                return;
            }
        }

        public class wind
        {
            public float Speed { get { return speed; } }
            public string WindName { get { return windName; } }
            public int Direction { get { return direction; } }
            public string DirectionCode { get { return directionCode; } }
            public string DirectionName { get { return directionName; } }

            private float speed ;
            private string windName ;
            private int direction ;
            private string directionCode ;
            private string directionName ;

            public wind()
            {

            }
            public wind(float spd, string wname, int dir, string dcode, string dname)
            {
                speed = spd;
                windName = wname;
                direction = dir;
                directionCode = dcode;
                directionName = dname;
            }

            public void setwindSpeed(float spd, string wname)
            {
                speed = spd;
                windName = wname;
            }

            public void setwindDir(int dir, string dcode, string dname)
            {
                direction = dir;
                directionCode = dcode;
                directionName = dname;
            }
        }

        public class atominfo
        {
            public float TempNow { get { return tempNow; } }
            public float TempMin { get { return tempMin; } }
            public float TempMax { get { return tempMax; } }
            public int Humdity { get { return humdity; } }
            public int Pressure { get { return pressure; } }
            public string PressureUnit { get { return pressureUnit; } }
            private float tempNow;
            private float tempMin;
            private float tempMax;
            private int humdity;
            private int pressure;
            private string pressureUnit;

            public void setTemp(float a, float b, float c)
            {
                tempNow = a;
                tempMin = b;
                tempMax = c;
            }

            public void setHumidity(int a)
            {
                humdity = a;
            }

            public void setPressure(int a, string b)
            {
                pressure = a;
                pressureUnit = b;
            }
            
            public atominfo()
            { }
            public atominfo(float a, float b, float c)
            {
                tempNow = a;
                tempMin = b;
                tempMax = c;
            }
        }
        
        
        public CurrentWeather(int cid)
        {
            cityId = cid;
        }

        public bool processXMLString(string xmlText)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(xmlText);
                if (xDoc.HasChildNodes)
                {
                    ProcNodeValues(xDoc.ChildNodes);
                }
                return true;
            }
            catch (Exception)
            { return false; }
        }

        public bool fetchWeatherData(string OpenWeatherAPIUrl, string ApiKey, metricType mType)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                string queryMetricType = "";
                switch(mType)
                {
                    case metricType.kelvin:
                        break;
                    case metricType.imperial:
                        queryMetricType = "&units=imperial";
                        break;
                    case metricType.metric:
                        queryMetricType = "&units=metric";
                        break;
                    default:
                        queryMetricType = "&units=metric";
                        break;
                }
                xDoc.LoadXml(OpenWeatherAPIUrl + queryMetricType + "&mode=xml&appid=" + ApiKey);
                if (xDoc.HasChildNodes)
                {
                    ProcNodeValues(xDoc.ChildNodes);
                }
                else
                {
                    return false;
                }
                return true;
            }catch(Exception)
            { return false; }
        }

        private void ProcNodeValues(XmlNodeList xNodeList)
        {
            foreach(XmlNode xNode in xNodeList)
            {
                if (xNode.HasChildNodes)
                {
                    ProcNodeValues(xNode.ChildNodes);
                }
                var xNodeAttribs = xNode.Attributes;
                switch(xNode.Name)
                {
                    case "city":
                        cityInfo = new cityinfo(xNodeAttribs["name"].Value);
                        break;
                    case "coord":
                        cityInfo.setCityCoord(System.Convert.ToSingle(xNodeAttribs["lon"].Value), System.Convert.ToSingle(xNodeAttribs["lat"].Value));
                        break;
                    case "country":
                        cityInfo.setCityCountry(xNode.Value);
                        break;
                    case "temperature":
                        atomInfo = new atominfo();
                        

                }
            }
        }
    }
    
}