using Messages;
using NServiceBus;
using log4net;

namespace Sales.Handlers
{
    public class PlaceOrderHandler : IHandleMessages<PlaceOrder>
    {
        public IBus Bus { get; set; }
        public void Handle(PlaceOrder message)
        {
            LogManager.GetLogger("PlaceOrderHandler").Info("Order accepted, customer: " + message.CustomerId + ", product: "+ message.ProductId);
            Bus.Publish<OrderAccepted>(m =>
                {
                    m.CustomerId = message.CustomerId;
                    m.ProductId = message.ProductId;
                    m.OrderValue = message.ProductId*1000;
                });
        }
    }
}