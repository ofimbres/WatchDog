using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WatchDog.WpfApp.Helpers
{
    public static class ImageEncodingHelper
    {
        public static MemoryStream GetJpegStreamFromWriteableBitmap(WriteableBitmap bitmapSource)
        {
            // create a png bitmap encoder which knows how to save a .png file
            BitmapEncoder encoder = new GifBitmapEncoder();

            // create frame from the writable bitmap and add to encoder
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            MemoryStream fs = new MemoryStream();
                encoder.Save(fs);
                fs.Position = 0;

                return fs;
        }

        public static MemoryStream GetGifStreamFromWriteableBitmap(WriteableBitmap bitmapSource)
        {
            // create a png bitmap encoder which knows how to save a .png file
            BitmapEncoder encoder = new GifBitmapEncoder();

            // create frame from the writable bitmap and add to encoder
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (MemoryStream fs = new MemoryStream())
            {
                encoder.Save(fs);
                fs.Position = 0;

                return fs;
            }
        }
    }
}
