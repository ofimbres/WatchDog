using Microsoft.ServiceBus.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDog.WpfApp
{
    public static class NotificationHubHelper
    {
        private const string CONNECTION_STRING = "Endpoint=sb://testingnothub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=x8xmgwVVLz30UWs1qDdFZL6xDcNEm5tZVUUofl+ixNE=";
        private const string NOTIFICATIONHUB_PATH = "testingnothub";

        private static string W8Toast =
            "<?xml version='1.0' encoding='utf-8'?>" +
            "<toast launch='HUMAN_DETECTED'>" +
            "<visual><binding template='ToastText01'>" +
            "<text id='1'>Human detected!</text>" +
            "</binding>" +
            "</visual>" +
            "</toast>";

        private static string WP8Toast =
            "<?xml version='1.0' encoding='utf-8'?>" +
            "<wp:Notification xmlns:wp='WPNotification'>" +
            "<wp:Toast>" +
            "<wp:Text1>Human detected!</wp:Text1>" +
            "<wp:Text2>Test message</wp:Text2>" +
            "</wp:Toast>" +
            "</wp:Notification>";

        public static async Task SendNotificationAsync()
        {
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(CONNECTION_STRING, NOTIFICATIONHUB_PATH);
            await hub.SendWindowsNativeNotificationAsync(W8Toast);
            //await hub.SendMpnsNativeNotificationAsync(WP8Toast);
        }
    }
}
