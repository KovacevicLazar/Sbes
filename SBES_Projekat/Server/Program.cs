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
			string address = "net.tcp://localhost:9999/WCFService";

			ServiceHost host = new ServiceHost(typeof(WCFService));
			host.AddServiceEndpoint(typeof(IWCFContracts), binding, address);

			host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

			host.Open();
			Console.WriteLine("WCFService is opened. Press <enter> to finish...");

			//
			//Potrebno je prijaviti servis na TGS, jer je sada aktivan
			//

			Console.ReadLine();

			//
			//Ovde bi trebalo javiti TGS-u da servis vise nije aktivan
			//

			host.Close();

		}
	}
}
