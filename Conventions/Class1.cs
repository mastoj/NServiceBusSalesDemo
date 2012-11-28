using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace Conventions
{
    public class NServiceBusConventions : IWantToRunBeforeConfiguration
    {
        private static List<Type> _eventTypes = new List<Type>()
            {
                typeof(OrderAccepted),
                typeof(CustomerMadePreferred)
            }; 

        public void Init()
        {
            Configure.Instance.DefiningEventsAs(
                (t) => _eventTypes.Contains(t));
        }
    }
}
