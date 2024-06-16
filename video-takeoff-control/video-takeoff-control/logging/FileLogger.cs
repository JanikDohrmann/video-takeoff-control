using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_takeoff_control.logging
{
    internal class FileLogger : ILogger
    {
        private string logPath;

        private FileLogger()
        {

        }

        private FileLogger(string logPath)
        {
            this.logPath = logPath;

            if(!string.IsNullOrEmpty(logPath) && !Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }

        public void Log(LogLevel logLevel, string message)
        {
            string filename = string.Format("log_video-takeoff-control_{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
            string filepath = Path.Combine(logPath, filename);

            File.AppendAllText(filepath, string.Format("{0}\t{1}\t{2}\n", DateTime.Now, logLevel, message));
        }

        public static FileLogger? create(string logPath)
        {
            if (string.IsNullOrEmpty(logPath))
            {
                logPath = "\\logs\\";
            }

            if(!logPath.EndsWith("\\"))
            {
                logPath = logPath + "\\";
            }

            return new FileLogger(logPath);
        }
    }
}
