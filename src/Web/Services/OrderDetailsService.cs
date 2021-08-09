using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Services
{
    public class OrderDetailsService
    {
        private readonly string functionAppUrl;
        public OrderDetailsService(IConfiguration configuration)
        {
            functionAppUrl = configuration["SaveOrderDetailsFunctionAppUrl"];
        }

        public async Task SaveAsync(string adress, Dictionary<string, int> items, decimal totalPay)
        {
            var sendedObject = new { TotalPay = totalPay, Adress = adress, OrderItems = items };
            var body = new StringContent(System.Text.Json.JsonSerializer.Serialize(sendedObject));

            using var client = new HttpClient();
            await client.PostAsync(functionAppUrl, body);
        }
    }
}
