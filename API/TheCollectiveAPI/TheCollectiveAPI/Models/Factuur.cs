using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCollectiveAPI.Models
{
    internal class Factuur
    {
        [JsonProperty("startdate")]
        public DateTime startdate { get; set; }

        [JsonProperty("enddate")]
        public DateTime enddate { get; set; }

        [JsonProperty("hoesId")]
        public string hoesId { get; set; }
    }
}
