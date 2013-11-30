using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Storage.Streams;

namespace Direct3DUtils
{
    public static class WritableBitmapBinSave
    {
        public static async Task<WriteableBitmap> ReadBinAsync(Stream stream)
        {
            try
            {
                var reader = new BinaryReader(stream);
                int h = 0;
                int w = 0;
                await Task.Run(() =>
                {
                    w = reader.ReadInt32();
                    h = reader.ReadInt32();
                });
                WriteableBitmap bmp = null;
                int[] pix = null;
                bmp = new WriteableBitmap(w, h);
                pix = bmp.Pixels;
                await Task.Run(() =>
                {
                    for (int i = 0; i < w * h; i++)
                    {
                        pix[i] = reader.ReadInt32();
                    }
                });
                bmp.Pixels[0] = bmp.Pixels[0];
                return bmp;
            }
            catch
            {
                return null;
            }
        }
        public static Task WriteBinAsync(this WriteableBitmap bmp, Stream stream)
        {        
            Int32 h = bmp.PixelHeight;
            Int32 w = bmp.PixelWidth;
            var pix = bmp.Pixels;
            return Task.Run(() =>
            {
                try
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write(w);
                    writer.Write(h);
                    for (int i = 0; i < w * h; i++)
                    {
                        writer.Write(pix[i]);
                    }
                }
                catch
                { }
            });
        }
        public static async Task<WriteableBitmap> ReadBinAsync(IInputStream stream)
        {
            try
            {
                var reader = new DataReader(stream);
                await reader.LoadAsync(2 * 4);
                int h = 0;
                int w = 0;
                w = reader.ReadInt32();
                h = reader.ReadInt32();
                WriteableBitmap bmp = new WriteableBitmap(w, h);
                var pix = bmp.Pixels;
                await reader.LoadAsync((uint)(w * h * 4));
                for (int i = 0; i < w * h; i++)
                {
                    pix[i] = reader.ReadInt32();
                }
                bmp.Pixels[0] = bmp.Pixels[0];
                return bmp;

            }
            catch
            {
                return null;
            }
        }
        public static async Task WriteBinAsync(this WriteableBitmap bmp, IOutputStream stream)
        {

            try
            {
                int h = bmp.PixelHeight;
                int w = bmp.PixelWidth;
                var pix = bmp.Pixels;

                var writer = new DataWriter(stream);

                writer.WriteInt32(w);
                writer.WriteInt32(h);

                for (int i = 0; i < w * h; i++)
                {
                    writer.WriteInt32(pix[i]);
                }

                await writer.StoreAsync();


            }
            catch
            { }
        }
    }
}
