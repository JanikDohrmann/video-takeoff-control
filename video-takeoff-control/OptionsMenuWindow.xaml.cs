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

            textVerticalControlLinePosition.Text = Settings.controlLineX.ToString();
            textHorizontalControlLinePosition.Text = Settings.controlLineY.ToString();
            textControlLineWidth.Text = Settings.controlLineWidth.ToString();
            checkShowVerticalControlLine.IsChecked = Settings.showVerticalControlLine;
            checkShowHorizontalControlLine.IsChecked = Settings.showHorizontalControlLine;
            checkCenterControlLine.IsChecked = Settings.centerControlLine;
            
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
            string verticalControlLinePosition = textVerticalControlLinePosition.Text;

            if (Regex.IsMatch(verticalControlLinePosition, "\\d+"))
            {
                Settings.controlLineX = Int32.Parse(verticalControlLinePosition);
            }

            string horizontalControlLinePosition = textHorizontalControlLinePosition.Text;

            if (Regex.IsMatch(horizontalControlLinePosition, "\\d+"))
            {
                Settings.controlLineY = Int32.Parse(horizontalControlLinePosition);
            }

            string controlLineWidth = textControlLineWidth.Text;

            if (Regex.IsMatch(controlLineWidth, "\\d+"))
            {
                Settings.controlLineWidth = Int32.Parse(controlLineWidth);
            }

            Settings.showVerticalControlLine = checkShowVerticalControlLine.IsChecked.GetValueOrDefault(true);
            Settings.showHorizontalControlLine = checkShowHorizontalControlLine.IsChecked.GetValueOrDefault(true);
            Settings.centerControlLine = checkCenterControlLine.IsChecked.GetValueOrDefault(false);
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
