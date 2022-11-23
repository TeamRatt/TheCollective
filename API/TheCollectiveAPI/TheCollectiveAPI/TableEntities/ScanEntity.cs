using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCollectiveAPI.TableEntities
{
    internal class ScanEntity : TableEntity
    {
        public ScanEntity() { }
        public ScanEntity(string id, string CompanyName)
        {
            this.RowKey = id;
            this.PartitionKey = CompanyName;
            this.Timestamp = DateTime.Now;
        }
        public string MacAddress { get; set; }

        public string ScannedId { get; set; }
        public string CompanyName { get; set; }
        public bool InOut { get; set; }
        public Guid FacturationId { get; set; }

    }
}
