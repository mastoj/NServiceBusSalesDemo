using Messages;

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
}