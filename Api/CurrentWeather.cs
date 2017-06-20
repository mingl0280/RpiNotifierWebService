using System;
using System.Xml;

namespace RpiWebService.Api
{
    public class CurrentWeather
    {
        public int cityId { get; set; }
        public cityinfo CityInfo { get { return cityInfo; } }
        public int MetricType { get; set; }
        public atominfo AtomosphereInfo { get { return atomInfo; } }
        public sunTime SunTime { get { return sTime; } }
        public wind WindStatus { get { return wStatus; } }
        public cloudinfo CloudInfo { get { return cloudInfo; } }
        public long Visibility { get { return visibility; } }
        public precipitationinfo PrecipitationInfo { get { return precipitationInfo; } }
        public weatherinfo WeatherInfo { get { return weatherInfo; } }
        public DateTime LastUpdate { get => lstUpdate; }

        public bool IsReady { get; set; }
        

        private cityinfo cityInfo = new cityinfo();
        private atominfo atomInfo = new atominfo();
        private sunTime sTime = new sunTime();
        private wind wStatus = new wind();
        private cloudinfo cloudInfo = new cloudinfo();
        private long visibility;
        private precipitationinfo precipitationInfo = new precipitationinfo();
        private weatherinfo weatherInfo = new weatherinfo();
        private DateTime lstUpdate;

        public enum metricType
        {
            kelvin = 0,
            metric = 1,
            imperial = 2
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
            { }
            public cityinfo(string name)
            {
                cityName = name;
            }
        }

        public class sunTime
        {
            public String SunRise { get { return sunRise; } }
            public String SunSet { get { return sunSet; } }

            private string sunRise;
            private string sunSet;

            public void SetSunTime(string Rise, string Set)
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
            public float Direction { get { return direction; } }
            public string DirectionCode { get { return directionCode; } }
            public string DirectionName { get { return directionName; } }

            private float speed;
            private string windName;
            private float direction;
            private string directionCode;
            private string directionName;
            

            public wind()
            {   }
            public wind(float spd, string wname, int dir, string dcode, string dname)
            {
                speed = spd;
                windName = wname;
                direction = dir;
                directionCode = dcode;
                directionName = dname;
            }
            /// <summary>
            /// Wind initilizer by wind speed and name
            /// </summary>
            /// <param name="spd">Wind Speed (m/s for metric or mi/h for imperial)</param>
            /// <param name="wname">Wind Name</param>
            public wind(float spd, string wname)
            {
                speed = spd;
                windName = wname;
            }

            /// <summary>
            /// Set wind speed and name info
            /// </summary>
            /// <param name="spd">Wind Speed (m/s for metric or mi/h for imperial)</param>
            /// <param name="wname">Wind Name</param>
            public void setwindSpeed(float spd, string wname)
            {
                speed = spd;
                windName = wname;
            }

            /// <summary>
            /// Set wind direction info
            /// </summary>
            /// <param name="dir">Direction degree</param>
            /// <param name="dcode">Direction code</param>
            /// <param name="dname">Direction Name</param>
            public void setwindDir(float dir, string dcode, string dname)
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

            /// <summary>
            /// Set temperature status for today
            /// </summary>
            /// <param name="now">Current Temperature</param>
            /// <param name="min">Minimial Temperature</param>
            /// <param name="max">Maximum Temperature</param>
            public void SetTemp(float now, float min, float max)
            {
                tempNow = now;
                tempMin = min;
                tempMax = max;
            }

            /// <summary>
            /// Set current humidity value
            /// </summary>
            /// <param name="hum">Humidity value</param>
            public void SetHumidity(int hum)
            {
                humdity = hum;
            }

            /// <summary>
            /// Set current atmosphere pressure status
            /// </summary>
            /// <param name="pre">Atmosphere pressure</param>
            /// <param name="preUnit">Pressure Unit</param>
            public void SetPressure(int pre, string preUnit)
            {
                pressure = pre;
                pressureUnit = preUnit;
            }

            public atominfo()
            { }

            public atominfo(float now, float min, float max)
            {
                tempNow = now;
                tempMin = min;
                tempMax = max;
            }
        }

        public class cloudinfo
        {
            public int CloudType { get { return cloudType; } }
            public string CloudName { get { return cloudName; } }

            private int cloudType;
            private string cloudName;
            public cloudinfo() { }
            /// <summary>
            /// Cloud Info Initilizer
            /// </summary>
            /// <param name="cltype"></param>
            /// <param name="clname"></param>
            public cloudinfo(int cltype, string clname)
            {
                cloudType = cltype;
                cloudName = clname;
            }
            public void SetCloudInfo(int cltype, string clname)
            {
                cloudType = cltype;
                cloudName = clname;
            }
        }

        public class weatherinfo
        {
            public int WeatherId { get { return weatherId; } }
            public string WeatherDescription { get { return weatherDescription; } }
            public string IconId { get { return iconId; } }
            private int weatherId;
            private string weatherDescription;
            private string iconId;

            public weatherinfo()
            { }

            /// <summary>
            /// Weather Info Initilizer
            /// </summary>
            /// <param name="id">Weather ID</param>
            /// <param name="desc">Weather Description</param>
            /// <param name="ico">Weather Icon ID String</param>
            public void SetWeatherinfo(int id, string desc, string ico)
            {
                weatherId = id;
                weatherDescription = desc;
                iconId = ico;
            }
        }

        public class precipitationinfo
        {
            public float Precipitation { get { return precipitation; } }
            public string Mode { get { return mode; } }
            public string IconId { get { return unit; } }
            private float precipitation;
            private string mode;
            private string unit;

            public precipitationinfo()
            { }

            /// <summary>
            /// Precipation Info Initializer
            /// </summary>
            /// <param name="prec">Precipitation Value</param>
            /// <param name="m">Precipitation Mode(Rain or Snow)</param>
            /// <param name="u">Precipitation Timing Unit</param>
            public void SetPrecipitationInfo(float prec, string m, string u)
            {
                precipitation = prec;
                mode = m;
                unit = u;
            }

            /// <summary>
            /// Precipation Info Initializer (no precipitation)
            /// </summary>
            /// <param name="m">Precipitation Mode is NO</param>
            public void SetPrecipitationInfo(string m)
            {
                mode = m;
                precipitation = 0.0f;
                unit = "";
            }
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

        /// <summary>
        /// Fetch Weather Data From OpenWeatherMap.org
        /// </summary>
        /// <param name="OpenWeatherAPIUrl">Base query url(select by city, zip or anything else)</param>
        /// <param name="ApiKey">OpenWeatherMap API Key</param>
        /// <param name="mType">Unit Type</param>
        /// <param name="intMetricType">Metric Type Integer, 1 = metric units, 2 = imperial units, 3 = iso units</param>
        /// <param name="language">Language to perform the query</param>
        /// <returns></returns>
        public bool fetchWeatherData(string OpenWeatherAPIUrl, string ApiKey, int intMetricType, string language = "")
        {
            IsReady = false;
            try
            {
                XmlDocument xDoc = new XmlDocument();
                string queryMetricType = "";
                MetricType = intMetricType;
                //MetricType = GetMetricType(intMetricType);
                switch (intMetricType)
                {
                    case 0:
                        break;
                    case 2:
                        queryMetricType = "&units=imperial";
                        break;
                    case 1:
                        queryMetricType = "&units=metric";
                        break;
                    default:
                        queryMetricType = "&units=metric";
                        break;
                }
                xDoc.Load(OpenWeatherAPIUrl + queryMetricType + "&mode=xml" + ((language == "") ? "" : "&lang=" + language) + "&appid=" + ApiKey);

                if (xDoc.HasChildNodes)
                {
                    ProcNodeValues(xDoc.ChildNodes);
                }
                else
                {
                    return false;
                }
                IsReady = true;
                return true;
            }
            catch (Exception e)
            {
                return false; }
        }

        public metricType GetMetricType(int _input)
        {
            switch (_input)
            {
                case 1:
                    return metricType.metric;
                case 2:
                    return metricType.imperial;
                default:
                    return metricType.kelvin;
            }
        }

        private void ProcNodeValues(XmlNodeList xNodeList)
        {
            foreach (XmlNode xNode in xNodeList)
            {
                if (xNode.HasChildNodes)
                {
                    ProcNodeValues(xNode.ChildNodes);
                }
                var xNodeAttribs = xNode.Attributes;
                switch (xNode.Name)
                {
                    case "city":
                        cityInfo.setCityName(xNodeAttribs["name"].Value);
                        break;
                    case "coord":
                        cityInfo.setCityCoord(Convert.ToSingle(xNodeAttribs["lon"].Value), Convert.ToSingle(xNodeAttribs["lat"].Value));
                        break;
                    case "country":
                        cityInfo.setCityCountry(xNode.Value);
                        break;
                    case "temperature":
                        atomInfo.SetTemp(Convert.ToSingle(xNodeAttribs["value"].Value),
                            Convert.ToSingle(xNodeAttribs["min"].Value),
                            Convert.ToSingle(xNodeAttribs["max"].Value));
                        break;
                    case "humidity":
                        atomInfo.SetHumidity(Convert.ToInt32(xNodeAttribs["value"].Value));
                        break;
                    case "pressure":
                        atomInfo.SetPressure(Convert.ToInt32(xNodeAttribs["value"].Value), xNodeAttribs["unit"].Value);
                        break;
                    case "speed":
                        wStatus.setwindSpeed(Convert.ToSingle(xNodeAttribs["value"].Value),
                            xNodeAttribs["name"].Value);
                        break;
                    case "direction":
                        wStatus.setwindDir(Convert.ToSingle(xNodeAttribs["value"].Value),
                            xNodeAttribs["code"].Value,
                            xNodeAttribs["name"].Value);
                        break;
                    case "clouds":
                        cloudInfo = new cloudinfo(Convert.ToInt32(xNodeAttribs["value"].Value), xNodeAttribs["name"].Value);
                        break;
                    case "visibility":
                        if (xNodeAttribs["value"] != null)
                        visibility = Convert.ToInt64(xNodeAttribs["value"].Value);
                        break;
                    case "precipitation":
                        string mode = xNodeAttribs["mode"].Value;
                        if (mode == "rain" || mode == "snow")
                        {
                            precipitationInfo.SetPrecipitationInfo(
                                Convert.ToSingle(xNodeAttribs["value"].Value),
                                mode,
                                xNodeAttribs["unit"].Value
                            );
                        }
                        else
                        {
                            precipitationInfo.SetPrecipitationInfo(mode);
                        }
                        break;
                    case "weather":
                        weatherInfo.SetWeatherinfo(Convert.ToInt32(xNodeAttribs["number"].Value),
                            xNodeAttribs["value"].Value,
                            xNodeAttribs["icon"].Value);
                        break;
                    case "sun":
                        SunTime.SetSunTime(xNodeAttribs["rise"].Value, xNodeAttribs["set"].Value);
                        break;
                    case "lastupdate":
                        lstUpdate = DateTime.Parse(xNodeAttribs["value"].Value);
                        break;
                }
            }
        }
    }
}