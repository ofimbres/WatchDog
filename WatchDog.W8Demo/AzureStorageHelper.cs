using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace WatchDog.W8Demo
{
    public static class AzureStorageHelper
    {
        private const string STORAGE_CONNECTIONSTRING =
            "DefaultEndpointsProtocol=https;AccountName=testing2;AccountKey=7TVHhJByZrsgxXAYQa4aNwROPxuBJ9bbX6WvnGZJI4rmBQ3l7r+21txId07VVvD6VjQ/LLkYUiCaGmO7maeCJQ==";

        private const string QUEUE_ENDPOINT = "http://testing2.queue.core.windows.net/";
        private const string ACCOUNT_NAME = "testing2";
        private const string QUEUE_NAME = "myqueue";

        public static async Task RetrieveQueueMessage(string url)
        {
            string requestMethod = "GET";

            String urlPath = String.Format("{0}/messages", QUEUE_NAME);

            String storageServiceVersion = "2012-02-12";
            String dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            String canonicalizedHeaders = String.Format(
                    "x-ms-date:{0}\nx-ms-version:{1}",
                    dateInRfc1123Format,
                    storageServiceVersion);
            String canonicalizedResource = String.Format("/{0}/{1}", ACCOUNT_NAME, urlPath);
            String stringToSign = String.Format(
                    "{0}\n\n\n\n\n\n\n\n\n\n\n\n{1}\n{2}",
                    requestMethod,
                    canonicalizedHeaders,
                    canonicalizedResource);
            String authorizationHeader = CreateAuthorizationHeader(stringToSign);

            Uri uri = new Uri(QUEUE_ENDPOINT + urlPath);

            var request = new HttpClient();
            request.DefaultRequestHeaders.Add("x-ms-date", dateInRfc1123Format);
            request.DefaultRequestHeaders.Add("x-ms-version", storageServiceVersion);
            request.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/atom+xml,application/xml"));

            var response2 = await request.GetStreamAsync(uri.AbsoluteUri);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            //request.Method = requestMethod;
            //request.Headers.Add("x-ms-date", dateInRfc1123Format);
            //request.Headers.Add("x-ms-version", storageServiceVersion);
            //request.Headers.Add("Authorization", authorizationHeader);
            //request.Accept = "application/atom+xml,application/xml";

            //using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            //{
            Stream dataStream = response2; //response.GetResponseStream();
            using (StreamReader reader = new StreamReader(dataStream))
            {
                String responseFromServer = reader.ReadToEnd();
            }
            //}

            //// Retrieve storage account from connection string
            //CloudStorageAccount storageAccount =
            //    CloudStorageAccount.Parse(STORAGE_CONNECTIONSTRING);

            //// Create the queue client
            //CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            //// Retrieve a reference to a queue
            //CloudQueue queue = queueClient.GetQueueReference(QUEUE_NAME);

            //// Peek at the next message
            ////CloudQueueMessage peekedMessage = queue.PeekMessage();
            ////queue.DeleteMessage(peekedMessage);

            //// Display message.
            ////Console.WriteLine(peekedMessage.AsString);
        }

        public static String CreateAuthorizationHeader(String canonicalizedString)
        {
            String signature = String.Empty;

            //using (HMACSHA256 hmacSha256 = new HMACSHA256( Convert.FromBase64String(storageAccountKey) )) {
            //    Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(canonicalizedString);
            //    signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            //}

            byte[] base2 = Convert.FromBase64String("7TVHhJByZrsgxXAYQa4aNwROPxuBJ9bbX6WvnGZJI4rmBQ3l7r+21txId07VVvD6VjQ/LLkYUiCaGmO7maeCJQ==");
            //    Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(canonicalizedString);
            //    signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            signature = GetSHA256Key(base2, canonicalizedString);

            String authorizationHeader = String.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}:{2}",
                "7TVHhJByZrsgxXAYQa4aNwROPxuBJ9bbX6WvnGZJI4rmBQ3l7r+21txId07VVvD6VjQ/LLkYUiCaGmO7maeCJQ==",
                "testing2",
                signature
            );

            return authorizationHeader;
        }

        private static string GetSHA256Key(byte[] secretKey, string value)
        {
            var objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var hash = objMacProv.CreateHash(secretKey.AsBuffer());
            hash.Append(CryptographicBuffer.ConvertStringToBinary(value, BinaryStringEncoding.Utf8));
            return CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());
        }
    }
}
