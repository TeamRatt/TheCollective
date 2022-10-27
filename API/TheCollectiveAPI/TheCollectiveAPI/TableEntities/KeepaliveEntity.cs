using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCollectiveAPI.TableEntities
{
    internal class KeepaliveEntity : TableEntity
    {
        public KeepaliveEntity() { }
        public KeepaliveEntity(string id, string companyName)
        {
            this.RowKey = id;
            this.PartitionKey = companyName;
        }
        public string MacAddress { get; set; }
    }
}
