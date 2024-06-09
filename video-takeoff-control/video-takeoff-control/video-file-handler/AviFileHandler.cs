using System;
using SharpAvi.Codecs;
using SharpAvi.Output;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace video_takeoff_control.video_file_handler
{
    internal class AviFileHandler : IVideoFileHandler
    {
        public void saveVideo(string filename, List<Bitmap> frames)
        {
            try
            {
                 AviWriter aviWriter = new AviWriter(filename)
                {
                    FramesPerSecond = Settings.framerate,
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
            }
            catch (Exception e)
            {
                File.WriteAllText("Log.txt", e.ToString());
            }         
        }
    }
}
