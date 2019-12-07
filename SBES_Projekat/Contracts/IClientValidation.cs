using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Contracts
{
	[ServiceContract]
	public interface IClientValidation
	{
		[OperationContract]
		bool ValidateUser(string username, string password);

        [OperationContract]
        bool ServiceExist(string hostName);


        [OperationContract]
        bool serviceRegistration(string ipAddr, string hostName, EndpointIdentity ServiceID);
    }
}
