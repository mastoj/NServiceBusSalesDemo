using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Saga;
using log4net;

namespace Shipping
{
    public class BookShippingPolicy : Saga<BookShippingData>,
        IAmStartedByMessages<BookShipping>,
        IHandleTimeouts<FedExOrder>,
        IHandleMessages<FedExOrder>,
        IHandleMessages<UPSOrder>
    {
        public void Handle(BookShipping message)
        {
            Console.WriteLine("Booking shipping " + message.OrderId);
            var fedExOrder = new FedExOrder() { OrderId = message.OrderId };
            Bus.Send(fedExOrder);
            RequestUtcTimeout(TimeSpan.FromSeconds(20), fedExOrder);
        }

        public void Timeout(FedExOrder message)
        {
            Console.WriteLine("FedEx timeout");
            if (!Data.ShipmentBooked)
            {
                Console.WriteLine("");
                var upsOrder = new UPSOrder() { OrderId = message.OrderId };
                Bus.Send(upsOrder);
                RequestUtcTimeout(TimeSpan.FromSeconds(20), upsOrder);
            }
        }

        public void Handle(FedExOrder fedExOrder)
        {
            if (Data.ShipmentBooked)
            {
                Console.WriteLine("Canceling FedEx");
//                Bus.Send<CancelFedEx>(y => y.OrderId = fedExOrder.OrderId);
            }
            else
            {
                Console.WriteLine("FedEx confirmed");
                ReplyToOriginator(new ShipmentBooked() { OrderId = fedExOrder.OrderId });
                Data.ShipmentBooked = true;
            }
        }

        public void Handle(UPSOrder upsOrder)
        {
            if (Data.ShipmentBooked)
            {
                Console.WriteLine("Canceling UPS");
//                Bus.Send<CancelUPS>(y => y.OrderId = upsOrder.OrderId);
            }
            else
            {
                Console.WriteLine("UPS confirmed");
                ReplyToOriginator(new ShipmentBooked() { OrderId = upsOrder.OrderId });
                Data.ShipmentBooked = true;
            }
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping(y => y.OrderId, (BookShipping m) => m.OrderId);
        }
    }

    public class BookShippingData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        public bool ShipmentBooked { get; set; }

        public int OrderId { get; set; }
    }
}
