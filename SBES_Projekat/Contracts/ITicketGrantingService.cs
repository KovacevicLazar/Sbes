using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	public interface ITicketGrantingService
	{
		bool ServiceExists(string serviceName);
		Tuple<string, string> GetServiceEndpointAndSecretKey(string serviceName, string hashedClientPassword);
		string Encript(string input, string key);
		string Decript(string input, string key);
		string GenerateSecretKey();
		void RegisterService(string serviceName, string servicePassword, string port);
		void SignOutService(string serviceName);
	}
}
