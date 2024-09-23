using System;
using System.Collections.Generic;
using video_takeoff_control.logging;

namespace video_takeoff_control
{
    internal class Settings
    {
        //Control Line
        public static int controlLineX;
        public static int controlLineWidth;
        public static System.Drawing.Color controlLineColor;
        public static bool showControlLine;

        //Video Storage
        public static string storageFolderPath;
        public static int framerate;

        //Competition
        public static string competitionName;

        //Video Source
        public static Dictionary<string, string> videoSourceReceiveIps;
        public static Dictionary<string, string> videoSourceSendIps;
        public static Dictionary<string, int> videoSourceReceivePorts;
        public static Dictionary<string, int> videoSourceSendPorts;
        public static Dictionary<string, int> videoSourceImageSize;

        //Init
        public static void initializeSettings()
        {
            controlLineX = 0;
            controlLineWidth = 5;
            controlLineColor = System.Drawing.Color.Red;
            showControlLine = true;

            storageFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "video-takeoff-control");
            framerate = 30;
            

            competitionName = "Wettkampfname";

            videoSourceSendIps = new Dictionary<string, string>();
            videoSourceSendIps.Add("cam1", "127.0.0.1");
            videoSourceSendPorts = new Dictionary<string, int>();
            videoSourceSendPorts.Add("cam1", 10526);
            videoSourceReceiveIps = new Dictionary<string, string>();
            videoSourceReceiveIps.Add("cam1", "127.0.0.1");
            videoSourceReceivePorts = new Dictionary<string, int>();
            videoSourceReceivePorts.Add("cam1", 10525);
            videoSourceImageSize = new Dictionary<string, int>();
            videoSourceReceivePorts.Add("cam1", 10525);

            MainWindow.GetLogger().Log(LogLevel.Information, "Setting initialized!");
        }
    }
}
