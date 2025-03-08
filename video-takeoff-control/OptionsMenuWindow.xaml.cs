using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using video_takeoff_control.logging;
using video_takeoff_control.settings;
using video_takeoff_control.video_source;

namespace video_takeoff_control
{
    /// <summary>
    /// Interaktionslogik für OptionsMenuWindow.xaml
    /// </summary>
    public partial class OptionsMenuWindow : Window
    {
        private MainWindow _mainWindow;
        private Settings settings;
        public OptionsMenuWindow(MainWindow mainWindow, Settings settings)
        {
            _mainWindow = mainWindow;
            this.settings = settings;
            InitializeComponent();

            comboLanguage.SelectedValue = settings.uiCulture;

            textVerticalControlLinePosition.Text = settings.controlLineX.ToString();
            textHorizontalControlLinePosition.Text = settings.controlLineY.ToString();
            textControlLineWidth.Text = settings.controlLineWidth.ToString();
            checkShowVerticalControlLine.IsChecked = settings.showVerticalControlLine;
            checkShowHorizontalControlLine.IsChecked = settings.showHorizontalControlLine;
            checkCenterControlLine.IsChecked = settings.centerControlLine;
            
            List<String> controlLineColorOptions = new List<String>();
            controlLineColorOptions.Add(settings.controlLineColor.Name);
            comboControlLineColor.ItemsSource = controlLineColorOptions;
            comboControlLineColor.SelectedIndex = 0;

            textVideoStoragePath.Text = settings.storageFolderPath;
            textFrameRate.Text = settings.framerate.ToString();

            textHttpCameraUrl.Text = settings.videoSources[0].hostname;
            comboVideoSourceType.SelectedIndex = (int)settings.videoSources[0].selectedVideoSourceType;
        }

        private void ChangeCommon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string culture = comboLanguage.SelectedValue.ToString();
                settings.uiCulture = culture;
                App.ChangeLanguage(culture);

                Settings.storeSettings(settings);
                _mainWindow.setup(settings);
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
                settings.videoSources[0].hostname = textHttpCameraUrl.Text;
                VideoSourceType videoSourceType = comboVideoSourceType.SelectedIndex == 0 ? VideoSourceType.Webcam : VideoSourceType.SimpleHttpCamera;
                settings.videoSources[0].selectedVideoSourceType = videoSourceType;
                MainWindow.GetLogger().Log(LogLevel.Debug, $"Video Source Type selected: {videoSourceType.ToString()}");
                Settings.storeSettings(settings);
                _mainWindow.setup(settings);
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
                settings.controlLineX = Int32.Parse(verticalControlLinePosition);
            }

            string horizontalControlLinePosition = textHorizontalControlLinePosition.Text;

            if (Regex.IsMatch(horizontalControlLinePosition, "\\d+"))
            {
                settings.controlLineY = Int32.Parse(horizontalControlLinePosition);
            }

            string controlLineWidth = textControlLineWidth.Text;

            if (Regex.IsMatch(controlLineWidth, "\\d+"))
            {
                settings.controlLineWidth = Int32.Parse(controlLineWidth);
            }

            settings.showVerticalControlLine = checkShowVerticalControlLine.IsChecked.GetValueOrDefault(true);
            settings.showHorizontalControlLine = checkShowHorizontalControlLine.IsChecked.GetValueOrDefault(true);
            settings.centerControlLine = checkCenterControlLine.IsChecked.GetValueOrDefault(false);
            Settings.storeSettings(settings);
            _mainWindow.setup(settings);
        }

        private void SaveVideoStorage_Click(object sender, RoutedEventArgs e)
        {
            settings.storageFolderPath = textVideoStoragePath.Text;

            string framerate = textFrameRate.Text;

            if (Regex.IsMatch(framerate, "\\d+"))
            {
                settings.framerate = Int32.Parse(framerate);
            }
            Settings.storeSettings(settings);
            _mainWindow.setup(settings);
        }
    }
}
