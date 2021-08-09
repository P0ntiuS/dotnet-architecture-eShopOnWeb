using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using Microsoft.Azure.Storage.Blob;
using System.Net.Http.Headers;

namespace FunctionApps
{
    public static class OrderItemReserver
    {
        [FunctionName("OrderItemReserver")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "options", "post", Route = null)] HttpRequestMessage req,
            [Blob("eshop-order-items", FileAccess.ReadWrite)] CloudBlobContainer orderStorage,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (req.Method == HttpMethod.Options)
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Webhook-Allowed-Origin", "*");

                return response;
            }

            var requestMessage = await req.Content.ReadAsStringAsync();
            var message = JToken.Parse(requestMessage);
            var fileName = $"order-{message["data"]["OrderId"]}.json";

            log.LogInformation($"Request {requestMessage}");

            try
            {
                var container = orderStorage;
                await container.CreateIfNotExistsAsync();

                var blob = container.GetBlockBlobReference(fileName);
                blob.Properties.ContentType = "application/json";

                log.LogInformation($"Message data: {message["data"]}");

                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(message["data"]["Details"].ToString())))
                {
                    await blob.UploadFromStreamAsync(stream);
                }
            }
            catch(Exception e)
            {
                log.LogError(e.Message);
                var url = Environment.GetEnvironmentVariable("LogicAppEndpoint");
                using (var client = new HttpClient())
                {
                    using(var body = new MultipartFormDataContent())
                    {
                        using (var byteContent = new ByteArrayContent(Encoding.UTF8.GetBytes(message["data"]["Details"].ToString())))
                        {
                            byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                            body.Add(byteContent, "file", fileName);

                            var response = await client.PostAsync(url, body);
                        }
                    }
                }
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
