using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Contracts
{
	public interface ITicketGrantingService
	{
		bool ServiceExists(string serviceName);
        Tuple<string, string> GetServiceEndpointAndSecretKey(string serviceName, string hashedClientPassword);
	}
}
