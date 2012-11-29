using System;
using System.Threading;
using Messages;
using NServiceBus;
using log4net;

namespace UPSProxy.Handlers
{
    public class UPSOrderHandler : IHandleMessages<UPSOrder>
    {
        public IBus Bus { get; set; }
        public void Handle(UPSOrder message)
        {
            if (message.OrderId % 3 == 0)
            {
                Console.WriteLine("This might take a while");
                Thread.Sleep(22000);
            }
            else
            {
                Console.WriteLine("This should go fast");
            }
            Bus.Reply(message);
        }
    }
}
