using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using Manager;

namespace Client
{
	public class Program
	{
		static void Main(string[] args)
		{

			// Debugger.Launch();

			NetTcpBinding binding = new NetTcpBinding();
			string address = "net.tcp://localhost:9999/WCFService";

			using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(address))))
			{
				proxy.SendMessage("msg", new byte[] { 1, 2, 3});
			}

			Console.ReadLine();
		
		}
	}
}
