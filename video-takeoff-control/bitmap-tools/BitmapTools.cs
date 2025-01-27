using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_takeoff_control.bitmap_tools
{
    internal class BitmapTools
    {
        public static void addMetadata(Bitmap bitmap)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                int rectHeight = 50;

                RectangleF rectf = new RectangleF(10, bitmap.Height - rectHeight - 10, 600, rectHeight);

                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                string metadataString = String.Format("{0}", DateTime.Now.ToString());

                graphics.DrawString(metadataString, new Font("Tahoma", 28, FontStyle.Bold), Brushes.Black, rectf);
            }
        }
    }
}
