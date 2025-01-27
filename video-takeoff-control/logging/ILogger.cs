using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_takeoff_control.logging
{
    /// <summary>
    /// Represents a logging interface to log messages and exceptions.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message with the specified log level.
        /// </summary>
        /// <param name="logLevel">The severity level of the log.</param>
        /// <param name="message">The message to be logged.</param>
        /// <remarks>
        /// The <paramref name="logLevel"/> can be used to categorize logs, 
        /// such as Info, Warning, or Error. Implementers should ensure that
        /// the message is recorded according to the specified severity.
        /// </remarks>
        void Log(LogLevel logLevel, string message);

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        /// <remarks>
        /// This method is used to capture and log exception details, 
        /// including the message, stack trace, and any inner exceptions.
        /// </remarks>
        void LogException(Exception exception);
    }
}
