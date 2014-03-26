using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace WatchDog.W8Demo2.Models
{
    public class PhotoViewModel
    {
        public string Url { get; set; }
        public string CreatedDate { get; set; }

        public ImageSource Source { get; set; }
    }
}
