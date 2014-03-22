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
        private const string QUEUE_NAME = "myqueue";


        public static async Task<Uri> UploadImage(WriteableBitmap bitmapSource)
        {
            // create a png bitmap encoder which knows how to save a .png file
            BitmapEncoder encoder = new PngBitmapEncoder();

            // create frame from the writable bitmap and add to encoder
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (MemoryStream fs = new MemoryStream())
            {
                encoder.Save(fs);
                fs.Position = 0;

                string time = System.DateTime.Now.ToString("yyyy-MM-dd--hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

                CloudBlockBlob blob = await UploadImageToBlobContainer(time, fs);
                string url = blob.Uri.AbsoluteUri;
                //PublishQueueMessage(url);

                return blob.Uri;
            }
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


        //public static void A()
        //{
        //    // Retrieve storage account from connection string
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(STORAGE_CONNECTIONSTRING);

        //    // Create the queue client
        //    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

        //    // Retrieve a reference to a queue
        //    CloudQueue queue = queueClient.GetQueueReference("myqueue");

        //    // Get the next message
        //    CloudQueueMessage retrievedMessage = queue.GetMessage();

        //    //Process the message in less than 30 seconds, and then delete the message
        //    queue.DeleteMessage(retrievedMessage);
        //}

        //public static void List()
        //{
        //    // Retrieve storage account from connection string.
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(STORAGE_CONNECTIONSTRING);

        //    // Create the blob client. 
        //    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //    // Retrieve reference to a previously created container.
        //    CloudBlobContainer container = blobClient.GetContainerReference(CONTAINER_NAME);

        //    // Loop over items within the container and output the length and URI.
        //    foreach (IListBlobItem item in container.ListBlobs("", false))
        //    {
        //        CloudBlockBlob blob = (CloudBlockBlob)item;

        //        Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);
        //    }
        //}
    }
}
