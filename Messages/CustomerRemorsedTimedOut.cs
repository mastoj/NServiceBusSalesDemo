namespace Messages
{
    public class CustomerRemorsedTimedOut : OrderBase
    {
    }
    public class OrderPreliminaryAccepted : OrderBase
    {
    }
    public class OrderCancelled : OrderBase
    {

    }
    public class CancelOrder : OrderBase
    {

    }

    public abstract class OrderBase
    {
        public int OrderId { get; set; }
    }
}