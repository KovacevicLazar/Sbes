using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Contracts;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;

namespace Servis
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

			//host.Authorization.ServiceAuthorizationManager = new CustomServiceAuthorizationManager();

			//host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
			//List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
			//policies.Add(new CustomAuthorizationPolicy());
			//host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

			host.Open();
			Console.WriteLine("WCFService is opened. Press <enter> to finish...");
			Console.ReadLine();

			host.Close();
		}
	}
}
