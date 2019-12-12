using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contracts;

namespace DC
{
	public class TicketGrantingService : ITicketGrantingService
	{
		//<hostName,ServiceID>
		private static Dictionary<string, ServiceEntity> activeServices;

		public TicketGrantingService()
		{
			activeServices = new Dictionary<string, ServiceEntity>();
		}

		public string GetServiceEndpoint(string serviceName)
		{
			if (ServiceExists(serviceName))
			{
				return "net.tcp://localhost:" + activeServices[serviceName].port + "/" + activeServices[serviceName].serviceName;
			}
			else
			{
				return null;
			}
		}

		public string GenerateSecretKey(string encriptionKey)
		{
			throw new NotImplementedException();
		}
		
		public bool ServiceExists(string serviceName)
		{
			return activeServices.ContainsKey(serviceName);
		}
	}
}
