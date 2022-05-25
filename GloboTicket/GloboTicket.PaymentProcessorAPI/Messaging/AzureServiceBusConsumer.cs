using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using GloboTicket.PaymentProcessorAPI.Messages;
using System.Text;

namespace GloboTicket.PaymentProcessorAPI.Messaging
{

    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private ServiceBusProcessor paymentProcessor;
        private readonly IConfiguration _configuration;

        public AzureServiceBusConsumer( IConfiguration configuration)
        {
            _configuration = configuration;

            serviceBusConnectionString = _configuration.GetValue<string>("ConnectionStrings:ServiceBusConnection");
            var client = new ServiceBusClient(serviceBusConnectionString);
            // do I pull queue name in from configuration ??
            paymentProcessor = client.CreateProcessor("payments");
        }

        public async Task Start()
        {
            paymentProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            paymentProcessor.ProcessErrorAsync += ErrorHandler;
            await paymentProcessor.StartProcessingAsync();

        }
        public async Task Stop()
        {
            await paymentProcessor.StopProcessingAsync();
            await paymentProcessor.DisposeAsync();

        }
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            PaymentMessage paymentMessage = JsonConvert.DeserializeObject<PaymentMessage>(body);
            Console.WriteLine($"Payment request received for basket id {paymentMessage.BasketId}.");
        }
    }
}
