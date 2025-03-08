using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using video_takeoff_control.video_source;

namespace video_takeoff_control.settings
{
    public class VideoSourceSettings
    {
        public string name { get; set; }
        public VideoSourceType selectedVideoSourceType { get; set; }
        public string hostname { get; set; }
    }
}
