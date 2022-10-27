using System;
using Newtonsoft.Json;

namespace TheCollectiveAPI.Models


{
    public class LastScanHoes
    {

            [JsonProperty("hoes_id")]
            public string HoesId { get; set; }

            [JsonProperty("company_name")]
            public string CompanyName { get; set; }

            [JsonProperty("last_scan")]
            public DateTime lastScan { get; set; }
    
    }
}

