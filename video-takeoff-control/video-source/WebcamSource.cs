using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using video_takeoff_control.logging;

namespace video_takeoff_control.video_source
{
    internal class WebcamSource : IVideoSource
    {
        private MainWindow mainWindow;
        
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        public WebcamSource(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
        }

        public void close()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.NewFrame -= new NewFrameEventHandler(video_NewFrame);
                videoSource = null;
            }
        }

        public void preview()
        {
            videoSource.Start();
        }

        public void startRecording()
        {

        }

        public void stopRecording()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {      
            using (Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone())
            {
                mainWindow.newFrame(bitmap);
            }
        }
    }
}
