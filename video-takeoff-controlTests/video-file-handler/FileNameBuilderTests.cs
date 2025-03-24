using Microsoft.VisualStudio.TestTools.UnitTesting;
using video_takeoff_control.video_file_handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace video_takeoff_control.video_file_handler.Tests
{
    [TestClass()]
    public class FileNameBuilderTests
    {
        [TestMethod()]
        public void buildFileNameTest()
        {
            string actualFileName = FileNameBuilder.buildFileName("", "Weisprung Männer");
            Assert.IsTrue(Regex.IsMatch(actualFileName, @"Video_[äöüa-zA-Z-]+_[T\d-]+.avi"));
        }
    }
}