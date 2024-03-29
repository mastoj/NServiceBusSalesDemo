using log4net;

namespace Shipping 
{
    using NServiceBus;

	/*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://nservicebus.com/GenericHost.aspx
	*/
	public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
        public void Run()
        {
            LogManager.GetLogger("EndpointConfig").Info("Shipping running");
        }

        public void Stop()
        {
            LogManager.GetLogger("EndpointConfig").Info("Shipping stopped");
        }

        public void Init()
        {
            Configure.With().DefaultBuilder()
                     .JsonSerializer().UnicastBus().DoNotAutoSubscribe();
        }
    }
}