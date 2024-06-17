using System;
using video_takeoff_control.video_file_handler;
using Xunit;

namespace video_takeoff_control_test
{
    public class FileNameBuilderTest
    {
        [Fact]
        public void buildFileNameTest()
        {
            string actualFileName = FileNameBuilder.buildFileName("", "Weisprung Männer");
            Assert.Matches(@"Video_[äöüa-zA-Z-]+_[T\d-]+.avi", actualFileName);
        }
    }
}