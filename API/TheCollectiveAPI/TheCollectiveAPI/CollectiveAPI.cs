using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;
using System;
using Azure.Messaging.EventHubs;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using TheCollectiveAPI.TableEntities;

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
    }
}