<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="RpiWebService.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>WebCalendar</title>
    <script type="text/javascript" src="Scripts/clock.js"></script>
    <link rel="stylesheet" href="css/clock.css" />
    <link rel="stylesheet" href="css/main.css" />
</head>
<body>
    <form runat="server">
        <asp:ScriptManager runat="server">
            <Scripts>
                <%--若要了解有关在 ScriptManager 中绑定脚本的详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkID=301884 --%>
                <%--框架脚本--%>
                <asp:ScriptReference Name="MsAjaxBundle" />
                <asp:ScriptReference Name="jquery" />
                <asp:ScriptReference Name="bootstrap" />
                <asp:ScriptReference Name="respond" />
                <asp:ScriptReference Name="WebForms.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebForms.js" />
                <asp:ScriptReference Name="WebUIValidation.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebUIValidation.js" />
                <asp:ScriptReference Name="MenuStandards.js" Assembly="System.Web" Path="~/Scripts/WebForms/MenuStandards.js" />
                <asp:ScriptReference Name="GridView.js" Assembly="System.Web" Path="~/Scripts/WebForms/GridView.js" />
                <asp:ScriptReference Name="DetailsView.js" Assembly="System.Web" Path="~/Scripts/WebForms/DetailsView.js" />
                <asp:ScriptReference Name="TreeView.js" Assembly="System.Web" Path="~/Scripts/WebForms/TreeView.js" />
                <asp:ScriptReference Name="WebParts.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebParts.js" />
                <asp:ScriptReference Name="Focus.js" Assembly="System.Web" Path="~/Scripts/WebForms/Focus.js" />
                <asp:ScriptReference Name="WebFormsBundle" />
                <%--站点脚本--%>
            </Scripts>
        </asp:ScriptManager>
    </form>
    <div class="PageContainer">
        <div class="clockContainer">
            <div class="clock">
                <div class="clock-face">
                    <div class="clock-hands">
                        <div class="hand hour-hand"></div>
                        <div class="hand min-hand"></div>
                        <div class="hand second-hand"></div>
                        <div class="clock-text">
                            <div class="clockText Top">12</div>
                            <div class="clockText Middle-Left">-9</div>
                            <div class="clockText Middle-Right">3-</div>
                            <div class="clockText Bottom">6</div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
        <div class="DateTimeContainer">
            <div class="CapitalText" id="dateLine"></div>
            <div class="CapitalText small" id="timeLine"></div>
        </div>
        <div class="WeatherContainer">
            <div class="WeatherUpperBlock">
                <div class="WeatherLogo">
                    <img id="curWeather"/>
                </div>
                <span id="weatherLocation"></span>
                <br />
                <span id="weatherText"></span>
            </div>
            <div class="WeatherLowerBlock">
                
            </div>
        </div>
        <div class="ScheduleContainer">
        </div>
    </div>
</body>
</html>
<script type="text/javascript">
    const secHand = document.querySelector('.second-hand');
    const minHand = document.querySelector('.min-hand');
    const hourHand = document.querySelector('.hour-hand');
    initDate();
    setInterval(updateDate, 1000);

    const DateTextLine = document.getElementById('dateLine');
    const TimeTextLine = document.getElementById('timeLine');

    setInterval(function () {
        var oDate = new Date();
        DateTextLine.textContent = formatDate(oDate, 'YYYY/MM/DD 星期');
        TimeTextLine.textContent = strftime('%O %H:%M:%S');
    }, 1000);

    var wSocket_weather = new WebSocket('ws://192.168.1.199:10080/Api/WeatherHandler.ashx');
    var wSocket_sched = new WebSocket('ws://192.168.1.199:10080/Api/ScheduleHandler.ashx');

    wSocket_weather.onopen = function (evt)
    {
        //Supported location types: City, CityCountry, Geo, Zip
        wSocket_weather.send('locationType=CityCountry;location=Albuquerque,NM;metricType=1')
    }

</script>
