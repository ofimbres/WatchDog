using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace WatchDog.Visors
{
    public class OutputImageEventArgs : EventArgs
    {
        public OutputImageEventArgs(WriteableBitmap frame)
        {
            this.Frame = frame;
        }

        public WriteableBitmap Frame { get; private set; }
    }
}
