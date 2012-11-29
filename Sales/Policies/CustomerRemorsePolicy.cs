using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Saga;

namespace Sales.Policies
{
    public class CustomerRemorsePolicy : Saga<CustomerRemorsePolicyData>,
        IAmStartedByMessages<PlaceOrder>,
        IHandleTimeouts<CustomerRemorsedTimedOut>,
        IHandleMessages<CancelOrder>

    {
        private static Dictionary<int, Order> _orders = new Dictionary<int, Order>(); 

        private void AddOrder(Order order)
        {
            int orderId = order.OrderId;
            if (_orders.ContainsKey(orderId))
            {
                _orders[orderId] = order;
            }
            else
            {
                _orders.Add(orderId, order);
            }
        }

        public void Handle(PlaceOrder message)
        {
            Data.OrderId = message.OrderId;
            var order = new Order()
                {
                    CustomerId = message.CustomerId,
                    ProductId = message.ProductId,
                    OrderId = message.OrderId,
                    OrderValue = message.ProductId * 1000
                };
            var orderPrelAcc = new OrderPreliminaryAccepted {OrderId = order.OrderId};
            AddOrder(order);
            Bus.Publish(orderPrelAcc);
            RequestUtcTimeout<CustomerRemorsedTimedOut>(TimeSpan.FromSeconds(60), y =>
                {
                    y.OrderId = 1;
                });
            Console.WriteLine("Order created with id: " + order.OrderId);

        }

        public static void Reset()
        {
            _orders = new Dictionary<int, Order>();
        }

        public void Timeout(CustomerRemorsedTimedOut state)
        {
            if (!Data.OrderCancelled)
            {
                Console.WriteLine("Remorse timeout for order: " + state.OrderId);
                var order = _orders.SingleOrDefault(y => y.Key == state.OrderId).Value;
                if (order != null)
                {
                    Bus.Publish<OrderAccepted>(m =>
                        {
                            m.CustomerId = order.CustomerId;
                            m.ProductId = order.ProductId;
                            m.OrderValue = order.OrderValue;
                            m.OrderId = order.OrderId;
                        });
                    Data.OrderAccepted = true;
                }
            }
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<PlaceOrder>(y => y.OrderId, m => m.OrderId);
            ConfigureMapping<CancelOrder>(y => y.OrderId, m => m.OrderId);
        }

        public void Handle(CancelOrder cancelOrder)
        {
            if (!Data.OrderAccepted)
            {
                Data.OrderCancelled = true;
                Console.WriteLine("Ordered cancelled, order id: " + cancelOrder.OrderId);
                Bus.Publish(new OrderCancelled() {OrderId = cancelOrder.OrderId});
            }
        }
    }

    public class Order : OrderPreliminaryAccepted
    {
        public int ProductId { get; set; }

        public int CustomerId { get; set; }

        public double OrderValue { get; set; }
    }

    public class CustomerRemorsePolicyData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        public bool OrderCancelled { get; set; }

        public int OrderId { get; set; }

        public bool OrderAccepted { get; set; }
    }
}
