using System.Drawing;

namespace video_takeoff_control
{
    internal class ControlLine
    {
        public static void drawControlLine(Bitmap bitmap)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Pen pen = new Pen(Settings.controlLineColor, Settings.controlLineWidth);
                int x = Settings.controlLineX <= bitmap.Width ? Settings.controlLineX : bitmap.Width;
                x = x >= 0 ? Settings.controlLineX : 0;
                
                if(Settings.showControlLine)
                {
                    graphics.DrawLine(pen, new Point(x, 0), new Point(x, bitmap.Height));
                }
            }
        }
    }
}
