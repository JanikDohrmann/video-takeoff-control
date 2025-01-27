using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using video_takeoff_control.logging;
using video_takeoff_control.video_source;

namespace video_takeoff_control
{
    /// <summary>
    /// Interaktionslogik für OptionsMenuWindow.xaml
    /// </summary>
    public partial class OptionsMenuWindow : Window
    {
        private MainWindow _mainWindow;
        public OptionsMenuWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();

            comboLanguage.SelectedIndex = 0;

            textControlLinePosition.Text = Settings.controlLineX.ToString();
            textControlLineWidth.Text = Settings.controlLineWidth.ToString();
            checkShowControlLine.IsChecked = Settings.showControlLine;
            
            List<String> controlLineColorOptions = new List<String>();
            controlLineColorOptions.Add(Settings.controlLineColor.Name);
            comboControlLineColor.ItemsSource = controlLineColorOptions;
            comboControlLineColor.SelectedIndex = 0;

            textVideoStoragePath.Text = Settings.storageFolderPath;
            textFrameRate.Text = Settings.framerate.ToString();

            textHttpCameraUrl.Text = Settings.httpVideoSourceURL["cam1"];
            comboVideoSourceType.SelectedIndex = (int) Settings.selectedVideoSourceType;
        }

        private void ChangeCommon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string culture = comboLanguage.SelectedValue.ToString();
                Settings.uiCulture = culture;
                App.ChangeLanguage(culture);
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im ChangeCommon_Click Button: {ex.ToString()}");
            }
        }

        private void ChangeVideoSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.httpVideoSourceURL["cam1"] = textHttpCameraUrl.Text;
                VideoSourceType videoSourceType = comboVideoSourceType.SelectedIndex == 0 ? VideoSourceType.Webcam : VideoSourceType.SimpleHttpCamera;
                Settings.selectedVideoSourceType = videoSourceType;
                MainWindow.GetLogger().Log(LogLevel.Debug, $"Video Source Type selected: {videoSourceType.ToString()}");
                _mainWindow.setupCamera(videoSourceType);
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im ChangeVideoSource_Click Button: {ex.ToString()}");
            }

        }

        private void ChangeControlLine_Click(object sender, RoutedEventArgs e)
        {
            string controlLinePosition = textControlLinePosition.Text;

            if (Regex.IsMatch(controlLinePosition, "\\d+"))
            {
                Settings.controlLineX = Int32.Parse(controlLinePosition);
            }

            string controlLineWidth = textControlLineWidth.Text;

            if (Regex.IsMatch(controlLineWidth, "\\d+"))
            {
                Settings.controlLineWidth = Int32.Parse(controlLineWidth);
            }

            Settings.showControlLine = checkShowControlLine.IsChecked.GetValueOrDefault(true);
        }

        private void SaveVideoStorage_Click(object sender, RoutedEventArgs e)
        {
            Settings.storageFolderPath = textVideoStoragePath.Text;

            string framerate = textFrameRate.Text;

            if (Regex.IsMatch(framerate, "\\d+"))
            {
                Settings.framerate = Int32.Parse(framerate);
            }
        }
    }
}
