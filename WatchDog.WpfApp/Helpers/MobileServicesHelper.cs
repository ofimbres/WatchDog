using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDog.WpfApp.Models;

namespace WatchDog.WpfApp.Helpers
{
    public static class MobileServicesHelper
    {
        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://testingms2.azure-mobile.net/",
            "fwcUFdgJMBJOpgyTUmUtqRBmeuqPXa32");

        public static async Task InsertPhotoAudit(PhotoAudit item)
        {
            IMobileServiceTable<PhotoAudit> photoAuditTable = 
                MobileService.GetTable<PhotoAudit>();

            // Inserts a new PhotoAudit into the database.
            // The Insert script on the Mobile Services back end will send a new message to the Service Bus queue.
            await photoAuditTable.InsertAsync(item);
        }
    }
}
