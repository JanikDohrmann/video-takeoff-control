using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_takeoff_control.logging
{
    /// <summary>
    /// Provides file-based logging functionality.
    /// Implements the <see cref="ILogger"/> interface to log messages and exceptions to a file.
    /// </summary>
    internal class FileLogger : ILogger
    {
        /// <summary>
        /// The path where log files are stored.
        /// </summary>
        private string logPath;

        /// <summary>
        /// Prevents direct instantiation of the <see cref="FileLogger"/> class.
        /// Use the <see cref="create"/> method to create an instance.
        /// </summary>
        private FileLogger()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class with the specified log path.
        /// </summary>
        /// <param name="logPath">The directory path where log files will be stored.</param>
        /// <remarks>
        /// If the specified <paramref name="logPath"/> does not exist, it will be created.
        /// </remarks>
        private FileLogger(string logPath)
        {
            this.logPath = logPath;

            if(!string.IsNullOrEmpty(logPath) && !Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Logs a message with the specified log level to a file.
        /// </summary>
        /// <param name="logLevel">The severity level of the log.</param>
        /// <param name="message">The message to be logged.</param>
        /// <remarks>
        /// Logs are written to a file named in the format "log_video-takeoff-control_yyyy-MM-dd.log"
        /// located in the <see cref="logPath"/> directory.
        /// Each log entry includes the timestamp, log level, and message.
        /// </remarks>
        public void Log(LogLevel logLevel, string message)
        {
            string filename = string.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
            string filepath = Path.Combine(logPath, filename);

            File.AppendAllText(filepath, string.Format("{0}\t{1}\t{2}\n", DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"), logLevel, message));
        }

        /// <inheritdoc />
        /// <summary>
        /// Logs an exception as an error to the log file.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        /// <remarks>
        /// This method delegates to <see cref="Log(LogLevel, string)"/> with a log level of <see cref="LogLevel.Error"/>.
        /// </remarks>
        public void LogException(Exception exception)
        {
            Log(LogLevel.Error, exception.ToString());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="logPath">The directory path where log files will be stored.</param>
        /// <returns>
        /// A new <see cref="FileLogger"/> instance if <paramref name="logPath"/> is valid; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// If <paramref name="logPath"/> is not provided or is empty, the default path "\\logs\\" is used.
        /// Trailing backslashes in the path are added automatically if missing.
        /// </remarks>
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
