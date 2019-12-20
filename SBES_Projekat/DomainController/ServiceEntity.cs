using System;
using System.ServiceModel;

namespace DC
{
	internal class ServiceEntity
	{
		public string ipAddress;
		public string hostName;
		public string port;
        public string servicePassword;
        public EndpointIdentity endpointIdentity; //IdentitetServisa

        public ServiceEntity(string ipAddress, string hostName, string port, string servicePassword, EndpointIdentity endpointIdentity)
		{
			this.ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
			this.hostName = hostName ?? throw new ArgumentNullException(nameof(hostName));
			this.port = port ?? throw new ArgumentNullException(nameof(port));
            this.servicePassword=servicePassword ?? throw new ArgumentNullException(nameof(servicePassword));
            this.endpointIdentity = endpointIdentity;
        }
	}
}