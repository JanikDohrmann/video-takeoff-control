using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<VideoSourceSettings> videoSourceSettingsItems { get; set; }

        public OptionsMenuWindow()
        {
            this.Close();
        }

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

            List<string> controlLineColorOptions = new List<string> { "Red", "Blue", "Yellow", "Green", "Black" };
            comboControlLineColor.ItemsSource = controlLineColorOptions;
            comboControlLineColor.SelectedIndex = controlLineColorOptions.IndexOf(settings.controlLineColor);

            textVideoStoragePath.Text = settings.storageFolderPath;

            videoSourceSettingsItems = new ObservableCollection<VideoSourceSettings>(settings.videoSources);
            DataContext = this;
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

        private void AddVideoSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VideoSourceEditorWindow videoSourceEditorWindow = new VideoSourceEditorWindow(settings, this);
                videoSourceEditorWindow.Show();
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im AddVideoSource_Click Button: {ex.ToString()}");
            }

        }

        private void EditVideoSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = listVideoSources.SelectedIndex;
                if (index >= 0)
                {
                    VideoSourceEditorWindow videoSourceEditorWindow = new VideoSourceEditorWindow(settings, this, settings.videoSources[index]);
                    videoSourceEditorWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im EditVideoSource_Click Button: {ex.ToString()}");
            }

        }

        private void RemoveVideoSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = listVideoSources.SelectedIndex;
                if (index >= 0)
                {
                    VideoSourceSettings videoSourceSettings = settings.videoSources[index];
                    settings.videoSources.RemoveAt(index);
                    videoSourceSettingsItems.Remove(videoSourceSettings);
                }
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im EditVideoSource_Click Button: {ex.ToString()}");
            }

        }

        private void ChangeVideoSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.storeSettings(settings);
                _mainWindow.setup(settings);
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im ChangeVideoSource_Click Button: {ex.ToString()}");
            }

        }

        public void updateUIVideoSourcesList()
        {
            videoSourceSettingsItems.Clear();
            foreach (VideoSourceSettings item in (settings.videoSources))
            {
                videoSourceSettingsItems.Add(item);
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

            settings.controlLineColor = comboControlLineColor.Text;

            Settings.storeSettings(settings);
            _mainWindow.setup(settings);
        }

        private void SaveVideoStorage_Click(object sender, RoutedEventArgs e)
        {
            settings.storageFolderPath = textVideoStoragePath.Text;

            Settings.storeSettings(settings);
            _mainWindow.setup(settings);
        }
    }
}
