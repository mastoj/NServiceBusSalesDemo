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
            Console.WriteLine("Place an order");
            var readLine = Console.ReadLine();
            while (!string.IsNullOrEmpty(readLine))
            {
                var placeOrder = ParseLine(readLine);
                Console.WriteLine("Placing order for customer: {0}, and product: {1}", placeOrder.CustomerId,
                                  placeOrder.ProductId);
                Bus.Send<PlaceOrder>(c =>
                    {
                        c.ProductId = placeOrder.ProductId;
                        c.CustomerId = placeOrder.CustomerId;
                    });
                Console.WriteLine("Order placed");
                Console.WriteLine("Place an order");
                readLine = Console.ReadLine();
            }
        }

        private static PlaceOrder ParseLine(string readLine)
        {
            var input = readLine.Split(',').Select(int.Parse).ToList();
            return new PlaceOrder() {CustomerId = input[0], ProductId = input[1]};
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
