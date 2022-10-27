using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TheCollectiveAPI.TableEntities
{
    internal class DeviceEntity : TableEntity
    {
        public DeviceEntity() { }

        public DeviceEntity(string id, string companyName)
        {
            this.PartitionKey = companyName;
            this.RowKey = id;
            this.Timestamp = DateTime.Now;
        }


        public string CompanyName { get; set; }

        public string MacAddress { get; set; }

        public string Location { get; set; }

    }
}
