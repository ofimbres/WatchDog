using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WatchDog.WpfApp.Models
{
    [DataContract(Name="PhotoAudits")]
    public class PhotoAudit
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
