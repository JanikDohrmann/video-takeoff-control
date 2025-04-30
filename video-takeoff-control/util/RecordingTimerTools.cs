using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using video_takeoff_control.logging;

namespace video_takeoff_control.util
{
    public class RecordingTimerTools
    {
        public static int convertRecordingTimerSelection(string recordingTimerSelection)
        {
            if(!string.IsNullOrEmpty(recordingTimerSelection) && Regex.IsMatch(recordingTimerSelection, @"\d+s"))
            {
                return int.Parse(recordingTimerSelection.Replace("s", "").Trim()) * 1000;
            }
            else
            {
                return 0;
            }
        }
    }
}
