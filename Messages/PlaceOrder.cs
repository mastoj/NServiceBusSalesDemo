namespace Messages
{
    public class PlaceOrder
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }

        public int OrderId { get; set; }
    }
}