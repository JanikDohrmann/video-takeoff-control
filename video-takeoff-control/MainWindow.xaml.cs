using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using video_takeoff_control.bitmap_tools;
using video_takeoff_control.logging;
using video_takeoff_control.settings;
using video_takeoff_control.util;
using video_takeoff_control.video_file_handler;
using video_takeoff_control.video_source;

namespace video_takeoff_control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Window> childWindows = new List<Window>();

        private Settings settings;

        private List<BitmapImage> recordedVideo;
        private int frameCounter;

        private bool recording;

        private IVideoSource videoSource;
        private IVideoFileHandler videoFileHandler;

        private static ILogger logger;

        private System.Timers.Timer recordingTimer;

        private const string VideoStatusNoSource = "NO SOURCE";
        private const string VideoStatusLive = "LIVE";
        private const string VideoStatusRecording = "REC";
        private const string VideoStatusPlayback = "PLAYBACK";
        private const string VideoStatusFile = "FILE";
        private const string VideoStatusLoading = "LOADING";

        public MainWindow()
        {
            string logpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "video-takeoff-control", "logs");
            logger = FileLogger.create(logpath);

            logger.Log(LogLevel.Information, "Starting!");

            try
            {
                
                setup();
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im MainWindow: {ex.ToString()}");
            }
            
        }

        private void setup()
        {
            Settings settings_new = Settings.loadSettings();
            setup(settings_new);
        } 

        public void setup(Settings settings_new)
        {
            this.settings = settings_new;
            if(settings.videoSources.Count > 0)
            {
                setupCamera(settings.videoSources[0]);
                MainWindow.GetLogger().Log(LogLevel.Information, "Videosource created!");
            }

            recordedVideo = new List<BitmapImage>();
            frameCounter = 0;
            recording = false;

            InitializeComponent();

            comboActiveVideoSource.Items.Clear();
            foreach (VideoSourceSettings videoSourceSettings in settings.videoSources)
            {
                comboActiveVideoSource.Items.Add(videoSourceSettings.name);
            }
            if (comboActiveVideoSource.Items.Count > 0)
            {
                comboActiveVideoSource.SelectedIndex = 0;
            }

            comboRecordingTimer.SelectedIndex = 0;

            buttonBack.IsEnabled = false;
            buttonForward.IsEnabled = false;
            buttonStopRecord.IsEnabled = false;
            buttonClear.IsEnabled = false;
            buttonStartRecord.IsEnabled = videoSource != null;
            updateVideoStatus(videoSource != null ? VideoStatusLive : VideoStatusNoSource);

            updateCompetitionName();

            videoFileHandler = new AviFileHandler(settings);
            populateSettingsDrawer();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                resetFrameProgress();
                if (videoSource == null)
                {
                    MainWindow.GetLogger().Log(LogLevel.Warning, "Recording could not be started because no video source is configured.");
                    return;
                }

                videoSource.preview();
                updateVideoStatus(VideoStatusRecording);

                recordedVideo = new List<BitmapImage>();
                frameCounter = 0;
                recording = true;
                buttonStopRecord.IsEnabled = true;
                buttonStopRecord.IsDefault = true;
                buttonStartRecord.IsEnabled = false;
                editCompetitioNameButton.IsEnabled = false;
                comboActiveVideoSource.IsEnabled = false;
                comboRecordingTimer.IsEnabled = false;

                int recordingTimerDuration = RecordingTimerTools.convertRecordingTimerSelection(comboRecordingTimer.SelectedValue.ToString());

                if (recordingTimerDuration > 0)
                {
                    recordingTimer = new System.Timers.Timer(recordingTimerDuration);
                    recordingTimer.Elapsed += recordingTimerElapsed;
                    recordingTimer.AutoReset = false;
                    recordingTimer.Start();
                }
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im Start Button: {ex.ToString()}");
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            stop();
        }

        private void recordingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            MainWindow.GetLogger().Log(LogLevel.Debug, $"Timer stopped.");

            Dispatcher.Invoke(() => {
                stop();
            });
        }

        private void stop()
        {
            try
            {
                recording = false;

                videoSource?.stopRecording();
                frameCounter = recordedVideo.Count > 0 ? recordedVideo.Count - 1 : 0;
                updateFrameProgress();
                buttonBack.IsEnabled = recordedVideo.Count > 0;
                buttonForward.IsEnabled = recordedVideo.Count > 0;
                buttonStopRecord.IsEnabled = false;
                buttonStartRecord.IsEnabled = videoSource != null;
                buttonStartRecord.IsDefault = true;
                buttonClear.IsEnabled = true;
                editCompetitioNameButton.IsEnabled = true;

                if (recordedVideo.Count == 0)
                {
                    MainWindow.GetLogger().Log(LogLevel.Warning, "Recording stopped without captured frames. No video file was written.");
                    updateVideoStatus(videoSource != null ? VideoStatusLive : VideoStatusNoSource);
                    return;
                }

                updateVideoStatus(VideoStatusPlayback);

                int framerate = videoSource?.getFramerate() ?? 0;
                Task.Run(() => videoFileHandler.saveVideo(FileNameBuilder.buildFileName(settings.storageFolderPath, settings.competitionName, settings.attemptNumber), recordedVideo.Select(x => BitmapConversions.bitmapImage2Bitmap(x)).ToList(), framerate));
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Error in stop function: {ex.ToString()}");
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {   
            try
            {
                resetFrameProgress();
                videoSource?.preview();
                updateVideoStatus(videoSource != null ? VideoStatusLive : VideoStatusNoSource);

                buttonBack.IsEnabled = false;
                buttonForward.IsEnabled = false;
                buttonStopRecord.IsEnabled = false;
                buttonClear.IsEnabled = false;
                comboActiveVideoSource.IsEnabled = true;
                comboRecordingTimer.IsEnabled = true;
                buttonStartRecord.IsEnabled = videoSource != null;
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im Clear Button: {ex.ToString()}");
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            moveFrame(1);
        }

        private void BackwardButton_Click(object sender, RoutedEventArgs e)
        {
            moveFrame(-1);
        }

        private void moveFrame(int step)
        {
            if (recordedVideo == null || recordedVideo.Count == 0)
            {
                resetFrameProgress();
                return;
            }

            if(step > 0) 
            {
                int newFrameCounter = frameCounter < recordedVideo.Count - 1 ? frameCounter + step : recordedVideo.Count - 1;
                frameCounter = newFrameCounter;
            }
            else if(step < 0)
            {
                int newFrameCounter = frameCounter > 0 ? frameCounter + step : 0;
                frameCounter = newFrameCounter;
                
            }

            updateFrameProgress();
            Dispatcher.BeginInvoke(new Action(() => image.Source = recordedVideo[frameCounter]));
        }

        private void updateFrameProgress()
        {
            if (recordedVideo == null || recordedVideo.Count <= 1)
            {
                framePogress.Value = 0;
                return;
            }

            int maxFrames = recordedVideo.Count - 1;

            double progress = ((double)frameCounter / maxFrames) * 100;

            framePogress.Value = progress;
        }

        private void resetFrameProgress()
        {
            framePogress.Value = 0;
        }

        private void updateVideoStatus(string status)
        {
            if (textVideoStatus != null)
            {
                textVideoStatus.Text = status;
            }
        }

        private void updateLoadingVideoOverlay(bool isLoading)
        {
            if (loadingVideoOverlay != null)
            {
                loadingVideoOverlay.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            videoSource?.close();

            foreach(Window window in childWindows)
            {
                window.Close();
            }

            logger.Log(LogLevel.Information, "Shutting down!");

            base.OnClosed(e);
        }

        private void openOptionsMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                toggleSettingsDrawer();
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Error in openOptionsMenu Button: {ex.ToString()}");
            }
        }

        private void toggleSettingsDrawer()
        {
            if (settingsDrawer == null)
            {
                return;
            }

            if (settingsDrawer.Visibility == Visibility.Visible)
            {
                settingsDrawer.Visibility = Visibility.Collapsed;
                return;
            }

            populateSettingsDrawer();
            settingsDrawer.Visibility = Visibility.Visible;
        }

        private void closeSettingsDrawer_Click(object sender, RoutedEventArgs e)
        {
            settingsDrawer.Visibility = Visibility.Collapsed;
        }

        private void populateSettingsDrawer()
        {
            if (comboDrawerLanguage == null || settings == null)
            {
                return;
            }

            comboDrawerLanguage.SelectedValue = settings.uiCulture;
            textDrawerVerticalControlLinePosition.Text = settings.controlLineX.ToString();
            textDrawerHorizontalControlLinePosition.Text = settings.controlLineY.ToString();
            textDrawerControlLineWidth.Text = settings.controlLineWidth.ToString();
            checkDrawerShowVerticalControlLine.IsChecked = settings.showVerticalControlLine;
            checkDrawerShowHorizontalControlLine.IsChecked = settings.showHorizontalControlLine;
            checkDrawerCenterControlLine.IsChecked = settings.centerControlLine;

            List<string> controlLineColorOptions = new List<string> { "Red", "Blue", "Yellow", "Green", "Black" };
            comboDrawerControlLineColor.ItemsSource = controlLineColorOptions;
            comboDrawerControlLineColor.SelectedItem = settings.controlLineColor;

            textDrawerVideoStoragePath.Text = settings.storageFolderPath;
            refreshDrawerVideoSources();
        }

        private void refreshDrawerVideoSources()
        {
            if (listDrawerVideoSources == null || settings == null)
            {
                return;
            }

            listDrawerVideoSources.ItemsSource = null;
            listDrawerVideoSources.ItemsSource = settings.videoSources;
        }

        private void setupAndKeepSettingsDrawerOpen()
        {
            setup(settings);
            if (settingsDrawer != null)
            {
                settingsDrawer.Visibility = Visibility.Visible;
            }
        }

        private void SaveDrawerCommon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string culture = comboDrawerLanguage.SelectedValue?.ToString() ?? settings.uiCulture;
                settings.uiCulture = culture;
                App.ChangeLanguage(culture);

                Settings.storeSettings(settings);
                setupAndKeepSettingsDrawerOpen();
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Error in SaveDrawerCommon Button: {ex.ToString()}");
            }
        }

        private void SaveDrawerControlLine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(textDrawerVerticalControlLinePosition.Text, out int verticalControlLinePosition))
                {
                    settings.controlLineX = verticalControlLinePosition;
                }

                if (int.TryParse(textDrawerHorizontalControlLinePosition.Text, out int horizontalControlLinePosition))
                {
                    settings.controlLineY = horizontalControlLinePosition;
                }

                if (int.TryParse(textDrawerControlLineWidth.Text, out int controlLineWidth))
                {
                    settings.controlLineWidth = controlLineWidth;
                }

                settings.showVerticalControlLine = checkDrawerShowVerticalControlLine.IsChecked.GetValueOrDefault(true);
                settings.showHorizontalControlLine = checkDrawerShowHorizontalControlLine.IsChecked.GetValueOrDefault(true);
                settings.centerControlLine = checkDrawerCenterControlLine.IsChecked.GetValueOrDefault(false);
                settings.controlLineColor = comboDrawerControlLineColor.Text;

                Settings.storeSettings(settings);
                setupAndKeepSettingsDrawerOpen();
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Error in SaveDrawerControlLine Button: {ex.ToString()}");
            }
        }

        private void SaveDrawerVideoStorage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                settings.storageFolderPath = textDrawerVideoStoragePath.Text;

                Settings.storeSettings(settings);
                setupAndKeepSettingsDrawerOpen();
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Error in SaveDrawerVideoStorage Button: {ex.ToString()}");
            }
        }

        private void AddDrawerVideoSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VideoSourceEditorWindow videoSourceEditorWindow = new VideoSourceEditorWindow(settings, refreshDrawerVideoSources);
                childWindows.Add(videoSourceEditorWindow);
                videoSourceEditorWindow.Show();
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Error in AddDrawerVideoSource Button: {ex.ToString()}");
            }
        }

        private void EditDrawerVideoSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = listDrawerVideoSources.SelectedIndex;
                if (index >= 0)
                {
                    VideoSourceEditorWindow videoSourceEditorWindow = new VideoSourceEditorWindow(settings, refreshDrawerVideoSources, settings.videoSources[index]);
                    childWindows.Add(videoSourceEditorWindow);
                    videoSourceEditorWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Error in EditDrawerVideoSource Button: {ex.ToString()}");
            }
        }

        private void RemoveDrawerVideoSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = listDrawerVideoSources.SelectedIndex;
                if (index >= 0)
                {
                    settings.videoSources.RemoveAt(index);
                    refreshDrawerVideoSources();
                }
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Error in RemoveDrawerVideoSource Button: {ex.ToString()}");
            }
        }

        private void SaveDrawerVideoSources_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.storeSettings(settings);
                setupAndKeepSettingsDrawerOpen();
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Error in SaveDrawerVideoSources Button: {ex.ToString()}");
            }
        }

        public void newFrame(Bitmap frame)
        {
            ControlLine.drawControlLine(frame, settings);
            BitmapTools.addMetadata(frame);
            BitmapImage bitmapImage = BitmapConversions.bitmap2BitmapImage(frame);

            if (recording)
            {
                recordedVideo.Add(bitmapImage);
            }

            Dispatcher.BeginInvoke(new Action(() => { image.Source = bitmapImage; }));
        }

        private void openAboutWindow_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            childWindows.Add(aboutWindow);
            aboutWindow.Show();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static ILogger GetLogger()
        {
            if(logger == null)
            {
                logger = FileLogger.create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "\\videoo-takeoff-control\\logs\\"));
            }

            return logger;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.Left:
                    moveFrame(-1);
                    break;
                case Key.Right:
                    moveFrame(1);
                    break;
            }
        }

        private void openCompetitionModal_Click(object sender, RoutedEventArgs e)
        {
            buttonStartRecord.Focus();
            CompetitionNameModal competitionNameModal = new CompetitionNameModal(this, settings);
            childWindows.Add(competitionNameModal);
            competitionNameModal.Show();
        }

        public void updateCompetitionName()
        {
            textCompetitionName.Text = settings.competitionName;
        }

        public void setupCamera(VideoSourceSettings? videoSourceSettings)
        {
            try
            {
                if (videoSourceSettings == null)
                {
                    MainWindow.GetLogger().Log(LogLevel.Warning, "Camera setup skipped because no video source settings were provided.");
                    return;
                }

                if (videoSource != null)
                {
                    videoSource.close();
                    videoSource = null;
                    Thread.Sleep(1000);
                }

                switch (videoSourceSettings.videoSourceType)
                {
                    case VideoSourceType.Webcam:
                        MainWindow.GetLogger().Log(LogLevel.Debug, "Creating new webcam video source");
                        videoSource = new WebcamSource(this, videoSourceSettings.deviceAddress, videoSourceSettings.framerate);
                        videoSource.preview();
                        updateVideoStatus(VideoStatusLive);
                        break;
                    case VideoSourceType.SimpleHttpCamera:
                        MainWindow.GetLogger().Log(LogLevel.Debug, "Creating new simple http camera video source");
                        videoSource = new SimpleHttpVideoSource(this, videoSourceSettings.hostname, videoSourceSettings.framerate);
                        videoSource.preview();
                        updateVideoStatus(VideoStatusLive);
                        break;
                    default:
                        throw new ArgumentException();
                        break;
                }
                MainWindow.GetLogger().Log(LogLevel.Debug, "New video source: " + videoSource);
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Error during Camera Setup: {ex.ToString()}");
            }
        }

        private void comboActiveVideoSource_Selected(object sender, SelectionChangedEventArgs e)
        {
            if(comboActiveVideoSource == null)
            {
                MainWindow.GetLogger().Log(LogLevel.Debug, "comboActiveVideoSource is null ");
                return;
            }
            if (comboActiveVideoSource.SelectedItem == null)
            {
                MainWindow.GetLogger().Log(LogLevel.Debug, "comboActiveVideoSource.SelectedItem is null ");
                return;
            }

            string slectedVideoSourceName = comboActiveVideoSource.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(slectedVideoSourceName))
            {
                setupCamera(settings.videoSources.Find(x => x.name.Equals(slectedVideoSourceName)));
            }
        }

        private void openVideoFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", settings.storageFolderPath);
        }

        private async void openLoadVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = settings.storageFolderPath;
            openFileDialog.Title = "Select Video";
            openFileDialog.Filter = "Videos (*.avi)|*.avi";

            if (openFileDialog.ShowDialog() == true && openFileDialog.FileName.EndsWith(".avi", StringComparison.OrdinalIgnoreCase))
            {
                string filepath = openFileDialog.FileName;

                string previousStatus = textVideoStatus?.Text ?? (videoSource != null ? VideoStatusLive : VideoStatusNoSource);
                updateVideoStatus(VideoStatusLoading);
                updateLoadingVideoOverlay(true);

                try
                {
                    List<Bitmap> videoFrameBitmampList = await Task.Run(() => videoFileHandler.loadVideo(filepath));
                    List<BitmapImage> loadedVideo = videoFrameBitmampList.Select(x => BitmapConversions.bitmap2BitmapImage(x)).ToList();

                    if (loadedVideo != null && loadedVideo.Count > 0)
                    {
                        recordedVideo = loadedVideo;
                        frameCounter = recordedVideo.Count - 1;
                        updateFrameProgress();
                        updateVideoStatus(VideoStatusFile);
                        image.Source = recordedVideo[frameCounter];

                        comboActiveVideoSource.IsEnabled = false;
                        comboRecordingTimer.IsEnabled = false;

                        videoSource?.stopRecording();
                        buttonBack.IsEnabled = true;
                        buttonForward.IsEnabled = true;
                        buttonStopRecord.IsEnabled = false;
                        buttonStartRecord.IsEnabled = videoSource != null;
                        buttonStartRecord.IsDefault = true;
                        buttonClear.IsEnabled = true;
                        editCompetitioNameButton.IsEnabled = true;
                    }
                    else
                    {
                        MainWindow.GetLogger().Log(LogLevel.Warning, $"Video file did not contain readable frames: {filepath}");
                        updateVideoStatus(previousStatus);
                    }
                }
                finally
                {
                    updateLoadingVideoOverlay(false);
                }
            }
        }
    }
}
