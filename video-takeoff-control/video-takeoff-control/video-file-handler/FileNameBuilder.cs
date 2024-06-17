using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_takeoff_control.video_file_handler
{
    public class FileNameBuilder
    {
        public static string buildFileName(string folderpath, string competitionName)
        {
            string filename = Path.Combine(folderpath, string.Format("Video_{0}_{1}.avi", competitionName.Replace(" ", "-"), DateTime.Now.ToString("yyyy'-'MM'-'dd'T'mmhhss")));

            return filename;
        }
    }
}
