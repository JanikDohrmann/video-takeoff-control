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

                if(Settings.centerControlLine)
                {
                    int x = (bitmap.Width / 2) - (Settings.controlLineWidth / 2);
                    int y = (bitmap.Height / 2) - (Settings.controlLineWidth / 2);

                    if (Settings.showVerticalControlLine)
                    {
                        graphics.DrawLine(pen, new Point(x, 0), new Point(x, bitmap.Height));
                    }
                    if (Settings.showHorizontalControlLine)
                    {
                        graphics.DrawLine(pen, new Point(0, y), new Point(bitmap.Width, y));
                    }
                }
                else
                {
                    int x = Settings.controlLineX <= bitmap.Width ? Settings.controlLineX : bitmap.Width;
                    x = x >= 0 ? Settings.controlLineX : 0;

                    int y = Settings.controlLineY <= bitmap.Height ? Settings.controlLineY : bitmap.Height;
                    y = y >= 0 ? Settings.controlLineY : 0;

                    if (Settings.showVerticalControlLine)
                    {
                        graphics.DrawLine(pen, new Point(x, 0), new Point(x, bitmap.Height));
                    }
                    if(Settings.showHorizontalControlLine)
                    {
                        graphics.DrawLine(pen, new Point(0, y), new Point(bitmap.Width, y));
                    }
                }
            }
        }
    }
}
