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

            // Create subscriptions
            await ServiceBusHelper.CreateModeStatusSubscription(defaultSubscriberName);
            this.MODE_STATUS_OKButton.Text = "OK";

            await ServiceBusHelper.CreatePhotoAuditSubscription(defaultSubscriberName);
            this.PHOTO_AUDIT_OKButton.Text = "OK";

            // Activate listeners
            RetrieveModeStatusCloudMessages();
            RetrieveCloudMessages();

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
                // is that makes sence? I don't think so!
                //if (!isActive)
                //{
                //    await Task.Delay(600);
                //    continue;
                //}

                var vm = await ServiceBusHelper.RetrievePhotoAuditMessage(subscription);

                uiDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        async () =>
                                        {
                                            vm.Source = await DownloadPhoto(vm.Url);
                                            itemsForListBox.Add(vm);

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
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await ShowDialogToActivateCameraStreaming();
                });
            };

            defaultSubscriberName = await RegisterDevice();
        }

        private async Task ShowDialogToActivateCameraStreaming()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var dialog = new MessageDialog("Activate camera 'Live Streaming?'");
                dialog.Commands.Add(new UICommand("OK"));
                dialog.Commands.Add(new UICommand("No"));

                await dialog.ShowAsync();
            });
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            itemsForListBox = new ObservableCollection<PhotoViewModel>();
            listView.ItemsSource = itemsForListBox;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PhotoViewModel item = listView.SelectedItem as PhotoViewModel;
            selectedImage.Source = item.Source;
        }

        private async void observingModeButton_Click(object sender, RoutedEventArgs e)
        {
            observingModeButton.IsEnabled = false;
            passiveModeButton.IsEnabled = true;

            isActive = true;

            //OBSERVING_MODE
            ModeHistory item = new ModeHistory() { ModeStatus = "1", CreatedDate = DateTime.Now };
            await MobileServicesHelper.InsertModeHistory(item);
        }

        private async void passiveModeButton_Click(object sender, RoutedEventArgs e)
        {
            observingModeButton.IsEnabled = true;
            passiveModeButton.IsEnabled = false;

            isActive = false;

            ModeHistory item = new ModeHistory() { ModeStatus = "0", CreatedDate = DateTime.Now };
            await MobileServicesHelper.InsertModeHistory(item);
        }
    }
}
