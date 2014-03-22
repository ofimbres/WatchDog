using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace WatchDog.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensorManager ksm;
        private DispatcherTimer timer;
        private WriteableBitmap lastFrame;

        //private readonly KinectWindowViewModel viewModel;

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
            SetDefaultValues();
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

        // http://stackoverflow.com/a/11720080/1118485
        private async void TimerTick(object sender, EventArgs e)
        {
            await AzureStorageHelper.UploadImage(lastFrame);
        }

        #region KINECT EVENTS
        // Creation of custom event handlers and delegates

        private void ColorViewer_OutputImageChanged(object sender, OutputImageEventArgs e)
        {
            lastFrame = e.Frame;
            Debug.WriteLine("frame updated");
        }

        private bool notifFlag;
        private async void KinectSkeletonViewer_SkeletonTrackingStateChanged(object sender, Visors.EventHandlers.SkeletonTrackingStateEventArgs e)
        {
            if (e.SkeletonDetected && !notifFlag)
            {
                notifFlag = true;

                Debug.WriteLine("Human detected");
                await NotificationHubHelper.SendNotificationAsync();

                // avoid to 
                await Task.Delay(10000);
                notifFlag = false;
            }
        }
        #endregion

        private void SetDefaultValues()
        {
            TurnOnInfrarredVisorButton_Click(null, null);
            AbleSkeletonTrackingButton_Click(null, null);
        }

        #region CONTROLS EVENTS
        private void TurnOnRGBVisorButton_Click(object sender, RoutedEventArgs e)
        {
            TurnOnRGBVisorButton.IsEnabled = false;
            TurnOnInfrarredVisorButton.IsEnabled = true;
            ksm.ColorFormat = ColorImageFormat.RgbResolution1280x960Fps12;
        }

        private void TurnOnInfrarredVisorButton_Click(object sender, RoutedEventArgs e)
        {
            TurnOnRGBVisorButton.IsEnabled = true;
            TurnOnInfrarredVisorButton.IsEnabled = false;
            ksm.ColorFormat = ColorImageFormat.InfraredResolution640x480Fps30;
        }

        private void DisableSkeletonTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AbleSkeletonTrackingButton.IsEnabled = true;
            DisableSkeletonTrackingButton.IsEnabled = false;
            ksm.SkeletonStreamEnabled = false;
        }

        private void AbleSkeletonTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AbleSkeletonTrackingButton.IsEnabled = false;
            DisableSkeletonTrackingButton.IsEnabled = true;
            ksm.SkeletonStreamEnabled = true;
        }
        #endregion
    }
}
