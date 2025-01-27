using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_takeoff_control.video_source
{
    internal interface IVideoSource
    {
        void startRecording();
        void stopRecording();
        void preview();
        void close();
    }
}
