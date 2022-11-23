using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCollectiveAPI.TableEntities
{
    internal class FactuurEntity : TableEntity
    {
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string hoesId { get; set; }

        public FactuurEntity() { }
    }
}
