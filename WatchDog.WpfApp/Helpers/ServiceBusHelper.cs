using Microsoft.ServiceBus;
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
        public const string DEFAULT_SUBSCRIPTIONNAME_FOR_MODES = "server123";

        public const string CONNECTIONSTRING =
            "Endpoint=sb://testingnothub-ns.servicebus.windows.net/;SharedAccessKeyName=default;SharedAccessKey=4pera1gQB0b9o2JWB+IJB7riciTWL8E/vfQsTQG6bsE=";

        public const string CONNECTIONSTRING2 =
    "Endpoint=sb://testingnothub-ns.servicebus.windows.net/;SharedAccessKeyName=default;SharedAccessKey=iiAsxPzaVuYCFtV0KqbEIU/n+YAZSSxB8i+NS9N8t/A=";

        //private static Subscription modeSubscription = new Subscription(
        //    TOPIC_PATH_FOR_MODES, DEFAULT_SUBSCRIPTIONNAME_FOR_MODES, CONNECTIONSTRING);

        public static async Task CreateModeStatusSubscription()
        {

            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(CONNECTIONSTRING2);

            if (!namespaceManager.SubscriptionExists(TOPIC_PATH_FOR_MODES, DEFAULT_SUBSCRIPTIONNAME_FOR_MODES))
            {
                namespaceManager.CreateSubscription(TOPIC_PATH_FOR_MODES, DEFAULT_SUBSCRIPTIONNAME_FOR_MODES);
            }
            else
            {
                namespaceManager.DeleteSubscription(TOPIC_PATH_FOR_MODES, DEFAULT_SUBSCRIPTIONNAME_FOR_MODES);
                await CreateModeStatusSubscription();
            }

            //bool flag = false;
            //try
            //{
            //    await Subscription.CreateAsync(TOPIC_PATH_FOR_MODES, DEFAULT_SUBSCRIPTIONNAME_FOR_MODES, CONNECTIONSTRING2);
            //}
            //catch (Exception ex)
            //{
            //    flag = true;
            //}

            //if (flag)
            //{
            //    await Subscription.DeleteAsync(TOPIC_PATH_FOR_MODES, DEFAULT_SUBSCRIPTIONNAME_FOR_MODES, CONNECTIONSTRING2);
            //    await CreateModeStatusSubscription(subscriptionName);
            //}
        }

        public static async Task<string> RetrieveModeStatusMessage(SubscriptionClient subscriptionClient)
        {
            BrokeredMessage message = await subscriptionClient.ReceiveAsync();

            if (message != null)
            {
                try
                {
                    // Remove message from subscription
                    message.Complete();

                    return message.Properties["messagenumber"].ToString();
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
