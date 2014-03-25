using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WatchDog.WpfApp.Helpers;
using WatchDog.WpfApp.Models;

namespace WatchDog.WpfApp
{
    public static class AzureStorageHelper
    {
        private const string STORAGE_CONNECTIONSTRING =
            "DefaultEndpointsProtocol=https;AccountName=testing2;AccountKey=7TVHhJByZrsgxXAYQa4aNwROPxuBJ9bbX6WvnGZJI4rmBQ3l7r+21txId07VVvD6VjQ/LLkYUiCaGmO7maeCJQ==";

        private const string CONTAINER_NAME = "mycontainer";


        public static async Task<Uri> UploadImage(WriteableBitmap bitmapSource)
        {
            Stream fs = ImageEncodingHelper.GetJpegStreamFromWriteableBitmap(bitmapSource);
            string time = System.DateTime.Now.ToString("yyyy-MM-dd--hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            CloudBlockBlob blob = await UploadImageToBlobContainer(time + ".jpg", fs);
            string url = blob.Uri.AbsoluteUri;
            //PublishQueueMessage(url);

            return blob.Uri;
        }

        private static async Task<CloudBlockBlob> UploadImageToBlobContainer(string filename, Stream source)
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(STORAGE_CONNECTIONSTRING);

            // Create the blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container
            CloudBlobContainer container = blobClient.GetContainerReference(CONTAINER_NAME);
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);
            await blockBlob.UploadFromStreamAsync(source);

            return blockBlob;
        }

        /////////////////////////////////// could be
        //private static void PublishQueueMessage(string url)
        //{
        //    // Retrieve storage account from connection string
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(STORAGE_CONNECTIONSTRING);

        //    // Create the queue client
        //    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

        //    // Retrieve a reference to a queue
        //    CloudQueue queue = queueClient.GetQueueReference(QUEUE_NAME);

        //    // Create the queue if it doesn't already exist
        //    queue.CreateIfNotExists();

        //    // Create a message and add it to the queue.
        //    CloudQueueMessage message = new CloudQueueMessage(url);
        //    queue.AddMessage(message);
        //}
    }
}
