using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_takeoff_control.logging
{
    public interface ILogger
    {
        void Log(LogLevel logLevel, string message);
    }
}
