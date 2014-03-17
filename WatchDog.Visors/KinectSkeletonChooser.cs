namespace WatchDog.Visors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Microsoft.Kinect;
   

    /// <summary>
    /// KinectSkeletonChooser class is a lookless control that will select skeletons based on a specified heuristic.
    /// It contains logic to track players over multiple frames and use this data to select based on distance, activity level or other methods.
    /// It is intended that if you use this control, no other code will manage the skeleton tracking state on the Kinect Sensor,
    /// as they will collide in unpredictable ways.
    /// </summary>
    public class KinectSkeletonChooser : KinectControl
    {
        private Skeleton[] skeletonData;


        protected override void OnKinectSensorChanged(object sender, KinectSensorManagerEventArgs<KinectSensor> args)
        {
            if (null == args)
            {
                throw new ArgumentNullException("args");
            }

            var oldKinectSensor = args.OldValue;
            var newKinectSensor = args.NewValue;

            if (oldKinectSensor != null)
            {
                oldKinectSensor.SkeletonFrameReady -= this.SkeletonFrameReady;
            }

            if (newKinectSensor != null && newKinectSensor.Status == KinectStatus.Connected)
            {
                newKinectSensor.SkeletonFrameReady += this.SkeletonFrameReady;
            }

            this.EnsureSkeletonStreamState();
        }

        private void EnsureSkeletonStreamState()
        {
            if ((null != this.KinectSensorManager) && (null != this.KinectSensorManager.KinectSensor))
            {
                this.KinectSensorManager.KinectSensor.SkeletonStream.AppChoosesSkeletons = false; // default is false
            }
        }

        private void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Ensure that this event still corresponds to the current Kinect Sensor (if any)
            if ((null == this.KinectSensorManager) || (!object.ReferenceEquals(this.KinectSensorManager.KinectSensor, sender)))
            {
                return;
            }

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((this.skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                }
            }
        }
    }
}
