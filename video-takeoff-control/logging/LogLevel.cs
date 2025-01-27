using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_takeoff_control.logging
{
    /// <summary>
    /// Specifies the severity level of a log message.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug-level messages, typically used for troubleshooting and development purposes.
        /// </summary>
        Debug,

        /// <summary>
        /// Informational messages that represent the normal flow of an application.
        /// </summary>
        Information,

        /// <summary>
        /// Warning messages that indicate a potential issue or unexpected situation.
        /// </summary>
        Warning,

        /// <summary>
        /// Error messages that indicate a failure in the current operation or request.
        /// </summary>
        Error,

        /// <summary>
        /// Fatal messages that indicate a severe failure causing the application to terminate.
        /// </summary>
        Fatal
    }
}
