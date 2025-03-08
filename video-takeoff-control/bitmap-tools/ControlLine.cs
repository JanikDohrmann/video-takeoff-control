using System.Drawing;

namespace video_takeoff_control
{
    internal class ControlLine
    {
        public static void drawControlLine(Bitmap bitmap, Settings settings)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Pen pen = new Pen(settings.controlLineColor, settings.controlLineWidth);

                if(settings.centerControlLine)
                {
                    int x = (bitmap.Width / 2) - (settings.controlLineWidth / 2);
                    int y = (bitmap.Height / 2) - (settings.controlLineWidth / 2);

                    if (settings.showVerticalControlLine)
                    {
                        graphics.DrawLine(pen, new Point(x, 0), new Point(x, bitmap.Height));
                    }
                    if (settings.showHorizontalControlLine)
                    {
                        graphics.DrawLine(pen, new Point(0, y), new Point(bitmap.Width, y));
                    }
                }
                else
                {
                    int x = settings.controlLineX <= bitmap.Width ? settings.controlLineX : bitmap.Width;
                    x = x >= 0 ? settings.controlLineX : 0;

                    int y = settings.controlLineY <= bitmap.Height ? settings.controlLineY : bitmap.Height;
                    y = y >= 0 ? settings.controlLineY : 0;

                    if (settings.showVerticalControlLine)
                    {
                        graphics.DrawLine(pen, new Point(x, 0), new Point(x, bitmap.Height));
                    }
                    if(settings.showHorizontalControlLine)
                    {
                        graphics.DrawLine(pen, new Point(0, y), new Point(bitmap.Width, y));
                    }
                }
            }
        }
    }
}
