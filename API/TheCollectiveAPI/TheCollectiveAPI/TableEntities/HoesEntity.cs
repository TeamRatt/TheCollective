using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCollectiveAPI.TableEntities
{
    internal class HoesEntity : TableEntity
    {
        public HoesEntity() { }


        public string HoesId { get; set; }

        public string CompanyName { get; set; }
    }
}
