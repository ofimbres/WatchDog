using Microsoft.WindowsAzure.Messaging;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;

namespace WatchDog.W8Demo
{
    public static class NotificationHubHelper
    {
        private const string HUB_NAME = "testingnothub";
        private const string HUB_CONNSTRING = "Endpoint=sb://testingnothub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=x8xmgwVVLz30UWs1qDdFZL6xDcNEm5tZVUUofl+ixNE=";

        public static PushNotificationChannel Channel { get; private set; }

        public async static Task CreatePushNotificationChannel()
        {
            Channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
        }

        public async static Task<Microsoft.WindowsAzure.Messaging.Registration> RegisterDevice()
        {
            var hub = new NotificationHub(HUB_NAME, HUB_CONNSTRING);
            var result = await hub.RegisterNativeAsync(Channel.Uri);

            return result;
        }
    }
}
