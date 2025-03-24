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
        public VideoSourceType videoSourceType { get; set; }
        public string hostname { get; set; }
        public int framerate { get; set; }
        public string deviceAddress { get; set; }

        public VideoSourceSettings()
        {
        }

        public VideoSourceSettings(string name, VideoSourceType videoSourceType, string hostname, int framerate)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.videoSourceType = videoSourceType;
            this.hostname = hostname ?? throw new ArgumentNullException(nameof(hostname));
            this.framerate = framerate;
        }

        public VideoSourceSettings(string name, VideoSourceType videoSourceType, int framerate, string deviceAddress)
        {
            this.name = name;
            this.videoSourceType = videoSourceType;
            this.framerate = framerate;
            this.deviceAddress = deviceAddress;
        }

        public VideoSourceSettings(string name, VideoSourceType videoSourceType, string hostname, int framerate, string deviceAddress) : this(name, videoSourceType, hostname, framerate)
        {
            this.deviceAddress = deviceAddress;
        }
    }
}
