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
        
        private VideoCaptureDevice videoSource;

        private int framerate;

        public WebcamSource(MainWindow mainWindow, string deviceAddress, int framerate)
        {
            this.mainWindow = mainWindow;
            this.framerate = framerate;
            videoSource = new VideoCaptureDevice(deviceAddress);
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

        public int getFramerate()
        {
            return framerate;
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
