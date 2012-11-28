using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Saga;

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
            var fedExOrder = new FedExOrder() {OrderId = message.OrderId};
            Bus.Send(fedExOrder);
            RequestUtcTimeout(TimeSpan.FromSeconds(20), fedExOrder);
        }

        public void Timeout(FedExOrder message)
        {
            var upsOrder = new UPSOrder() { OrderId = message.OrderId };
            Bus.Send(upsOrder);
            RequestUtcTimeout(TimeSpan.FromSeconds(20), upsOrder);
        }

        public void Handle(FedExOrder createFedExConfirmation)
        {
            if (Data.ShipmentBooked)
            {
                Bus.Send<CancelFedEx>(y => y.OrderId = createFedExConfirmation.OrderId);
            }
            else
            {
                Bus.Send(new ShipmentBooked() { OrderId = createFedExConfirmation.OrderId });
                Data.ShipmentBooked = true;
            }
        }

        public void Handle(UPSOrder createFedExConfirmation)
        {
            if (Data.ShipmentBooked)
            {
                Bus.Send<CancelUPS>(y => y.OrderId = createFedExConfirmation.OrderId);
            }
            else
            {
                Bus.Send(new ShipmentBooked() { OrderId = createFedExConfirmation.OrderId });
                Data.ShipmentBooked = true;
            }
        }
    }

    public class BookShippingData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        public bool ShipmentBooked { get; set; }
    }
}
