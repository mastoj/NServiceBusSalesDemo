using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace UI.Handlers
{
    public class ShipmentBookedHandler : IHandleMessages<ShipmentBooked>
    {
        public void Handle(ShipmentBooked message)
        {
            Console.WriteLine("Shipment booked for order: " + message.OrderId);
        }
    }

    public class OrderedCancelledHandler : IHandleMessages<OrderCancelled>
    {
        public void Handle(OrderCancelled message)
        {
            Console.WriteLine("Order cancelled for: " + message.OrderId);
        }
    }

    public class OrderAcceptedHandler : IHandleMessages<OrderAccepted>
    {
        public void Handle(OrderAccepted message)
        {
            Console.WriteLine("Order accepted for: " + message.OrderId);
        }
    }
}
