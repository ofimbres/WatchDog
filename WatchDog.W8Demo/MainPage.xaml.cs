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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WatchDog.W8Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

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

        }

        private ObservableCollection<PhotoViewModel> itemsForListBox;

        private async Task RetrieveCloudMessages(string subscriptionName)
        {
            var uiDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            Subscription subscription =
                new Subscription(ServiceBusHelper.TOPIC_PATH, subscriptionName, ServiceBusHelper.CONNECTIONSTRING);

            while (true)
            {
               
                var vm = await ServiceBusHelper.RetrieveMessage(subscription);

                uiDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        async () =>
                                        {
                                            vm.Source = await DownloadPhoto(vm.Url);
                                            itemsForListBox.Add(vm);

                                            ScrollToBottom();
                                        });
            }
        }

        private void ScrollToBottom()
        {
            var scrollViewer = listView.GetFirstDescendantOfType<ScrollViewer>();
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.ScrollableWidth);
        }

        private async void InitNotificationsAsync()
        {
            await NotificationHubHelper.CreatePushNotificationChannel();

            NotificationHubHelper.Channel.PushNotificationReceived += async (s, e) =>
            {
                //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                //  {
                //      var dialog = new MessageDialog("duma");
                //      dialog.Commands.Add(new UICommand("OK"));
                //      await dialog.ShowAsync();
                //  });
            };

            await RegisterDevice();
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
            //string subscriptionName = textbox.Text;
            //await ServiceBusHelper.CreateSubscription(subscriptionName);

            itemsForListBox = new ObservableCollection<PhotoViewModel>();
            listView.ItemsSource = itemsForListBox;
            //InitNotificationsAsync();

            await NotificationHubHelper.CreatePushNotificationChannel();
            var registrationId = await RegisterDevice();

            await ServiceBusHelper.CreateSubscription(registrationId);
            RetrieveCloudMessages(registrationId);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PhotoViewModel item = listView.SelectedItem as PhotoViewModel;
            selectedImage.Source = item.Source;
        }
    }
    
    public class PhotoViewModel
    {
        public string Url { get; set; }
        public string CreatedDate { get; set; }

        public ImageSource Source { get; set; }
    }
}
