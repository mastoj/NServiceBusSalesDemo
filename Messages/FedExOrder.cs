namespace Messages
{
    public class FedExOrder : BookShippingBase
    {
    }
    public class UPSOrder : BookShippingBase
    {
    }

    public class ShipmentBooked : BookShipping
    {}

    public class CancelUPS : BookShipping
    {
        
    }
    public class CancelFedEx : BookShipping
    {}
    public abstract class BookShippingBase
    {
        public int OrderId { get; set; }
    }
}