using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;



namespace GloboTicket.Messaging
{

    public class AzureServiceBusMessageBus : IMessageBus
    {
        private readonly IConfiguration _config;

        public AzureServiceBusMessageBus(IConfiguration config)
        {
            var currDir = Directory.GetCurrentDirectory();

            _config = config;

        }

        public async Task PublishMessage(BaseMessage message, string queueName)
        {
            // TODO Configure connection string and make this send to a queue not a topic
            string connectionString = _config.GetValue<string>("ConnectionStrings:ServiceBusConnection");

            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(queueName);

            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(finalMessage);

            await client.DisposeAsync();
        }
    }
}

