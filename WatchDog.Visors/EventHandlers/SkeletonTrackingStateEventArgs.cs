using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchDog.Visors.EventHandlers
{
    public class SkeletonTrackingStateEventArgs : EventArgs
    {
        public SkeletonTrackingStateEventArgs(int skeletonCount)
        {
            this.SkeletonCount = skeletonCount;
        }

        public int SkeletonCount { get; set; }
        public bool SkeletonDetected
        {
            get { return SkeletonCount != 0; }
        }
    }
}
