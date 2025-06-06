using System;
using SharpAvi.Codecs;
using SharpAvi.Output;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using video_takeoff_control.logging;
using video_takeoff_control.settings;
using OpenCvSharp;

namespace video_takeoff_control.video_file_handler
{
    internal class AviFileHandler : IVideoFileHandler
    {
        private Settings settings;

        public AviFileHandler(Settings settings)
        {
            this.settings = settings;
        }

        public void saveVideo(string filename, List<Bitmap> frames, int framerate)
        {
            try
            {
                string storageFolder = Path.Combine(settings.storageFolderPath, settings.competitionName);
                if(!Directory.Exists(storageFolder))
                {
                    Directory.CreateDirectory(storageFolder);
                }

                MainWindow.GetLogger().Log(logging.LogLevel.Debug, "Framerate: " + framerate);

                AviWriter aviWriter = new AviWriter(filename)
                {
                    FramesPerSecond = framerate,
                    EmitIndex1 = true,
                };

                int width = frames[0].Width;
                int height = frames[0].Height;

                IAviVideoStream videoStream = aviWriter.AddMJpegWpfVideoStream(width, height);

                foreach (Bitmap frame in frames)
                {
                    videoStream.WriteFrame(true, BitmapConversions.bitmapToByte(frame, width, height));
                }

                aviWriter.Close();
                settings.attemptNumber++;
                MainWindow.GetLogger().Log(logging.LogLevel.Information, "Video written");
            }
            catch (Exception e)
            {
                MainWindow.GetLogger().LogException(e);
            }         
        }

        public List<Bitmap> loadVideo(string filePath)
        {
            MainWindow.GetLogger().Log(logging.LogLevel.Debug, "Start loading video");
            List<Bitmap> bitmaps = new List<Bitmap>();
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException();
                }

                using VideoCapture capture = new VideoCapture(filePath);
                if (!capture.IsOpened())
                {
                    throw new Exception("Unable to open video file.");
                }

                using Mat mat = new Mat();
                while (capture.Read(mat))
                {
                    if (mat.Empty())
                        break;

                    Bitmap bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat.Clone());
                    bitmaps.Add(bmp);
                }
                MainWindow.GetLogger().Log(logging.LogLevel.Information, "Video read");
                return bitmaps;
            }
            catch (Exception e)
            {
                MainWindow.GetLogger().LogException(e);
                return bitmaps;
            }
        }
    }
}
