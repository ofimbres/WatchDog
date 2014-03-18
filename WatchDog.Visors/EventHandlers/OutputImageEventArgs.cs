using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace WatchDog.Visors
{
    public class OutputImageEventArgs : EventArgs
    {
        public OutputImageEventArgs(byte[] rawPhotoData, WriteableBitmap frame)
        {
            this.RawPhotoData = rawPhotoData;
            this.Frame = frame;
        }

        public byte[] RawPhotoData { get; private set; }
        public WriteableBitmap Frame { get; private set; }
    }
}
