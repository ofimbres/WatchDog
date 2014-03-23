using Microsoft.Kinect;
using Microsoft.ServiceBus.Messaging;
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
using WatchDog.WpfApp.Helpers;
using WatchDog.WpfApp.Models;

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

        private bool canPublish;

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

        private async Task Init()
        {
            //NotificationHubHelper.r

            //SubscriptionClient Client = SubscriptionClient.CreateFromConnectionString
            //    (ServiceBus.CONNECTIONSTRING, ServiceBus.TOPIC_PATH_FOR_MODES, ServiceBus.DEFAULT_SUBSCRIPTIONNAME_FOR_MODES);

        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            SetDefaultValues();

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
            if (canPublish)
            {
                await PublishPhoto();
            }
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

        private async Task PublishPhoto()
        {
            SetTextUILog("Publishing...");

            // Step 1: Upload to a blob storage
            var uri = await AzureStorageHelper.UploadImage(lastFrame);

            // Step 2: Insert new url to a Mobile Services table triggering
            // a ServiceBus Topic 
            PhotoAudit photo = new PhotoAudit()
            {
                Url = uri.AbsoluteUri,
                CreatedDate = DateTime.Now
            };
            await MobileServicesHelper.InsertPhotoAudit(photo);

            SetTextUILog(" success: " + photo.Url + "\r\n\r\n");
            Debug.WriteLine("Photo published successfully: " + photo.Id + ", " + photo.Url);
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

        private void SetTextUILog(string text)
        {
            logTextbox.Text += text;
            logTextbox.ScrollToEnd();
        }
    }
}
