using Microsoft.WindowsAzure.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WatchDog.W8Demo.Models;
using Microsoft.AspNet.SignalR.Client;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WatchDog.W8Demo
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // by default is turnOff camera
        private bool isActive;
        private string defaultSubscriberName;

        IHubProxy imageStreamProxy;
        HubConnection hubConnection;

        public MainPage()
        {
            this.InitializeComponent();

            //Omitted for brevity
            //App.CurrentChannel.PushNotificationReceived +=
            //                  CurrentChannel_PushNotificationReceived;
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await GetDefaultSubscriberNameAsync();
            textbox.Text = defaultSubscriberName;

            itemsForListBox = new ObservableCollection<PhotoViewModel>();
            listView.ItemsSource = itemsForListBox;

            // Create subscriptions
            await ServiceBusHelper.CreateModeStatusSubscription(defaultSubscriberName);
            this.MODE_STATUS_OKButton.Text = "OK";

            await ServiceBusHelper.CreatePhotoAuditSubscription(defaultSubscriberName);
            this.PHOTO_AUDIT_OKButton.Text = "OK";

            // Activate listeners
            RetrieveModeStatusCloudMessages();
            RetrieveCloudMessages();
            RetrieveImageStreamMessages();

            // isActiveOrNot? according to the server
            isActive = await MobileServicesHelper.GetLastModeStatus();

            // Set button-enabling
            if (isActive)
            {
                this.observingModeButton.IsEnabled = false;
                this.passiveModeButton.IsEnabled = true;
            }
            else
            {
                this.observingModeButton.IsEnabled = true;
                this.passiveModeButton.IsEnabled = false;
            }

            // testing for hubs
            //var connection = new HubConnection("http://watchdogsignalrweb2.azurewebsites.net");
            //var myHub = connection.CreateHubProxy("ChatHub");

            //myHub.On<byte[]>("broadcastMessage",
            //    (imageData) => {
            //        int a = 2;
            //        int b = a;
            //        //selectedImage.Source = await ByteArrayToBitmapImage(imageData);
            //    });

            //await connection.Start();
        }

        private async Task<BitmapImage> ByteArrayToBitmapImage(byte[] byteArray)
        {
            var bitmapImage = new BitmapImage();

            var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(byteArray.AsBuffer());
            stream.Seek(0);

            bitmapImage.SetSource(stream);
            return bitmapImage;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            string args = e.Parameter.ToString();
            if (args != "")
                await ShowDialogToActivateCameraStreaming();

            base.OnNavigatedTo(e);
        }

        private ObservableCollection<PhotoViewModel> itemsForListBox;

        private async Task RetrieveCloudMessages()
        {
            var uiDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            Subscription subscription =
                new Subscription(ServiceBusHelper.TOPIC_PATH_FOR_IMAGES, defaultSubscriberName, ServiceBusHelper.CONNECTIONSTRING);

            while (true)
            {
                var vm = await ServiceBusHelper.RetrievePhotoAuditMessage(subscription);

                uiDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        async () =>
                                        {
                                            vm.Source = await DownloadPhoto(vm.Url);
                                            itemsForListBox.Add(vm);
                                            listView.SelectedItem = vm;

                                            ScrollToBottom();
                                        });
            }
        }

        private async Task RetrieveModeStatusCloudMessages()
        {
            var uiDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            Subscription modeSubscription = new Subscription(ServiceBusHelper.
                TOPIC_PATH_FOR_MODES, defaultSubscriberName, ServiceBusHelper.CONNECTIONSTRING2);

            while (true)
            {
                string modeStatusMessage = await ServiceBusHelper.RetrieveModeStatusMessage(modeSubscription);

                uiDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        async () =>
                                        {
                                            if (modeStatusMessage.Equals("1"))
                                            {
                                                isActive = true;

                                                this.observingModeButton.IsEnabled = false;
                                                this.passiveModeButton.IsEnabled = true;
                                            }
                                            else
                                            {
                                                isActive = false;

                                                this.observingModeButton.IsEnabled = true;
                                                this.passiveModeButton.IsEnabled = false;
                                            }
                                        });
            }
        }

        private async Task RetrieveImageStreamMessages()
        {
            var uiDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

            Subscription modeSubscription = new Subscription(ServiceBusHelper.TOPIC_PATH_IMAGESTREAM
    , ServiceBusHelper.DEFAULT_IMAGESTREAM_SUBSCRIBER, ServiceBusHelper.CONNECTIONSTRING3);

            while (true)
            {
                var modeStatusMessage = await ServiceBusHelper.RetrieveImageStreamMessage(modeSubscription);

                uiDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        async () =>
                        {
                            byte[] image = Convert.FromBase64String(modeStatusMessage);

                            var randomAccessStream = new InMemoryRandomAccessStream();
                            var outputStream = randomAccessStream.GetOutputStreamAt(0);
                            var dw = new DataWriter(outputStream);
                            var task = Task.Factory.StartNew(() => dw.WriteBytes(image));
                            await task;
                            await dw.StoreAsync();
                            await outputStream.FlushAsync();

                            var bmi = new WriteableBitmap( 640,480);
                            bmi.SetSource(randomAccessStream);

                            selectedImage.Source = bmi;
                            Debug.WriteLine("RECIBIDO");
                        });
            }
        }

        private void ScrollToBottom()
        {
            var scrollViewer = listView.GetFirstDescendantOfType<ScrollViewer>();
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.ScrollableWidth);
        }

        private async Task GetDefaultSubscriberNameAsync()
        {
            await NotificationHubHelper.CreatePushNotificationChannel();

            NotificationHubHelper.Channel.PushNotificationReceived += async (s, e) =>
            {
                e.Cancel = true;

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await ShowDialogToActivateCameraStreaming();
                });
            };

            defaultSubscriberName = await RegisterDevice();
        }

        bool isActiveDialog;
        private async Task ShowDialogToActivateCameraStreaming()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (!isActiveDialog)
                {
                    isActiveDialog = true;

                    var dialog = new MessageDialog("Activate camera 'Live Streaming?'");
                    dialog.Commands.Add(new UICommand("OK", new UICommandInvokedHandler(OnOkButton)));
                    dialog.Commands.Add(new UICommand("No"));

                    await dialog.ShowAsync();

                    isActiveDialog = false;
                }
            });
        }

        private void OnOkButton(IUICommand command)
        {

        }

        private async Task<string> RegisterDevice()
        {
            var result = await NotificationHubHelper.RegisterDevice();

            // Displays the registration ID so you know it was successful
            if (result.RegistrationId != null)
            {
                //var dialog = new MessageDialog("Registration successful: " + result.RegistrationId);
                //dialog.Commands.Add(new UICommand("OK"));
                //await dialog.ShowAsync();
                return result.RegistrationId;
            }

            throw new InvalidOperationException("result");
        }

        private async Task<ImageSource> DownloadPhoto(string url)
        {
            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = await client.SendAsync(request,
                                         HttpCompletionOption.ResponseHeadersRead);

            InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream();
            DataWriter writer = new DataWriter(ras.GetOutputStreamAt(0));
            writer.WriteBytes(await response.Content.ReadAsByteArrayAsync());
            await writer.StoreAsync();

            BitmapSource image = new BitmapImage();
            image.SetSource(ras);

            return image;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            selectedImage.Source = null;
            itemsForListBox.Clear();
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PhotoViewModel item = listView.SelectedItem as PhotoViewModel;
            selectedImage.Source = item.Source;
        }

        private async void observingModeButton_Click(object sender, RoutedEventArgs e)
        {
            observingModeButton.IsEnabled = false;
            //passiveModeButton.IsEnabled = true;

            isActive = true;

            //OBSERVING_MODE
            ModeHistory item = new ModeHistory() { ModeStatus = "1", CreatedDate = DateTime.Now };
            await MobileServicesHelper.InsertModeHistory(item);
        }

        private async void passiveModeButton_Click(object sender, RoutedEventArgs e)
        {
            observingModeButton.IsEnabled = true;
            //passiveModeButton.IsEnabled = false;

            isActive = false;

            ModeHistory item = new ModeHistory() { ModeStatus = "0", CreatedDate = DateTime.Now };
            await MobileServicesHelper.InsertModeHistory(item);
        }
    }
}
