using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDog.WpfApp.Helpers
{
    public static class ServiceBusHelper
    {
        public const string TOPIC_PATH_FOR_MODES = "testingtopicHistory";
        public const string DEFAULT_SUBSCRIPTIONNAME_FOR_MODES = "AllMessages";

        public const string CONNECTIONSTRING =
            "Endpoint=sb://testingnothub-ns.servicebus.windows.net/;SharedAccessKeyName=default;SharedAccessKey=4pera1gQB0b9o2JWB+IJB7riciTWL8E/vfQsTQG6bsE=";

        //private static Subscription modeSubscription = new Subscription(
        //    TOPIC_PATH_FOR_MODES, DEFAULT_SUBSCRIPTIONNAME_FOR_MODES, CONNECTIONSTRING);

        public static async Task<string> RetrieveModeStatusMessage(SubscriptionClient subscriptionClient)
        {
            BrokeredMessage message = await subscriptionClient.ReceiveAsync();

            if (message != null)
            {
                try
                {
                    // Remove message from subscription
                    message.Complete();

                    return message.GetBody<string>();
                }
                catch (Exception)
                {
                    // Indicate a problem, unlock message in subscription
                    message.Abandon();
                }
            }

            return null;
        }
    }
}
