using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
	public interface ITicketGrantingService
	{
        [OperationContract]
		bool ServiceExists(string serviceName);
        [OperationContract]
        Tuple<string, string> GetServiceEndpointAndSecretKey(string serviceName, string hashedClientPassword);
        [OperationContract]
        string Encript(string input, string key);
        [OperationContract]
        string Decript(string input, string key);
        [OperationContract]
        string GenerateSecretKey();
        //[OperationContract]
        //void RegisterService(string serviceName, string servicePassword, string port);
        //[OperationContract]
        //void SignOutService(string serviceName);
	}
}
