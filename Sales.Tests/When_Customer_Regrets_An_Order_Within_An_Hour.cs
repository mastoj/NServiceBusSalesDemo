using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus.Testing;
using NUnit.Framework;
using Sales.Policies;

namespace Sales.Tests
{
    [TestFixture]
    public class When_A_Customer_Places_An_Order : CustomerRemorsePolicyTestBase
    {
        [Test]
        public void The_Order_Should_Be_Preliminary_Accepted_And_Have_IncrementalIds()
        {
            Test.Initialize();
            int customerId = 1;
            int productId = 1;
            CustomerRemorsePolicy.Reset();
            int orderId = 1;
            Test.Saga<CustomerRemorsePolicy>()
                .ExpectPublish<OrderPreliminaryAccepted>(
                    (e) => e.OrderId == 1
                )
                .When(s => s.Handle(CreatePlaceOrder(customerId, productId, orderId)))
                .ExpectPublish<OrderPreliminaryAccepted>(
                    (e) => e.OrderId == 2
                )
                .When(s => s.Handle(CreatePlaceOrder(customerId, productId, orderId + 1)));
        }

        [Test]
        public void A_Customer_Remorse_Timeout_Should_Be_Set()
        {
            Test.Initialize();
            int customerId = 1;
            int productId = 1;
            int orderId = 1;
            CustomerRemorsePolicy.Reset();
            Test.Saga<CustomerRemorsePolicy>()
                .ExpectTimeoutToBeSetIn<CustomerRemorsedTimedOut>((m, ts) =>
                {
                    return ts == TimeSpan.FromSeconds(60)
                           && m.OrderId == orderId;
                })
                .When(s => s.Handle(CreatePlaceOrder(customerId, productId, orderId)));
        }
    }

    [TestFixture]
    public class When_Customer_Remorse_Period_Times_Out : CustomerRemorsePolicyTestBase
    {
        [Test]
        public void The_Order_Is_Accepted()
        {
            Test.Initialize();
            int customerId = 1;
            int productId = 1;
            int orderId = 1;
            CustomerRemorsePolicy.Reset();
            Test.Saga<CustomerRemorsePolicy>()
                .When(s => s.Handle(CreatePlaceOrder(customerId, productId, orderId)))
                .ExpectPublish<OrderAccepted>(s =>
                                              s.CustomerId == customerId &&
                                              s.ProductId == productId &&
                                              s.OrderValue == productId*1000 &&
                                              s.OrderId == orderId)
                .When(s => s.Timeout(CreateOrderTimeOut(orderId)));
        }
    }

    [TestFixture]
    public class When_Customer_Regrets_An_Order_Within_An_Hour : CustomerRemorsePolicyTestBase
    {
        [Test]
        public void The_Order_Accepted_Should_Not_Be_Sent_And_The_Order_Should_Be_Cancelled()
        {
            Test.Initialize();
            int customerId = 1;
            int productId = 1;
            int orderId = 1;
            Test.Saga<CustomerRemorsePolicy>()
                .When(s => s.Handle(CreatePlaceOrder(customerId, productId, orderId)))
                .ExpectPublish<OrderCancelled>(y => y.OrderId == orderId)
                .When(s => s.Handle(CreateCancelOrder(orderId)))
                .ExpectNotPublish<OrderAccepted>(s => s.OrderId == orderId)
                .When(s => s.Timeout(CreateOrderTimeOut(orderId)));
        }
    }

    [TestFixture]
    public class When_Customer_Regrets_An_Order_After_An_Hour : CustomerRemorsePolicyTestBase
    {
        [Test]
        public void The_Order_Should_Not_Be_Cancelled()
        {
            Test.Initialize();
            int customerId = 1;
            int productId = 1;
            int orderId = 1;
            CustomerRemorsePolicy.Reset();
            Test.Saga<CustomerRemorsePolicy>()
                .When(s => s.Handle(CreatePlaceOrder(customerId, productId, orderId)))
                .When(s => s.Timeout(CreateOrderTimeOut(orderId)))
                .ExpectNotPublish<OrderCancelled>(s => s.OrderId == orderId)
                .When(s => s.Handle(CreateCancelOrder(orderId)));
        }
    }

    public abstract class CustomerRemorsePolicyTestBase
    {
        protected PlaceOrder CreatePlaceOrder(int customerId, int productId, int orderId)
        {
            return new PlaceOrder() { CustomerId = customerId, ProductId = productId, OrderId = orderId};
        }
        protected CustomerRemorsedTimedOut CreateOrderTimeOut(int orderId)
        {
            return new CustomerRemorsedTimedOut() { OrderId = orderId };
        }
        protected CancelOrder CreateCancelOrder(int orderId)
        {
            return new CancelOrder() { OrderId = orderId };
        }
    }
}
