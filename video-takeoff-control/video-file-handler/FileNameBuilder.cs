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
        public static string buildFileName(string folderpath, string competitionName, int attemptNumber)
        {
            string filename = Path.Combine(folderpath, competitionName, string.Format("{0}_Video_{1}_{2}.avi", attemptNumber, competitionName.Replace(" ", "-"), DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HHmmss")));

            return filename;
        }
    }
}
