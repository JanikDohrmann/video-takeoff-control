using SharpAvi.Codecs;
using SharpAvi.Output;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace video_takeoff_control.video_file_handler
{
    internal class AviFileHandler : IVideoFileHandler
    {
        public void saveVideo(string filePath, List<Bitmap> frames)
        {
            AviWriter aviWriter = new AviWriter("test.avi")
            {
                FramesPerSecond = 10,
                EmitIndex1 = true,
            };

            IAviVideoStream videoStream = aviWriter.AddMpeg4VcmVideoStream(400, 400, 10);

            /*foreach (BitmapImage frame in frames)
            {
                videoStream.WriteFrame(true, ConvertBitmapToReadOnlySpan(BitmapImage2Bitmap(frame)));
            }*/

            aviWriter.Close();
        }
    }
}
