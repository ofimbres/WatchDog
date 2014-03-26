using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WatchDog.WpfApp.Models
{
    [DataContract(Name = "PhotoAudits")]
    public class PhotoAudit
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("createdDate", ItemConverterType = typeof(JavaScriptDateTimeConverter))]
        //[JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime CreatedDate { get; set; }
    }
}
