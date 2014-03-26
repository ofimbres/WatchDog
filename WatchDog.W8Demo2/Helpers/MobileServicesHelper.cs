using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WatchDog.W8Demo2.Models;
using WatchDog.WpfApp.Models;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WatchDog.W8Demo2.Helpers
{
    public static class MobileServicesHelper
    {
        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://testingms2.azure-mobile.net/",
            "fwcUFdgJMBJOpgyTUmUtqRBmeuqPXa32");

        public static async Task<List<PhotoViewModel>> GetPhotoAuditsByDate(DateTime date, TimeSpan time)
        {
            IMobileServiceTable<PhotoAudit> photoAuditTable =
                MobileService.GetTable<PhotoAudit>();

            var query = await photoAuditTable
                .Where(x => x.CreatedDate >= date.Add(time) && x.CreatedDate < date.AddDays(1))
                .Select(x => new PhotoViewModel()
                {
                    Url = x.Url,
                    CreatedDate = x.CreatedDate.Date.ToString()
                })
                .ToListAsync();

            return query;
        }

        private static async Task<ImageSource> DownloadPhoto(string url)
        {
            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = await client.SendAsync(request,
                                         HttpCompletionOption.ResponseHeadersRead);

            InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream();
            DataWriter writer = new DataWriter(ras.GetOutputStreamAt(0));
            writer.WriteBytes(await response.Content.ReadAsByteArrayAsync());
            await writer.StoreAsync();

            BitmapSource image = new BitmapImage();
            image.SetSource(ras);

            return image;
        }

    }
}
