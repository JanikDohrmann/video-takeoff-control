﻿using System;
using System.Collections.Generic;
using video_takeoff_control.logging;
using video_takeoff_control.video_source;

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
        public static VideoSourceType selectedVideoSourceType;
        public static Dictionary<string, string> httpVideoSourceURL;

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

            selectedVideoSourceType = VideoSourceType.Webcam;
            httpVideoSourceURL = new Dictionary<string, string>();
            httpVideoSourceURL.Add("cam1", "http://10.1.1.69:8080/shot.jpg");

            MainWindow.GetLogger().Log(LogLevel.Information, "Setting initialized!");
        }
    }
}
