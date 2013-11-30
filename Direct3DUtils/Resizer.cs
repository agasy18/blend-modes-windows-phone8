using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Direct3DUtilsComp;

namespace Direct3DUtils
{
    public class Resizer
    {
        public Resizer()
        {

        }

        public static Size NewSize(int wd, int ht)
        {
            const int maxImageSize = 2048;
            int width, height;
            double w = wd / maxImageSize;
            double h = ht / maxImageSize;
            if (w > h)
            {
                double tokos = ht * 100 / wd;
                width = maxImageSize;
                height = (int)(width * tokos) / 100;
            }
            else
            {
                double tokos = wd * 100 / ht;
                height = maxImageSize;
                width = (int)(height * tokos) / 100;
            }


            Size newSize = new Size(width, height);
            return newSize;
        }

        public static async Task<WriteableBitmap> NewBitmap(BitmapSource originalBitmap, Size newSize)
        {
            WriteableBitmap originalWb = originalBitmap as WriteableBitmap ?? new WriteableBitmap(originalBitmap);
            int pw = originalWb.PixelWidth;
            int ph = originalWb.PixelHeight;
            WriteableBitmap newBitmap = new WriteableBitmap((int)newSize.Width, (int)newSize.Height);
            int[] destArray = newBitmap.Pixels;
            int[] originalArray = originalWb.Pixels;
            HResizeLinear hr = new HResizeLinear();
            await Task.Run(() =>
            {
                hr.resize(out originalArray[0], out destArray[0], pw, ph, (int)newSize.Width, (int)newSize.Height);
            });
            newBitmap.Pixels[0] = destArray[0];
            return newBitmap;

        }

    }
}
