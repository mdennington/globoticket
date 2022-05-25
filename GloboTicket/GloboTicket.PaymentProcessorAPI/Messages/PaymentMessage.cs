using GloboTicket.Messaging;

namespace GloboTicket.PaymentProcessorAPI.Messages
{
    public class PaymentMessage : BaseMessage
    {
        public Guid BasketId { get; set; }
    }
}