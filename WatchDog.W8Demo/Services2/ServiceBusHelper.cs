using Microsoft.WindowsAzure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDog.W8Demo.Models;

namespace WatchDog.W8Demo
{
    public static class ServiceBusHelper
    {
        public const string TOPIC_PATH_FOR_IMAGES = "testingtopic";

        public const string TOPIC_PATH_FOR_MODES = "testingtopicHistory";
        //public const string DEFAULT_SUBSCRIPTIONNAME_FOR_MODES = "AllMessages";

        public const string CONNECTIONSTRING = 
            "Endpoint=sb://testingnothub-ns.servicebus.windows.net/;SharedAccessKeyName=default;SharedAccessKey=4pera1gQB0b9o2JWB+IJB7riciTWL8E/vfQsTQG6bsE=";
        public const string CONNECTIONSTRING2 =
            "Endpoint=sb://testingnothub-ns.servicebus.windows.net/;SharedAccessKeyName=default;SharedAccessKey=iiAsxPzaVuYCFtV0KqbEIU/n+YAZSSxB8i+NS9N8t/A=";

        //private static Subscription modeSubscription = new Subscription(
        //    TOPIC_PATH_FOR_MODES, DEFAULT_SUBSCRIPTIONNAME_FOR_MODES, CONNECTIONSTRING);

        //private static Topic photosTopic = new Topic(
        //    TOPIC_PATH, CONNECTIONSTRING);
        //private static Subscription photosSubscription = new Subscription(
        //    TOPIC_PATH,
        //    "AllMessages",
        //    CONNECTIONSTRING);

        //private static Queue photosQueue = new Queue(
        //    "testingqueue",
        //    "Endpoint=sb://testingnothub-ns.servicebus.windows.net/;SharedAccessKeyName=duma;SharedAccessKey=K9lrd/Bmd2zBrMCXDcV3EU+u+2VybryJtFfp+zwrOYI=");

        public static async Task CreatePhotoAuditSubscription(string subscriptionName)
        {
            bool flag = false;
            try
            {
                await Subscription.CreateAsync(TOPIC_PATH_FOR_IMAGES, subscriptionName, CONNECTIONSTRING);
            }
            catch (Exception ex)
            {
                flag = true;
            }

            if (flag)
            {
                await Subscription.DeleteAsync(TOPIC_PATH_FOR_IMAGES, subscriptionName, CONNECTIONSTRING);
                await CreatePhotoAuditSubscription(subscriptionName);
            }
        }

        public static async Task CreateModeStatusSubscription(string subscriptionName)
        {
            bool flag = false;
            try
            {
                await Subscription.CreateAsync(TOPIC_PATH_FOR_MODES, subscriptionName, CONNECTIONSTRING2);
            }
            catch (Exception ex)
            {
                flag = true;
            }

            if (flag)
            {
                await Subscription.DeleteAsync(TOPIC_PATH_FOR_MODES, subscriptionName, CONNECTIONSTRING2);
                await CreateModeStatusSubscription(subscriptionName);
            }
        }

        public static async Task<PhotoViewModel> RetrievePhotoAuditMessage(Subscription photosSubscription)
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

        public static async Task<string> RetrieveModeStatusMessage(Subscription modeSubscription)
        {
            string result;
            try
            {
                result = await modeSubscription.ReceiveAsync<string>();
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
