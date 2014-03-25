using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WatchDog.W8Demo
{
    public static class MobileServicesHelper
    {
        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://testingms2.azure-mobile.net/",
            "fwcUFdgJMBJOpgyTUmUtqRBmeuqPXa32");

        public static async Task InsertModeHistory(ModeHistory item)
        {
            IMobileServiceTable<ModeHistory> modeHistoryTable = MobileService.GetTable<ModeHistory>();

            // Inserts a new PhotoAudit into the database.
            // The Insert script on the Mobile Services back end will send a new message to the Service Bus queue.
            await modeHistoryTable.InsertAsync(item);
        }

        public static async Task<bool> GetLastModeStatus()
        {
            IMobileServiceTable<ModeHistory> modeHistoryTable =
                MobileService.GetTable<ModeHistory>();            

            // This query filters out completed TodoItems and 
            // items without a timestamp. 
            List<ModeHistory> items = await modeHistoryTable
               .ToListAsync();

            //// clean table on debugging
            //foreach (var item in items.Take(items.Count - 2))
            //{
            //    await modeHistoryTable.DeleteAsync(item);
            //}

            if (items.Count > 0)
                return items.Last().ModeStatus == "1";
            else
                return false;
        }
    }
}
