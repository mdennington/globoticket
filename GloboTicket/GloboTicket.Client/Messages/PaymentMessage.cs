using GloboTicket.Messaging;
using System;

namespace GloboTicket.Web.Messages
{
    public class PaymentMessage : BaseMessage
    {
        public Guid BasketId { get; set; }
    }
}
