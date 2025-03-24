using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using video_takeoff_control.logging;
using video_takeoff_control.settings;
using video_takeoff_control.video_source;
using AForge.Video.DirectShow;
using video_takeoff_control.video_source_tools;

namespace video_takeoff_control
{
    /// <summary>
    /// Interaktionslogik für VideoSourceEditorWindow.xaml
    /// </summary>
    public partial class VideoSourceEditorWindow : Window
    {
        private Settings settings;
        private VideoSourceSettings videoSourceSettings;
        private OptionsMenuWindow optionsMenuWindow;

        private FilterInfoCollection videoDevices;

        public VideoSourceEditorWindow(Settings settings, OptionsMenuWindow optionsMenuWindow)
        {
            this.settings = settings;
            this.optionsMenuWindow = optionsMenuWindow;
            InitializeComponent();

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach(FilterInfo videoDevice in videoDevices)
            {
                comboVideoSourceWebcamSelect.Items.Add(videoDevice.Name + " | " + MonikerStringParser.parseDevicePath(videoDevice.MonikerString));
            }
        }

        public VideoSourceEditorWindow(Settings settings, OptionsMenuWindow optionsMenuWindow, VideoSourceSettings videoSourceSettings)
        {
            this.settings = settings;
            this.optionsMenuWindow = optionsMenuWindow;
            this.videoSourceSettings = videoSourceSettings;
            InitializeComponent();

            textCameraName.Text = videoSourceSettings.name;         
            comboVideoSourceType.SelectedValue = videoSourceSettings.videoSourceType;
            textHttpCameraUrl.Text = videoSourceSettings.hostname;
            textFramerate.Text = videoSourceSettings.framerate.ToString();

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach (FilterInfo videoDevice in videoDevices)
            {
                comboVideoSourceWebcamSelect.Items.Add(videoDevice.Name + " | " + MonikerStringParser.parseDevicePath(videoDevice.MonikerString));

                if(videoSourceSettings.deviceAddress.Equals(videoDevice.MonikerString))
                {
                    comboVideoSourceWebcamSelect.SelectedItem = videoDevice.Name + " | " + MonikerStringParser.parseDevicePath(videoDevice.MonikerString);
                }
            }
        }

        private void SaveVideoSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(videoSourceSettings != null)
                {
                    string cameraName = textCameraName.Text;
                    VideoSourceType videoSourceType = comboVideoSourceType.SelectedIndex == 0 ? VideoSourceType.Webcam : VideoSourceType.SimpleHttpCamera;
                    string hostname = textHttpCameraUrl.Text;
                    int framerate = Convert.ToInt32(textFramerate.Text);

                    string webcamSelect = comboVideoSourceWebcamSelect.SelectedItem.ToString();

                    videoSourceSettings.name = cameraName;
                    videoSourceSettings.framerate = framerate;
                    videoSourceSettings.hostname = hostname;
                    videoSourceSettings.videoSourceType = videoSourceType;
                    foreach(FilterInfo videoDevice in videoDevices)
                    {
                        if(videoDevice.MonikerString.Contains(webcamSelect.Substring(webcamSelect.IndexOf(" | ") + 3)))
                        {
                            videoSourceSettings.deviceAddress = videoDevice.MonikerString;
                        }
                    }
                }
                else
                {
                    string cameraName = textCameraName.Text;
                    VideoSourceType videoSourceType = comboVideoSourceType.SelectedIndex == 0 ? VideoSourceType.Webcam : VideoSourceType.SimpleHttpCamera;
                    string hostname = textHttpCameraUrl.Text;
                    int framerate = Convert.ToInt32(textFramerate.Text);
                    
                    string deviceAddress = "";
                    string webcamSelect = comboVideoSourceWebcamSelect.SelectedItem.ToString();
                    foreach (FilterInfo videoDevice in videoDevices)
                    {
                        if (videoDevice.MonikerString.Contains(webcamSelect.Substring(webcamSelect.IndexOf(" | ") + 3)))
                        {
                            deviceAddress = videoDevice.MonikerString;
                        }
                    }
                    settings.videoSources.Add(new VideoSourceSettings(cameraName, videoSourceType, hostname, framerate, deviceAddress));
                }
                optionsMenuWindow.updateUIVideoSourcesList();
                this.Close();
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im SaveVideoSource_Click Button: {ex.ToString()}");
            }

        }
    }
}
