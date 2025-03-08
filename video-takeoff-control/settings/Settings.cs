using System;
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
        public System.Drawing.Color controlLineColor { get; set; }
        public bool showVerticalControlLine { get; set; }
        public bool showHorizontalControlLine { get; set; }
        public bool centerControlLine { get; set; }

        //Video Storage
        public string storageFolderPath { get; set; }
        public int framerate { get; set; }

        //Competition
        public string competitionName { get; set; }

        //Video Source
        public VideoSourceType selectedVideoSourceType { get; set; }
        public Dictionary<string, string> httpVideoSourceURL { get; set; }

        //Init
        public static Settings loadSettings()
        {
            Settings settings = new Settings
            {
                uiCulture = "en-UK",
                controlLineX = 0,
                controlLineY = 0,
                controlLineWidth = 5,
                controlLineColor = System.Drawing.Color.Red,
                showVerticalControlLine = true,
                showHorizontalControlLine = false,
                centerControlLine = false,
                storageFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "video-takeoff-control", "recordings"),
                framerate = 30,
                competitionName = "Wettkampfname",
                selectedVideoSourceType = VideoSourceType.Webcam,
                httpVideoSourceURL = new Dictionary<string, string>(),
            };

            settings.httpVideoSourceURL.Add("cam1", "http://10.1.1.69:8080/shot.jpg");


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
