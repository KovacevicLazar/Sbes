using System;

namespace DC
{
	internal class ServiceEntity
	{
		public string ipAddress;
		public string hostName;
		public string port;
        public string servicePassword;

        public ServiceEntity(string ipAddress, string hostName, string port, string servicePassword)
		{
			this.ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
			this.hostName = hostName ?? throw new ArgumentNullException(nameof(hostName));
			this.port = port ?? throw new ArgumentNullException(nameof(port));
            this.servicePassword=servicePassword ?? throw new ArgumentNullException(nameof(servicePassword));
        }
	}
}