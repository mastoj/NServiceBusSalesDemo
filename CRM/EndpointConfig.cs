using log4net;

namespace CRM 
{
    using NServiceBus;

	/*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://nservicebus.com/GenericHost.aspx
	*/
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, IWantToRunAtStartup, IWantCustomInitialization
    {
	    public IBus Bus { get; set; }

        public void Run()
        {
            LogManager.GetLogger("EndpointConfig").Info("CRM Running");
        }

	    public void Stop()
	    {
            LogManager.GetLogger("EndpointConfig").Info("CRM stopping");
	    }

        public void Init()
        {
            Configure.With().DefaultBuilder()
                     .JsonSerializer();
        }
    }
}