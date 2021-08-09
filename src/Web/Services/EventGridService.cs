using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Services
{
	public class EventGridService
	{
		private readonly EventGridPublisherClient client;

		public EventGridService(IConfiguration configuration)
		{
			client = new EventGridPublisherClient(new Uri(configuration["EventGrid:Endpoint"]),
				new Azure.AzureKeyCredential(configuration["EventGrid:Key"]));
		}

		public void PublishOrderItemsReserver(string email, Dictionary<string, int> details)
		{
			var random = new Random();
			var orderId = random.Next(100000, 999999);

			var events = new List<CloudEvent>()
			{
				new CloudEvent(
					"/cloudevents/eshop/source",
					"Eshop.OrderItemReserver",
					new { Email = email, OrderId = orderId, Details = details }),
			};

			client.SendEvents(events);
		}
	}
}
