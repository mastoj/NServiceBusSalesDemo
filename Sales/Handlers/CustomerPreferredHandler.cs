using Messages;
using NServiceBus;
using log4net;

namespace Sales.Handlers
{
    public class CustomerPreferredHandler : IHandleMessages<CustomerMadePreferred>
    {
        public void Handle(CustomerMadePreferred message)
        {
            LogManager.GetLogger("CustomerPreferredHandler").Info("Fancy trombone, we have a preferred customer" + message.CustomerId);
        }
    }
}