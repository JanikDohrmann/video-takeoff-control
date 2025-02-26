﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using video_takeoff_control.bitmap_tools;
using video_takeoff_control.logging;
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

        private List<BitmapImage> recordedVideo;
        private int frameCounter;

        private bool recording;

        private IVideoSource videoSource;
        private IVideoFileHandler videoFileHandler;

        private static ILogger logger;

        public MainWindow()
        {
            string commonApplicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string logpath = Path.Combine(commonApplicationDataFolder, "videoo-takeoff-control\\logs\\");
            logger = FileLogger.create(logpath);

            logger.Log(LogLevel.Information, "Starting!");

            try
            {
                Settings.initializeSettings();

                InitializeComponent();
                recordedVideo = new List<BitmapImage>();
                frameCounter = 0;
                recording = false;

                setupCamera(Settings.selectedVideoSourceType);
                MainWindow.GetLogger().Log(LogLevel.Information, "Videosource created!");

                videoFileHandler = new AviFileHandler();

                buttonBack.IsEnabled = false;
                buttonForward.IsEnabled = false;
                buttonStopRecord.IsEnabled = false;
                buttonClear.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im MainWindow: {ex.ToString()}");
            }
            
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            recordedVideo = new List<BitmapImage>();
            frameCounter = 0;
            recording = true;
            buttonStopRecord.IsEnabled = true;
            buttonStopRecord.IsDefault = true;
            buttonStartRecord.IsEnabled = false;
            editCompetitioNameButton.IsEnabled = false;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            recording = false;

            videoSource.stopRecording();
            frameCounter = recordedVideo.Count - 1;
            updateFrameProgress();
            buttonBack.IsEnabled = true;
            buttonForward.IsEnabled = true;
            buttonStopRecord.IsEnabled = false;
            buttonClear.IsEnabled = true;
            buttonClear.IsDefault = true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {   
            try
            {
                Task.Run(() => videoFileHandler.saveVideo(FileNameBuilder.buildFileName(Settings.storageFolderPath, Settings.competitionName), recordedVideo.Select(x => BitmapConversions.bitmapImage2Bitmap(x)).ToList()));
                resetFrameProgress();
                videoSource.preview();

                buttonBack.IsEnabled = false;
                buttonForward.IsEnabled = false;
                buttonStopRecord.IsEnabled = false;
                buttonClear.IsEnabled = false;
                buttonStartRecord.IsEnabled = true;
                buttonStartRecord.IsDefault = true;
                editCompetitioNameButton.IsEnabled = true;
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
            int maxFrames = recordedVideo.Count - 1;

            double progress = ((double)frameCounter / maxFrames) * 100;

            framePogress.Value = progress;
        }

        private void resetFrameProgress()
        {
            framePogress.Value = 0;
        }

        protected override void OnClosed(EventArgs e)
        {
            videoSource.close();

            foreach(Window window in childWindows)
            {
                window.Close();
            }

            logger.Log(LogLevel.Information, "Shutting down!");

            base.OnClosed(e);
        }

        private void openOptionsMenu_Click(object sender, RoutedEventArgs e)
        {
            OptionsMenuWindow optionsMenuWindow = new OptionsMenuWindow(this);
            childWindows.Add(optionsMenuWindow);
            optionsMenuWindow.Show();
        }

        public void newFrame(Bitmap frame)
        {
            ControlLine.drawControlLine(frame);
            BitmapTools.addMetadata(frame);
            BitmapImage bitmapImage = BitmapConversions.bitmap2BitmapImage(frame);

            if (recording)
            {
                recordedVideo.Add(bitmapImage);
            }

            Dispatcher.BeginInvoke(new Action(() => image.Source = bitmapImage));
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

            int newFrameCounter = frameCounter;

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
            CompetitionNameModal competitionNameModal = new CompetitionNameModal();
            childWindows.Add(competitionNameModal);
            competitionNameModal.mainWindow = this;
            competitionNameModal.Show();
        }

        public void updateCompetitionName()
        {
            textCompetitionName.Text = Settings.competitionName;
        }

        public void setupCamera(VideoSourceType type)
        {
            try
            {
                if (videoSource != null)
                {
                    videoSource.close();
                }

                switch (type)
                {
                    case VideoSourceType.Webcam:
                        videoSource = new WebcamSource(this);
                        videoSource.preview();
                        break;
                    case VideoSourceType.SimpleHttpCamera:
                        videoSource = new SimpleHttpVideoSource(this, "cam1");
                        videoSource.preview();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler im Camera Setup: {ex.ToString()}");
            }
        }
    }
}
