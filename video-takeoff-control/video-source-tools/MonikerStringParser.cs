using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_takeoff_control.video_source_tools
{
    public class MonikerStringParser
    {
        public static string parseDevicePath(string monikerString)
        {
            if(monikerString.IndexOf(@"?\") >= 0 && monikerString.LastIndexOf("#") >= 0) {
                return monikerString.Substring(monikerString.IndexOf(@"?\") + 2, monikerString.LastIndexOf("#") - (monikerString.IndexOf(@"?\") + 2));
            }
            else if(monikerString.IndexOf(@"?\") >= 0)
            {
                return monikerString.Substring(monikerString.IndexOf(@"?\") + 2);
            }
            else
            {
                return monikerString;
            }
        }
    }
}
