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
	public class TicketGrantingService : ChannelFactory<ITicketGrantingService>, ITicketGrantingService
	{
		//<hostName,ServiceID>
		private static Dictionary<string, ServiceEntity> activeServices;
		// <client, secretKey>
		private static Dictionary<string, string> createdSecretKeys;

		ITicketGrantingService factory;

		public TicketGrantingService()
		{
			if (activeServices == null) activeServices = new Dictionary<string, ServiceEntity>();
			if (createdSecretKeys == null) createdSecretKeys = new Dictionary<string, string>();
		}

		public Tuple<string, string> GetServiceEndpointAndSecretKey(string serviceName, string hashedClientPassword)
		{
			if (ServiceExists(serviceName))
			{
				string hostEndpoint = "net.tcp://localhost:" + activeServices[serviceName].port + "/" + activeServices[serviceName].serviceName;
				string secretKey = GenerateSecretKey();
				string clientEncriptedSecretKey = Encript(secretKey, hashedClientPassword);
				string serviceEncriptedSecretKey = Encript(secretKey, activeServices[serviceName].servicePassword);

				factory.SendEncriptedSecretKey(serviceEncriptedSecretKey);
				return new Tuple<string, string>(hostEndpoint, clientEncriptedSecretKey);
			}
			else
			{
				return null;
			}
		}
		public void RegisterService(string serviceName, string servicePassword, string port)
		{
			activeServices.Add(serviceName, new ServiceEntity(serviceName, servicePassword, port));
		}

		public string GenerateSecretKey()
		{
			// TODO: Implementirati generisanje tajnog kljuca
			throw new NotImplementedException();
		}
		
		public bool ServiceExists(string serviceName)
		{
			return activeServices.ContainsKey(serviceName);
		}

		public string Encript(string input, string key)
		{
			//TODO
			throw new NotImplementedException();
		}

		public string Decript(string input, string key)
		{
			//TODO
			throw new NotImplementedException();
		}

	}
}
