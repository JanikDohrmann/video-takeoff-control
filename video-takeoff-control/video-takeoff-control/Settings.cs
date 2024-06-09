﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void initializeSettings()
        {
            controlLineX = 0;
            controlLineWidth = 5;
            controlLineColor = System.Drawing.Color.Red;
            showControlLine = true;

            storageFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "video-takeoff-control");
        }
    }
}
