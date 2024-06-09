using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
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

        public MainWindow()
        {
            Settings.initializeSettings();

            InitializeComponent();
            recordedVideo = new List<BitmapImage>();
            frameCounter = 0;
            recording = false;

            videoSource = new WebcamSource(this);
            videoSource.preview();

            videoFileHandler = new AviFileHandler();

            buttonBack.IsEnabled = false;
            buttonForward.IsEnabled = false;
            buttonStopRecord.IsEnabled = false;
            buttonClear.IsEnabled = false;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            recordedVideo = new List<BitmapImage>();
            frameCounter = 0;
            recording = true;
            buttonStopRecord.IsEnabled = true;
            buttonStartRecord.IsEnabled = false;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            recording = false;

            videoSource.stopRecording();
            frameCounter = recordedVideo.Count - 1;

            buttonBack.IsEnabled = true;
            buttonForward.IsEnabled = true;
            buttonStopRecord.IsEnabled = false;
            buttonClear.IsEnabled = true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            string competitionName = textCompetitionName.Text;
            
            videoFileHandler.saveVideo(FileNameBuilder.buildFileName(Settings.storageFolderPath, competitionName), recordedVideo.Select(x => BitmapConversions.bitmapImage2Bitmap(x)).ToList());

            videoSource.preview();

            buttonBack.IsEnabled = false;
            buttonForward.IsEnabled = false;
            buttonStopRecord.IsEnabled = false;
            buttonClear.IsEnabled = false;
            buttonStartRecord.IsEnabled = true;
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            int newFrameCounter = frameCounter < recordedVideo.Count - 1 ? frameCounter++ : recordedVideo.Count - 1;
            Dispatcher.BeginInvoke(new Action(() => image.Source = recordedVideo[newFrameCounter]));
        }

        private void BackwardButton_Click(object sender, RoutedEventArgs e)
        {
            int newFrameCounter = frameCounter > 0 ? frameCounter-- : 0;
            Dispatcher.BeginInvoke(new Action(() => image.Source = recordedVideo[newFrameCounter]));
        }

        protected override void OnClosed(EventArgs e)
        {
            videoSource.close();

            foreach(Window window in childWindows)
            {
                window.Close();
            }

            base.OnClosed(e);
        }

        private void openOptionsMenu_Click(object sender, RoutedEventArgs e)
        {
            OptionsMenuWindow optionsMenuWindow = new OptionsMenuWindow();
            childWindows.Add(optionsMenuWindow);
            optionsMenuWindow.Show();
        }

        public void newFrame(Bitmap frame)
        {
            ControlLine.drawControlLine(frame);
            BitmapImage bitmapImage = BitmapConversions.bitmap2BitmapImage(frame);

            if (recording)
            {
                recordedVideo.Add(bitmapImage);
            }

            Dispatcher.BeginInvoke(new Action(() => image.Source = bitmapImage));
        }
    }
}
