using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCollectiveAPI.Models
{
    internal class Hoes
    {
        [JsonProperty("hoes_id")]
        public string HoesId { get; set; }

        [JsonProperty("company_name")]
        public string CompanyName { get; set; }
    }
}
