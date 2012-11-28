using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus.Testing;
using NUnit.Framework;

namespace Shipping.Tests
{
    [TestFixture]
    public class When_Booking_A_Shipment : BookingShippingPolicyTestsBase
    {
        [Test]
        public void FedEx_Should_Be_Used_As_The_Primary_Delivery_Service()
        {
            var orderId = 1;
            Test.Initialize();
            Test.Saga<BookShippingPolicy>()
                .ExpectSend<FedExOrder>(f => f.OrderId == orderId)
                .ExpectTimeoutToBeSetIn<FedExOrder>((tm, ts) => ts == TimeSpan.FromSeconds(20))
                .When(s => s.Handle(CreateBooking(orderId)));
        }

        [Test]
        public void UPS_Should_Be_Used_As_The_Secondary_Delivery_Service()
        {
            var orderId = 1;
            Test.Initialize();
            Test.Saga<BookShippingPolicy>()
                .ExpectSend<UPSOrder>(f => f.OrderId == orderId)
                .ExpectTimeoutToBeSetIn<UPSOrder>((tm, ts) => ts == TimeSpan.FromSeconds(20))
                .When(s => s.Timeout(CreateFedEx(orderId)));
        }
    }
}
