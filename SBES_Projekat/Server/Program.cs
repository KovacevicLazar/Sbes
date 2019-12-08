using Contracts;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{

			NetTcpBinding binding = new NetTcpBinding();

            string address = "net.tcp://";
            string serviceIPAddr = "localhost:9999";
            string service = "WCFService";

            ServiceHost host = new ServiceHost(typeof(WCFService));
			host.AddServiceEndpoint(typeof(IWCFContracts), binding, address + serviceIPAddr  + "/" + service);

			host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

			host.Open();
			Console.WriteLine("WCFService is opened. Press <enter> to finish...");

            using (WCFServiceRegister proxy = new WCFServiceRegister(binding, new EndpointAddress(new Uri("net.tcp://localhost:9998/DomenController"))))
            {
                //EndpointAddress endpointAddress= new EndpointAddress(new Uri("net.tcp://localhost:9999/WCFService"), EndpointIdentity.CreateDnsIdentity("WCFService")); //DA LI OVDE PRAVIMO IDENTITI ILI NA TGS, AKO OVDE, KAKO GA POSLATI NA TGS
                proxy.SendIpAddrAndHostName(serviceIPAddr, service);
            }

            Console.ReadLine();

			//
			//Ovde bi trebalo javiti TGS-u da servis vise nije aktivan
			//

			host.Close();

		}
	}
}
