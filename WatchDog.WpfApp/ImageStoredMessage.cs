using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WatchDog.W8Demo
{
    [DataContract]
    public class ImageStoredMessage
    {
        public ImageStoredMessage(string url)
        {
            this.Url = url;
        }
        [JsonProperty]
        public string Url { get; set; }
    }
}
