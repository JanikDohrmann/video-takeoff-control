using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.DirectShow;

namespace video_takeoff_control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Window> childWindows = new List<Window>();

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        private List<BitmapImage> recordedVideo;
        private int frameCounter;

        private bool recording;

        public MainWindow()
        {
            Settings.controlLineX = 0;
            Settings.storageFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "video-takeoff-control");

            InitializeComponent();
            recordedVideo = new List<BitmapImage>();
            frameCounter = 0;
            recording = false;

            buttonBack.IsEnabled = false;
            buttonForward.IsEnabled = false;
            buttonStopRecord.IsEnabled = false;
            buttonClear.IsEnabled = false;

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No video sources found");
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
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

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
            }

            buttonBack.IsEnabled = true;
            buttonForward.IsEnabled = true;
            buttonStopRecord.IsEnabled = false;
            buttonClear.IsEnabled = true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            //ToDo Save Video

            videoSource.Start();

            buttonBack.IsEnabled = false;
            buttonForward.IsEnabled = false;
            buttonStopRecord.IsEnabled = false;
            buttonClear.IsEnabled = false;
            buttonStartRecord.IsEnabled = true;
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            int newFrameCounter = frameCounter--;
            Dispatcher.BeginInvoke(new Action(() => image.Source = recordedVideo[newFrameCounter]));
        }

        private void BackwardButton_Click(object sender, RoutedEventArgs e)
        {
            int newFrameCounter = frameCounter++;
            Dispatcher.BeginInvoke(new Action(() => image.Source = recordedVideo[newFrameCounter]));
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            using (Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone())
            {
                // Zeichne eine vertikale Linie in der Mitte des Bildes
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 5); // Rote Linie mit einer Breite von 5 Pixeln
                    int x = Settings.controlLineX <= bitmap.Width ? Settings.controlLineX : bitmap.Width;
                    x = x >= 0 ? Settings.controlLineX : 0;
                    graphics.DrawLine(pen, new System.Drawing.Point(x, 0), new System.Drawing.Point(x, bitmap.Height));
                }

                BitmapImage bitmapImage;
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }
                bitmapImage.Freeze();

                if (recording)
                {
                    recordedVideo.Add(bitmapImage);
                }

                Dispatcher.BeginInvoke(new Action(() => image.Source = bitmapImage));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.NewFrame -= new NewFrameEventHandler(video_NewFrame);
                videoSource = null;
            }

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
    }
}
