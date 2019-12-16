using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Server
{
	public class Connector : ITicketGrantingService
	{
        public string Decript(string input, string key)
        {
            throw new NotImplementedException();
        }

        public string Encript(string input, string key)
        {
            throw new NotImplementedException();
        }

        public string GenerateSecretKey()
        {
            throw new NotImplementedException();
        }

        public Tuple<string, string> GetServiceEndpointAndSecretKey(string serviceName, string hashedClientPassword)
        {
            throw new NotImplementedException();
        }

        //public void RegisterService(string serviceName, string servicePassword, string port)
        //{
        //    throw new NotImplementedException();
        //}

        public void SendEncriptedSecretKey(string key)
		{
			SecretKey.secretKey = key;
		}

        public bool ServiceExists(string serviceName)
        {
            throw new NotImplementedException();
        }

        //public void SignOutService(string serviceName)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
