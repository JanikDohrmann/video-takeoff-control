using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using video_takeoff_control.logging;

namespace video_takeoff_control.video_source
{
    internal class SimpleHttpVideoSource : IVideoSource
    {

        private MainWindow mainWindow;

        private HttpClient httpClient;

        private string videoSourceUrl;

        private Timer timer;
        private Task task;

        private bool running = false;

        public SimpleHttpVideoSource(MainWindow mainWindow, string camId)
        {
            this.mainWindow = mainWindow;
            httpClient = new HttpClient();
            this.videoSourceUrl = Settings.httpVideoSourceURL[camId];
            
        }
        public void close()
        {
            //timer?.Dispose();
            task?.Dispose();
            httpClient?.Dispose();
        }

        public void preview()
        {
            task = new Task(video_NewFrame);
            running = true;
            task.Start();
        }

        public void startRecording()
        {
            
        }

        public void stopRecording()
        {
            running = false;
            task?.Dispose();
        }

        private async void video_NewFrame()
        {
            try
            {
                while(running)
                {
                    // JPEG-Bild von der URL abrufen
                    var imageData = await httpClient.GetByteArrayAsync(videoSourceUrl);

                    // In einen MemoryStream konvertieren
                    using (var stream = new MemoryStream(imageData))
                    {
                        // Bitmap aus dem Stream erstellen
                        using (var bitmap = new Bitmap(stream))
                        {
                            mainWindow.newFrame(bitmap);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.GetLogger().Log(LogLevel.Error, $"Fehler beim Abrufen des Bildes: {ex.ToString()}");
            }
        }
    }
}
