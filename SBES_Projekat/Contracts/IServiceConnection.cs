using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	public interface IServiceConnection
	{
		void RegisterService(string serviceName, string servicePassword, string port);
		void SignOutService(string serviceName);
	}
}
