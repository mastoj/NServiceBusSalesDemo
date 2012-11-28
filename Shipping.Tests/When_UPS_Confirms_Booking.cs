using Messages;
using NServiceBus.Testing;
using NUnit.Framework;

namespace Shipping.Tests
{
    [TestFixture]
    public class When_UPS_Confirms_Booking : BookingShippingPolicyTestsBase
    {
        [Test]
        public void The_Booking_Is_Confirmed()
        {
            var orderId = 1;
            Test.Initialize();
            Test.Saga<BookShippingPolicy>()
                .ExpectSend<ShipmentBooked>(f => f.OrderId == orderId)
                .When(s => s.Handle(CreateUPSOrder(orderId)));
        }

        [Test]
        public void The_Booking_Is_Not_Confirmed_If_Already_Confirmed()
        {
            var orderId = 1;
            Test.Initialize();
            Test.Saga<BookShippingPolicy>()
                .When(s => s.Handle(CreateFedEx(orderId)))
                .ExpectNotSend<ShipmentBooked>(s => true)
                .ExpectSend<CancelUPS>(y => y.OrderId == orderId)
                .When(s => s.Handle(CreateUPSOrder(orderId)));
        }
    }
}