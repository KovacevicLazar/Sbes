using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace DC
{
    class Program
    {
        static void Main(string[] args)
		{
			string address = "net.tcp://localhost:9998/DomenController";
			NetTcpBinding binding = new NetTcpBinding();

			ServiceHost host = new ServiceHost(typeof(DomenController));
			host.AddServiceEndpoint(typeof(IClientConnection), binding, address);

            host.Open();
			Console.WriteLine("Domain controller is opened. Press <enter> to finish...");
			Console.ReadLine();

			host.Close();
		}
    }
}
