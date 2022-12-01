using Microsoft.Azure.WebJobs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;
using System;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using TheCollectiveAPI.TableEntities;
using Newtonsoft.Json;
using System.IO;
using TheCollectiveAPI.Models;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using System.Linq;

namespace TheCollectiveAPI
{
    public class CollectiveAPI
    {
        private static HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("accept", "application/json");
            return client;
        }

        /*[FunctionName("Scan")]
        public void Run([IoTHubTrigger("device/scan", Connection = "HostName=TeamRattTheCollective.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=cxiMDZeiT7sW8WJK0hf4V7petHzIOLemc7c+mtHICwI=")]EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.ToArray())}");
        }*/

        [FunctionName("GetAllScans")]
        public async Task<IActionResult> GetAllScans(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/scans")] HttpRequest req,
            ILogger log)
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    string connectionString = Environment.GetEnvironmentVariable("ConnectionStringStorage");
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                    CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
                    CloudTable table = cloudTableClient.GetTableReference("Scans");

                    TableQuery<ScanEntity> rangeQuery = new TableQuery<ScanEntity>();

                    var queryResult = await table.ExecuteQuerySegmentedAsync(rangeQuery, null);

                    ListScans listScans = new ListScans()
                    {
                        Scans = queryResult.Results,
                    };

                    return new OkObjectResult(listScans);
                }
                catch (Exception ex)
                {
                    log.LogError(ex.ToString());
                    return new StatusCodeResult(500);
                }
            }
        }


        [FunctionName("GetAllDevics")]
        public async Task<IActionResult> GetAllDevices(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/devices")] HttpRequest req,
            ILogger log)
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    string connectionString = Environment.GetEnvironmentVariable("ConnectionStringStorage");
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                    CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
                    CloudTable table = cloudTableClient.GetTableReference("Devices");

                    TableQuery<DeviceEntity> rangeQuery = new TableQuery<DeviceEntity>();

                    var queryResult = await table.ExecuteQuerySegmentedAsync(rangeQuery, null);

                    ListDevices listDevices = new ListDevices()
                    {
                        Devices = queryResult.Results,
                    };

                    return new OkObjectResult(listDevices);
                }
                catch (Exception ex)
                {
                    log.LogError(ex.ToString());
                    return new StatusCodeResult(500);
                }
            }
        }


        [FunctionName("GetAllKeepalive")]
        public async Task<IActionResult> GetAllKeepalive(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/keepalive")] HttpRequest req,
            ILogger log)
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    string connectionString = Environment.GetEnvironmentVariable("ConnectionStringStorage");
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                    CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
                    CloudTable table = cloudTableClient.GetTableReference("Keepalive");

                    TableQuery<KeepaliveEntity> rangeQuery = new TableQuery<KeepaliveEntity>();

                    var queryResult = await table.ExecuteQuerySegmentedAsync(rangeQuery, null);

                    ListKeepalive listKeepalive = new ListKeepalive()
                    {
                        Keepalives = queryResult.Results,
                    };

                    return new OkObjectResult(listKeepalive);
                }
                catch (Exception ex)
                {
                    log.LogError(ex.ToString());
                    return new StatusCodeResult(500);
                }
            }
        }

        //Geef alle hoezen weer.
        [FunctionName("GetHoezen")]
        public async Task<IActionResult> GetHoezen(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/hoezen/{companyName}")] HttpRequest req,
            string companyName,
            ILogger log)
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    string connectionString = Environment.GetEnvironmentVariable("ConnectionStringStorage");
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                    CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
                    CloudTable table = cloudTableClient.GetTableReference("Hoezen");

                    TableQuery<HoesEntity> rangeQuery = new TableQuery<HoesEntity>();

                    rangeQuery.Where(TableQuery.GenerateFilterCondition("CompanyName", QueryComparisons.Equal, companyName));


                    var queryResult = await table.ExecuteQuerySegmentedAsync(rangeQuery, null);

                    ListHoezen listHoezen = new ListHoezen()
                    {
                        listHoezen = queryResult.Results,
                    };

                    //return new OkObjectResult(listHoezen);

                    //We overlopen elke hoes van de lijst van de hoezen van bedrijf x
                    // => vervolgens doen we query naar table

                    ListScans listScansTemp = new ListScans() { Scans = null };
                    List<ScanEntity> listScans = new List<ScanEntity>();
                    table = cloudTableClient.GetTableReference("Scans");


                    foreach (var hoes in listHoezen.listHoezen)
                    {
                        TableQuery<ScanEntity> rangeQuery2 = new TableQuery<ScanEntity>();

                        rangeQuery2.Where(TableQuery.GenerateFilterCondition("ScannedId", QueryComparisons.Equal, hoes.HoesId));
                        var queryResult2 = await table.ExecuteQuerySegmentedAsync(rangeQuery2, null);
                        queryResult2.Results.Sort((b, a) => a.Timestamp.CompareTo(b.Timestamp));
                        listScansTemp.Scans = queryResult2.Results;
                        listScans.AddRange(listScansTemp.Scans.Take(1));                        
                    };



                    ListDevices listDevicesTemp = new ListDevices() { Devices = null };
                    List<DeviceEntity> listDevices = new List<DeviceEntity>();
                    table = cloudTableClient.GetTableReference("Devices");
                    foreach(var scan in listScans)
                    {
                        TableQuery<DeviceEntity> rangeQueryDevice = new TableQuery<DeviceEntity>();
                        rangeQueryDevice.Where(TableQuery.GenerateFilterCondition("MacAddress", QueryComparisons.Equal, scan.MacAddress));
                        var queryResultDevice = await table.ExecuteQuerySegmentedAsync(rangeQueryDevice, null);
                        listDevicesTemp.Devices = queryResultDevice.Results;
                        listDevices.AddRange(listDevicesTemp.Devices.Take(1));
                    }

                    List<LastScanHoes> listLastScanHoes = new List<LastScanHoes>();
                    string hoesid, companyname, location;
                    DateTime lastscan;
                    foreach(var hoes in listHoezen.listHoezen)
                    {
                        hoesid = hoes.HoesId;
                        companyname = hoes.CompanyName;
                        ScanEntity tempscan = new ScanEntity();
                        location = "";
                        lastscan = DateTime.MinValue;
                        foreach(var scan in listScans)
                        {
                            if (scan.ScannedId == hoes.HoesId) 
                            { 
                                lastscan = scan.Timestamp.DateTime;
                                tempscan = scan;
                            }
                        }
                        foreach(var device in listDevices) 
                        {
                            if (device.MacAddress == tempscan.MacAddress)
                            {
                                location = device.Location;
                            }
                        }

                        LastScanHoes tempScanhoes = new LastScanHoes()
                        {
                            HoesId = hoesid,
                            CompanyName = companyname,
                            Location = location,
                            lastScan = lastscan
                        };
                        listLastScanHoes.Add(tempScanhoes);
                    }
                    return new OkObjectResult(listLastScanHoes);

                }
                catch (Exception ex)
                {
                    log.LogError(ex.ToString());
                    return new StatusCodeResult(500);
                }
            }
        }


        //Geef alle hoezen weer.
        [FunctionName("GetCompanyNames")]
        public async Task<IActionResult> GetCompanynames(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/companynames")] HttpRequest req,
            ILogger log)
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    string connectionString = Environment.GetEnvironmentVariable("ConnectionStringStorage");
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                    CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
                    CloudTable table = cloudTableClient.GetTableReference("Devices");

                    TableQuery<DeviceEntity> rangeQuery = new TableQuery<DeviceEntity>();

                    var queryResult = await table.ExecuteQuerySegmentedAsync(rangeQuery, null);

                    ListDevices listDevices = new ListDevices()
                    {
                        Devices = queryResult.Results,
                    };

                    List<string> listCompanyNames = new List<string>();
                    foreach (DeviceEntity device in queryResult.Results)
                    {
                        if (!listCompanyNames.Contains(device.CompanyName.ToString())) { listCompanyNames.Add(device.CompanyName.ToString()); }
                    }

                    return new OkObjectResult(listCompanyNames);
                }
                catch (Exception ex)
                {
                    log.LogError(ex.ToString());
                    return new StatusCodeResult(500);
                }
            }
        }

        [FunctionName("AddScan")]
        public async Task<IActionResult> AddScan(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/scans")] HttpRequest req,
            ILogger log)
        {

            using (HttpClient client = GetClient())
            {
                try
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    Scan scan = JsonConvert.DeserializeObject<Scan>(requestBody);

                    string connectionString = Environment.GetEnvironmentVariable("ConnectionStringStorage");
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                    CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
                    CloudTable table = cloudTableClient.GetTableReference("Scans");

                    if (scan.InOut == false)
                    {
                        Guid FacturationGuid = System.Guid.NewGuid();
                        ScanEntity scanEntity = new ScanEntity()
                        {
                            MacAddress = scan.MacAddress,
                            ScannedId = scan.ScannedId,
                            CompanyName = scan.CompanyName,
                            PartitionKey = scan.CompanyName,
                            RowKey = System.Guid.NewGuid().ToString(),
                            InOut = scan.InOut,
                            FacturationId = FacturationGuid
                        };
                        TableOperation insertOperation = TableOperation.Insert(scanEntity);
                        await table.ExecuteAsync(insertOperation);

                        table = cloudTableClient.GetTableReference("Facturation");

                        FactuurEntity factuurEntity = new FactuurEntity()
                        {
                            hoesId = scan.ScannedId,
                            startdate = DateTime.Now,
                            enddate = new DateTime(1601, 1, 2),
                            PartitionKey = scan.CompanyName,
                            RowKey = FacturationGuid.ToString(),
                        };
                        insertOperation = TableOperation.Insert(factuurEntity);
                        await table.ExecuteAsync(insertOperation);
                    }
                    else if (scan.InOut)
                    {
                        table = cloudTableClient.GetTableReference("Scans");
                        TableQuery<ScanEntity> rangeQuery = new TableQuery<ScanEntity>();
                        rangeQuery.Where(TableQuery.GenerateFilterCondition("ScannedId", QueryComparisons.Equal, scan.ScannedId));
                        var queryResult = await table.ExecuteQuerySegmentedAsync(rangeQuery, null);
                        queryResult.Results.Sort((b, a) => a.Timestamp.CompareTo(b.Timestamp));
                        List<ScanEntity> lastScan = new List<ScanEntity>(queryResult.Results.Take(1));

                        ScanEntity scanEntity = new ScanEntity()
                        {
                            MacAddress = scan.MacAddress,
                            ScannedId = scan.ScannedId,
                            CompanyName = scan.CompanyName,
                            PartitionKey = scan.CompanyName,
                            RowKey = System.Guid.NewGuid().ToString(),
                            InOut = scan.InOut,
                            FacturationId = lastScan[0].FacturationId
                        };
                        TableOperation insertOperation = TableOperation.Insert(scanEntity);
                        await table.ExecuteAsync(insertOperation);

                        table = cloudTableClient.GetTableReference("Facturation");

                        TableQuery<FactuurEntity> rangeQueryFactuur = new TableQuery<FactuurEntity>();
                        rangeQueryFactuur.Where(TableQuery.GenerateFilterCondition("hoesId", QueryComparisons.Equal, scan.ScannedId));
                        var queryResultFactuur = await table.ExecuteQuerySegmentedAsync(rangeQueryFactuur, null);
                        queryResultFactuur.Results.Sort((b, a) => a.Timestamp.CompareTo(b.Timestamp));
                        List<FactuurEntity> lastFactuur = new List<FactuurEntity>(queryResultFactuur.Results.Take(1));

                        lastFactuur[0].enddate = DateTime.Now;
                        TableOperation replaceOperation = TableOperation.Replace(lastFactuur[0]);
                        await table.ExecuteAsync(replaceOperation);
                        return new OkObjectResult(lastFactuur[0]);
                    }








                    return new OkObjectResult(scan);
                }
                catch (Exception ex)
                {
                    log.LogError(ex.ToString());
                    return new StatusCodeResult(500);
                }
            }
        }

        [FunctionName("GetScanFromDevice")]
        public async Task<IActionResult> GetScanFromDevice(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/scans/{deviceid}")] HttpRequest req,
            string deviceid,
            ILogger log)
        {

            using (HttpClient client = GetClient())
            {
                try
                {
                    string connectionString = Environment.GetEnvironmentVariable("ConnectionStringStorage");
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                    CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
                    CloudTable table = cloudTableClient.GetTableReference("Scans");

                    TableQuery<ScanEntity> rangeQuery = new TableQuery<ScanEntity>();

                    rangeQuery.Where(TableQuery.GenerateFilterCondition("MacAddress", QueryComparisons.Equal, deviceid));
                    var queryResult = await table.ExecuteQuerySegmentedAsync(rangeQuery, null);

                    ListScans listScans = new ListScans()
                    {
                        Scans = queryResult.Results,
                    };

                    return new OkObjectResult(listScans);
                }
                catch (Exception ex)
                {
                    log.LogError(ex.ToString());
                    return new StatusCodeResult(500);
                }
            }
        }
    }
}