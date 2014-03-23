using Microsoft.WindowsAzure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDog.W8Demo
{
    public static class ServiceBusHelper
    {
        public const string TOPIC_PATH = "testingtopic";
        public const string CONNECTIONSTRING = 
            "Endpoint=sb://testingnothub-ns.servicebus.windows.net/;SharedAccessKeyName=default;SharedAccessKey=4pera1gQB0b9o2JWB+IJB7riciTWL8E/vfQsTQG6bsE=";


        //private static Topic photosTopic = new Topic(
        //    TOPIC_PATH, CONNECTIONSTRING);
        //private static Subscription photosSubscription = new Subscription(
        //    TOPIC_PATH,
        //    "AllMessages",
        //    CONNECTIONSTRING);

        //private static Queue photosQueue = new Queue(
        //    "testingqueue",
        //    "Endpoint=sb://testingnothub-ns.servicebus.windows.net/;SharedAccessKeyName=duma;SharedAccessKey=K9lrd/Bmd2zBrMCXDcV3EU+u+2VybryJtFfp+zwrOYI=");

        public static async Task CreateSubscription(string subscriptionName)
        {
            bool flag = false;
            try
            {
                await Subscription.CreateAsync(TOPIC_PATH, subscriptionName, CONNECTIONSTRING);
            }
            catch (Exception ex)
            {
                flag = true;
            }

            if (flag)
            {
                await Subscription.DeleteAsync(TOPIC_PATH, subscriptionName, CONNECTIONSTRING);
                await CreateSubscription(subscriptionName);
            }
        }

        public static async Task<PhotoViewModel> RetrieveMessage(Subscription photosSubscription)
        {
            PhotoViewModel result;
            try
            {
                string[] message = (await photosSubscription.ReceiveAsync<string>()).Split(',');
                result = new PhotoViewModel()
                {
                    Url = message[0], CreatedDate = message[1]
                };
            }
            catch (MessagingException ex)
            {
                // we need to catch exception thrown when no message is retrieved.
                throw ex;
            }

            return result;
        }
    }
}
