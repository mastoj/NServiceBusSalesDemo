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
    public abstract class BookingShippingPolicyTestsBase
    {
        protected FedExOrder CreateFedEx(int orderId)
        {
            return new FedExOrder() { OrderId = orderId };
        }

        protected UPSOrder CreateUPSOrder(int orderId)
        {
            return new UPSOrder() { OrderId = orderId };
        }

        protected BookShipping CreateBooking(int orderId)
        {
            return new BookShipping() { OrderId = 1 };
        }
        
    }

    [TestFixture]
    public class When_FedEx_Confirms_Booking : BookingShippingPolicyTestsBase
    {
        [Test]
        public void The_Booking_Is_Confirmed()
        {
            var orderId = 1;
            Test.Initialize();
            Test.Saga<BookShippingPolicy>()
                .ExpectSend<ShipmentBooked>(f => f.OrderId == orderId)
                .When(s => s.Handle(CreateFedEx(orderId)));
        }

        [Test]
        public void The_Booking_Is_Not_Confirmed_If_Already_Confirmed()
        {
            var orderId = 1;
            Test.Initialize();
            Test.Saga<BookShippingPolicy>()
                .When(s => s.Handle(CreateUPSOrder(orderId)))
                .ExpectNotSend<ShipmentBooked>(s => true)
                .ExpectSend<CancelFedEx>(y => y.OrderId == orderId)
                .When(s => s.Handle(CreateFedEx(orderId)));
        }
    }

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
