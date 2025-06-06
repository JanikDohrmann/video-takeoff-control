﻿using System;
using System.Collections.Generic;
using video_takeoff_control.logging;
using video_takeoff_control.video_source;
using video_takeoff_control.util;
using System.IO;

namespace video_takeoff_control.settings
{
    public class Settings
    {
        private static string SETTINGS_STORAGE_PATH = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "video-takeoff-control", "settings.xml");

        //UI settings
        public string uiCulture { get; set; }

        //Control Line
        public int controlLineX { get; set; }
        public int controlLineY { get; set; }
        public int controlLineWidth { get; set; }
        public string controlLineColor { get; set; }
        public bool showVerticalControlLine { get; set; }
        public bool showHorizontalControlLine { get; set; }
        public bool centerControlLine { get; set; }

        //Video Storage
        public string storageFolderPath { get; set; }

        //Competition
        public string competitionName { get; set; }
        public int attemptNumber { get; set; }

        //Video Source
        public List<VideoSourceSettings> videoSources { get; set; }

        //Init
        public static Settings loadSettings()
        {
            Settings settings = new Settings
            {
                uiCulture = "en-UK",
                controlLineX = 0,
                controlLineY = 0,
                controlLineWidth = 5,
                controlLineColor = System.Drawing.Color.Red.Name,
                showVerticalControlLine = true,
                showHorizontalControlLine = false,
                centerControlLine = false,
                storageFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "video-takeoff-control", "recordings"),
                competitionName = "Wettkampfname",
                attemptNumber = 0,
                videoSources = new List<VideoSourceSettings>(),
            };


            if (File.Exists(SETTINGS_STORAGE_PATH))
            {
                MainWindow.GetLogger().Log(LogLevel.Information, "Existing settings file found. Reloading.");
                string xml = File.ReadAllText(SETTINGS_STORAGE_PATH);
                settings = XmlTools.DeserializeFromXml<Settings>(xml);
            }

            App.ChangeLanguage(settings.uiCulture);

            MainWindow.GetLogger().Log(LogLevel.Information, "Setting initialized!");
            return settings;
        }

        public static void storeSettings(Settings settings)
        {
            string xml = XmlTools.SerializeToXml(settings);
            File.WriteAllText(SETTINGS_STORAGE_PATH, xml);
        }
    }
}
