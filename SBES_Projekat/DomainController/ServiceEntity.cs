using System;

namespace DC
{
	internal class ServiceEntity
	{
		public string serviceName;
		public string servicePassword;
		public string port;

		public ServiceEntity(string serviceName, string servicePassword, string port)
		{
			this.serviceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
			this.servicePassword = servicePassword ?? throw new ArgumentNullException(nameof(servicePassword));
			this.port = port ?? throw new ArgumentNullException(nameof(port));
		}
	}
}