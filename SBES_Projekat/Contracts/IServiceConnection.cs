using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
	public interface IServiceConnection
	{
        [OperationContract]
		void RegisterService(string IPAddr, string hostName, string port);
        [OperationContract]
		void SignOutService(string hostName);
	}
}
