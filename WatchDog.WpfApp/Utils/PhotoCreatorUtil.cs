using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using WatchDog.WpfApp.Models;
using WatchDog.WpfApp.Tables;

namespace WatchDog.WpfApp.Utils
{
    public static class PhotoCreatorUtil
    {
        public static void UploadPhotoToBlogContainer(CamAudit photo)
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            // Create the blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");
            container.CreateIfNotExists();
            container.SetPermissions(
    new BlobContainerPermissions
    {
        PublicAccess =
            BlobContainerPublicAccessType.Blob
    });

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob.png");
            blockBlob.UploadFromByteArray(photo.Picture, 0, photo.Picture.Length);
        }

        public static void UploadImage(CamAudit photo)
        {
            // Retrieve the storage account from the connection string.
            //CloudStorageAccount storageAccount =
            //CloudStorageAccount.Parse(
                //CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            // Create the table client.
            CloudTableClient tableClient =
            storageAccount.CreateCloudTableClient();
            // Create the CloudTable object that represents the "people" table.
            CloudTable table =
            tableClient.GetTableReference("CamAudit");
            table.CreateIfNotExists();
            // Create a new customer entity.
            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(photo);
            // Execute the insert operation.
            table.Execute(insertOperation);

            ///////
            // Get the data service context
            TableServiceContext serviceContext = tableClient.GetTableServiceContext();

            //CamAuditEntity
            List<CamAudit> partitionQuery =
                (from e in serviceContext.CreateQuery<CamAudit>("CamAudit")
                 where e.PartitionKey == "AnyPartition"
                 select e).ToList();
        }

        public static Photo CreatePhoto(WriteableBitmap frame)
        {
            // create a png bitmap encoder which knows how to save a .png file
            BitmapEncoder encoder = new PngBitmapEncoder();

            // create frame from the writable bitmap and add to encoder
            encoder.Frames.Add(BitmapFrame.Create(frame));

            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            // write the new file to disk
            try
            {
                using (MemoryStream fs = new MemoryStream())
                {
                    encoder.Save(fs);
                    fs.Position = 0;

                    //
                    // Retrieve storage account from connection string
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                        ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

                    // Create the blob client
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    // Retrieve reference to a previously created container
                    CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");
                    container.CreateIfNotExists();
                    container.SetPermissions(new BlobContainerPermissions
                    {
                        PublicAccess =
                            BlobContainerPublicAccessType.Blob
                    });

                    // Retrieve reference to a blob named "myblob".
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference("2013-myblob.png");
                    blockBlob.UploadFromStream(fs);

                    Debug.WriteLine("Photo uploaded: " + blockBlob.Uri.ToString());
                    //
                }
            }
            catch (IOException)
            {
                //this.statusBarText.Text = string.Format(CultureInfo.InvariantCulture, "{0} {1}", Properties.Resources.ScreenshotWriteFailed, path);
            }

            return null;
        }

        public static void List()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            // Create the blob client. 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs("", false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;

                    Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;

                    Console.WriteLine("Directory: {0}", directory.Uri);
                }
            }
        }
    }
}
