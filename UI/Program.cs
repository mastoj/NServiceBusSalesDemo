using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Installation.Environments;

namespace UI
{
    class Program
    {
        public static IBus Bus { get; set; }
        static void Main(string[] args)
        {
            ConfigureBus();
            Run();
        }

        private static void Run()
        {
            Console.WriteLine("Do something, prefix with b to book shipment");
            var readLine = Console.ReadLine();
            while (!string.IsNullOrEmpty(readLine))
            {
                var placeOrder = ParseLine(readLine);
                Bus.Send(placeOrder);
                Console.WriteLine("Confirmed");
                Console.WriteLine("Do something, prefix with b to book shipment");
                readLine = Console.ReadLine();
            }
        }

        private static object ParseLine(string readLine)
        {
            var firstChar = readLine[0];
            if (firstChar == 'b')
            {
                return ParseBookShipping(readLine.Substring(1));
            }
            return ParsePlaceOrder(readLine);
        }

        private static object ParsePlaceOrder(string substring)
        {
            var input = substring.Split(',').Select(int.Parse).ToList();
            var placeOrder = new PlaceOrder() { CustomerId = input[0], ProductId = input[1] };
            Console.WriteLine("Placing order for customer: {0}, and product: {1}", placeOrder.CustomerId,
                  placeOrder.ProductId);
            return placeOrder;
        }

        private static object ParseBookShipping(string substring)
        {
            var booking = new BookShipping() {OrderId = int.Parse(substring)};
            Console.WriteLine("Booking shipment for order: " + booking.OrderId);
            return booking;
        }

        private static void ConfigureBus()
        {
            Bus = Configure.With()
                           .Log4Net()
                           .DefaultBuilder()
                           .JsonSerializer()
                //.XmlSerializer("http://acme.com")
                           .MsmqTransport()
                           .UnicastBus()
                           .DoNotAutoSubscribe()
                           .CreateBus()
                           .Start(() => Configure.Instance.ForInstallationOn<Windows>().Install());
        }
    }
}
