using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WatchDog.W8Demo2.Helpers;
using WatchDog.W8Demo2.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WatchDog.W8Demo2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private ObservableCollection<PhotoViewModel> collection;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            collection = new ObservableCollection<PhotoViewModel>();
            list.ItemsSource = collection;

            var selectedDate = datePicker.Date.Date;
            var selectedTime = timePicker.Time;
            selectedDate.Add(selectedTime);

            var list2 = await MobileServicesHelper.GetPhotoAuditsByDate(selectedDate, selectedTime);

            foreach(var item in list2)
            {
                item.Source = await DownloadPhoto(item.Url);
                collection.Add(item);
            }
        }

        private async Task<ImageSource> DownloadPhoto(string url)
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

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            imageSelected.Source = (list.SelectedItem as PhotoViewModel).Source;
        }
    }
}
