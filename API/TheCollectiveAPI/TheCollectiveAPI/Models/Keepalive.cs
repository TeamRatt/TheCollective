using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCollectiveAPI.Models
{
    internal class Keepalive
    {
        [JsonProperty("MacAddress")]
        public string MacAddress { get; set; }

        [JsonProperty("Timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
