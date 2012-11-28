using System.Collections.Generic;
using Messages;
using NServiceBus;
using log4net;

namespace CRM.Handlers
{
    public class OrderAcceptedHandler : IHandleMessages<OrderAccepted>
    {
        private static readonly Dictionary<int, double> _runningTotals = new Dictionary<int, double>();
        private static readonly List<int> _preferredCustomers = new List<int>();

        public IBus Bus { get; set; }
        public void Handle(OrderAccepted message)
        {
            LogManager.GetLogger("OrderAcceptedHandler").Info("Order accepted, customer: " + message.CustomerId + ", product: " + message.ProductId + ", Order value: " + message.OrderValue);
            if (_runningTotals.ContainsKey(message.CustomerId))
            {
                _runningTotals[message.CustomerId] += message.OrderValue;
            }
            else
            {
                _runningTotals.Add(message.CustomerId, message.OrderValue);
            }
            var runningTotal = _runningTotals[message.CustomerId];
            if (runningTotal > 5000 && !_preferredCustomers.Contains(message.CustomerId))
            {
                _preferredCustomers.Add(message.CustomerId);
                LogManager.GetLogger("OrderAcceptedHandler")
                          .Info("Making customer preferred, customer: " + message.CustomerId +
                                " with a running total of: " + runningTotal);
                Bus.Publish<CustomerMadePreferred>(m =>
                {
                    m.CustomerId = message.CustomerId;
                });                
            }
        }
    }
}