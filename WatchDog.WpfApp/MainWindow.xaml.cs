using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WatchDog.Visors;
using WatchDog.WpfApp.Models;
using WatchDog.WpfApp.Tables;
using WatchDog.WpfApp.Utils;

namespace WatchDog.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensorManager ksm;
        private DispatcherTimer timer;
        private byte[] lastImageData;
        private WriteableBitmap lastFrame;

        //private readonly KinectWindowViewModel viewModel;
        //private KinectSensor sensor;
        //private byte[] colorPixels;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            //Closing += WindowClosing;

            ksm = new KinectSensorManager();
            ksm.KinectSensor = KinectSensor.KinectSensors.First();
            ksm.ColorFormat = ColorImageFormat.InfraredResolution640x480Fps30;
            ksm.SkeletonStreamEnabled = true;
            ksm.ColorStreamEnabled = true;

            this.DataContext = ksm;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //this.viewModel = new KinectWindowViewModel();

            //// The KinectSensorManager class is a wrapper for a KinectSensor that adds
            //// state logic and property change/binding/etc support, and is the data model
            //// for KinectDiagnosticViewer.
            //this.viewModel.KinectSensorManager = new KinectSensorManager();

            //Binding sensorBinding = new Binding("KinectSensor");
            //sensorBinding.Source = this;
            //BindingOperations.SetBinding(this.viewModel.KinectSensorManager, KinectSensorManager.KinectSensorProperty, sensorBinding);

            //// Attempt to turn on Skeleton Tracking for each Kinect Sensor
            //this.viewModel.KinectSensorManager.SkeletonStreamEnabled = true;

            //this.DataContext = this.viewModel;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10d);
            timer.Tick += TimerTick;
            timer.Start();
        }


        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        // http://stackoverflow.com/a/8067336/1118485
        private void ColorViewer_OutputImageChanged(object sender, OutputImageEventArgs e)
        {
            lastImageData = e.RawPhotoData;
            lastFrame = e.Frame;
            Debug.WriteLine("frame updated");
        }

        // http://stackoverflow.com/a/11720080/1118485
        private void TimerTick(object sender, EventArgs e)
        {
            CamAudit cam = new CamAudit(DateTime.Now, "Kinect Device 1", (byte[])lastImageData.Clone());
            //PhotoCreatorUtil.CreatePhoto(lastFrame);
            PhotoCreatorUtil.List();
            //PhotoCreatorUtil.CreatePhoto(currentFrame);
        }

        private void statusBarText_Checked(object sender, RoutedEventArgs e)
        {
            ksm.ColorFormat = ColorImageFormat.InfraredResolution640x480Fps30;
        }

        private void statusBarText_Unchecked(object sender, RoutedEventArgs e)
        {
            ksm.ColorFormat = ColorImageFormat.RgbResolution1280x960Fps12;
        }

        private void KinectSkeletonViewer_SkeletonTrackingStateChanged(object sender, Visors.EventHandlers.SkeletonTrackingStateEventArgs e)
        {
            if (e.SkeletonDetected)
            {
                Debug.WriteLine("Detected!!!!!!!!!!!!!!!11");
            }
        }
    }
}
