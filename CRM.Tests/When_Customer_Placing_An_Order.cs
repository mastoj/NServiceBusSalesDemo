using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Handlers;
using Messages;
using NServiceBus;
using NServiceBus.Testing;
using NUnit.Framework;

namespace CRM.Tests
{
    [TestFixture]
    public class When_Customer_Placing_An_Order
    {
        [Test]
        public void It_Should_Not_Be_Preferred_When_Running_Total_Is_Less_Than_5000()
        {
            MessageConventionExtensions.IsMessageTypeAction = t => t == typeof(OrderAccepted);
            Test.Initialize();
            Test.Handler<OrderAcceptedHandler>()
                .ExpectNotPublish<CustomerMadePreferred>(tm => false)
                .OnMessage<OrderAccepted>(m =>
                    {
                        m.CustomerId = 1;
                        m.ProductId = 1;
                        m.OrderValue = 5000;
                    });
        }

        [Test]
        public void It_Should_Be_Preferred_When_Running_Total_Is_More_Than_5000()
        {
            MessageConventionExtensions.IsMessageTypeAction = t => t == typeof(OrderAccepted);
            Test.Initialize();
            Test.Handler<OrderAcceptedHandler>()
                .ExpectPublish<CustomerMadePreferred>(tm => tm.CustomerId == 1)
                .OnMessage<OrderAccepted>(m =>
                {
                    m.CustomerId = 1;
                    m.ProductId = 1;
                    m.OrderValue = 5001;
                });
        }

        [Test]
        public void It_Should_Not_Be_Preferred_When_Running_Total_Is_More_Than_5000_And_Customer_Is_Already_Preferred()
        {
            MessageConventionExtensions.IsMessageTypeAction = t => t == typeof(OrderAccepted);
            Test.Initialize();
            Test.Handler<OrderAcceptedHandler>()
                .OnMessage<OrderAccepted>(m =>
                {
                    m.CustomerId = 1;
                    m.ProductId = 1;
                    m.OrderValue = 5001;
                });
            Test.Handler<OrderAcceptedHandler>()
                .ExpectNotPublish<CustomerMadePreferred>(tm => true)
                .OnMessage<OrderAccepted>(m =>
                {
                    m.CustomerId = 1;
                    m.ProductId = 1;
                    m.OrderValue = 5001;
                });
        }
    }
}
