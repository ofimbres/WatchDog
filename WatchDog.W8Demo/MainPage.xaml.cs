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

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            InitNotificationsAsync();
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

            var result = await NotificationHubHelper.RegisterDevice();

            // Displays the registration ID so you know it was successful
            if (result.RegistrationId != null)
            {
                var dialog = new MessageDialog("Registration successful: " + result.RegistrationId);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }

            await AzureStorageHelper.RetrieveQueueMessage("");
        }
    }
}
