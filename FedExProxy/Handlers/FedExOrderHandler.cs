using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using log4net;

namespace FedExProxy.Handlers
{
    public class FedExOrderHandler : IHandleMessages<FedExOrder>
    {
        public IBus Bus { get; set; }
        public void Handle(FedExOrder message)
        {
            if (message.OrderId%6 == 0)
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
