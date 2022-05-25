using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloboTicket.Messages
{
    public class PaymentMessage
    {
        public Guid BasketId { get; set; }
    }
}
