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
	public class TicketGrantingService : ChannelFactory<IServiceKeyHandler>, ITicketGrantingService
	{
        //<ipAddr,hostName>
        private static Dictionary<string, string> dnsTable;

        //<hostName,ServiceID>
        private static Dictionary<string, ServiceEntity> activeServices;
		// <client, secretKey>
		private static Dictionary<string, string> createdSecretKeys;

		IServiceKeyHandler factory;

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

		public void SendEncriptedSecretKey(string key)
		{
			throw new NotImplementedException();
		}

		

		public bool ValidateUser(string username, string password)
		{
			throw new NotImplementedException();
		}

		public Tuple<string, string> SendServiceRequest(string service, string username)
		{
			throw new NotImplementedException();
		}


        public void SignOutService(string hostName)
        {
        	activeServices.Remove(hostName);
            Console.WriteLine("Seris {0} is closed.", hostName);
        }

        public void RegisterService(string IPAddr, string hostName, string port)
        {
        	activeServices.Add(hostName, new ServiceEntity(IPAddr, hostName, port));
            dnsTable.Add(IPAddr, hostName);
            Console.WriteLine("Seris {0} is opened.",hostName);
        }

    }
}
