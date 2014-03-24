﻿using Microsoft.WindowsAzure.Messaging;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace WatchDog.W8Demo
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        // http://go.microsoft.com/fwlink/?LinkId=290986&clcid=0x409
        public static Microsoft.WindowsAzure.MobileServices.MobileServiceClient testingMS2Client = new Microsoft.WindowsAzure.MobileServices.MobileServiceClient(
        "https://testingms3.azure-mobile.net/",
        "fwcUFdgJMBJOpgyTUmUtqRBmeuqPXa32");

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public string AppState { get; set; }

//        public static MobileServiceClient MobileService =
//            new MobileServiceClient("https://testingms2.azure-mobile.net/", "fwcUFdgJMBJOpgyTUmUtqRBmeuqPXa32");

//        public static PushNotificationChannel CurrentChannel;

//        private async Task AcquirePushChannel()
//        {
//            //var user = await MobileService.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
//            //var user2 = await MobileService.LoginWithMicrosoftAccountAsync(user.MobileServiceAuthenticationToken);

//            CurrentChannel = await PushNotificationChannelManager.
//                    CreatePushNotificationChannelForApplicationAsync();

//            //
//CurrentChannel.PushNotificationReceived +=
//                              CurrentChannel_PushNotificationReceived;




//var channelTable = testingMS2Client.GetTable<Channel>();
//            Channel channel = new Channel
//            {
//                Uri = CurrentChannel.Uri,
//                Type = "Windows 8"
//            };
//            await channelTable.InsertAsync(channel);
//        }

//        async void CurrentChannel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
//        {
//            int a = 2;
//            int b = a;
//            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
//            //                          InitApartments);
//        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            AppState = "EXECUTED";
            ////////////////var a = e.Arguments;
            ////////////////var dialog = new MessageDialog(a);
            ////////////////dialog.Commands.Add(new UICommand(e.PreviousExecutionState.ToString()));
            ////////////////dialog.ShowAsync();

            // http://go.microsoft.com/fwlink/?LinkId=290986&clcid=0x409
            //WatchDog.W8Demo.testingMS2Push.UploadChannel();

            //await AcquirePushChannel();

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            AppState = "SUSPENDED";

            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
