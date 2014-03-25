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
        private DispatcherTimer timer1; // to Publish Photos
        private DispatcherTimer timer2; // to real-time streaming
        
        private WriteableBitmap lastFrame;

        private bool canPublish;
        private ServiceBus _bus;

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

            _bus = ServiceBus.Setup(ServiceBusUtilities.GetServiceBusCredentials());
        }

        private async Task Init()
        {
            //NotificationHubHelper.r

            //SubscriptionClient Client = SubscriptionClient.CreateFromConnectionString
            //    (ServiceBus.CONNECTIONSTRING, ServiceBus.TOPIC_PATH_FOR_MODES, ServiceBus.DEFAULT_SUBSCRIPTIONNAME_FOR_MODES);

        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            SetDefaultValues();

            // Create subscriptions
            await ServiceBusHelper.CreateModeStatusSubscription();

            // Activate listeners
            RetrieveModeStatusCloudMessages();

            // isActiveOrNot? according to the server
            canPublish = await MobileServicesHelper.GetLastModeStatus();

            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromSeconds(10d);
            timer1.Tick += TimerTick;
            timer1.Start();

            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(300);
            timer2.Tick += TimerTick2;
            timer2.Start();
        }

        private async Task RetrieveModeStatusCloudMessages()
        {
            var uiDispatcher = this.Dispatcher;

            SubscriptionClient modeSubscription = SubscriptionClient.CreateFromConnectionString(
                ServiceBusHelper.CONNECTIONSTRING2, ServiceBusHelper.TOPIC_PATH_FOR_MODES, ServiceBusHelper.DEFAULT_SUBSCRIPTIONNAME_FOR_MODES);

            while (true)
            {
                string modeStatusMessage = await ServiceBusHelper.RetrieveModeStatusMessage(modeSubscription);

                uiDispatcher.BeginInvoke(
                                        new Action(() =>
                                        {
                                            if (modeStatusMessage != null)
                                            {
                                                if (modeStatusMessage.Equals("1"))
                                                {
                                                    canPublish = true;
                                                }
                                                else
                                                {
                                                    canPublish = false;
                                                }
                                            }
                                        }), DispatcherPriority.Normal);
            }
        }


        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        // http://stackoverflow.com/a/11720080/1118485
        private async void TimerTick(object sender, EventArgs e)
        {
            if (canPublish)
            {
                //await PublishPhoto();
            }
        }

        private void TimerTick2(object sender, EventArgs e)
        {
            var stream = ImageEncodingHelper.GetGifStreamFromWriteableBitmap(lastFrame);

            var message = new ImageStreamMessage() { ImageData = stream.ToArray() };
            _bus.Publish<ImageStreamMessage>(message);
            Debug.Write("COOL!!");
        }

        #region KINECT EVENTS
        // Creation of custom event handlers and delegates

        private void ColorViewer_OutputImageChanged(object sender, OutputImageEventArgs e)
        {
            lastFrame = e.Frame.Clone();
            //Debug.WriteLine("frame updated");
        }

        private bool notifFlag;
        private async void KinectSkeletonViewer_SkeletonTrackingStateChanged(object sender, Visors.EventHandlers.SkeletonTrackingStateEventArgs e)
        {
            if (e.SkeletonDetected && !notifFlag && canPublish)
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
