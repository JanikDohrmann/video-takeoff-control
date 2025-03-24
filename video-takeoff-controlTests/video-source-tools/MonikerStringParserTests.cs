using Microsoft.VisualStudio.TestTools.UnitTesting;
using video_takeoff_control.video_source_tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_takeoff_control.video_source_tools.Tests
{
    [TestClass()]
    public class MonikerStringParserTests
    {
        [TestMethod()]
        public void parseDevicePathTest()
        {
            Assert.AreEqual("usb#vid_1bcf&pid_28c4&mi_00#7&1ad1b98&0&0000", MonikerStringParser.parseDevicePath(@"@device:pnp:\\?\usb#vid_1bcf&pid_28c4&mi_00#7&1ad1b98&0&0000#{65e8773d-8f56-11d0-a3b9-00a0c9223196}\global"));
            Assert.AreEqual(@"@device:sw:{860BB310-5D01-11D0-BD3B-00A0C911CE86}\{A3FCE0F5-3493-419F-958A-ABA1250EC20B}", MonikerStringParser.parseDevicePath(@"@device:sw:{860BB310-5D01-11D0-BD3B-00A0C911CE86}\{A3FCE0F5-3493-419F-958A-ABA1250EC20B}"));
        }
    }
}