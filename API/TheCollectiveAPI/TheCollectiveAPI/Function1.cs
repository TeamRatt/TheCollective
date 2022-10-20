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

namespace TheCollectiveAPI
{
    public class Function1
    {
        private static HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("accept", "application/json");
            return client;
        }

        [FunctionName("Scan")]
        public void Run([IoTHubTrigger("device/scan", Connection = "HostName=TeamRattTheCollective.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=cxiMDZeiT7sW8WJK0hf4V7petHzIOLemc7c+mtHICwI=")]EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.ToArray())}");
        }

        [FunctionName("GetAllCategoriesPortal")]
        public async Task<IActionResult> GetAllCategoriesP(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/categoriesportal")] HttpRequest req,
            ILogger log)
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    

                    return new OkObjectResult("ok");
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