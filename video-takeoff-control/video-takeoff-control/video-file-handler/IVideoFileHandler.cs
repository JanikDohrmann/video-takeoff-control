using System.Collections.Generic;
using System.Drawing;

namespace video_takeoff_control.video_file_handler
{
    internal interface IVideoFileHandler
    {
        void saveVideo(string filePath, List<Bitmap> frames);
    }
}
