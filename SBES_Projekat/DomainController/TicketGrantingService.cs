using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            if (dnsTable == null) dnsTable = new Dictionary<string, string>();
		}

		public Tuple<string, string> GetServiceEndpointAndSecretKey(string serviceName, string hashedClientPassword)
		{
			if (ServiceExists(serviceName))
			{
				string hostEndpoint = "net.tcp://localhost:" + activeServices[serviceName].port + "/" + activeServices[serviceName].serviceName;
				string secretKey = GenerateSecretKey();
				string clientEncriptedSecretKey = Encript(secretKey, hashedClientPassword);
				string serviceEncriptedSecretKey = Encript(secretKey, activeServices[serviceName].servicePassword);

                string seckey = Decript(clientEncriptedSecretKey, hashedClientPassword); // nesto nije u redu

				//factory.SendEncriptedSecretKey(serviceEncriptedSecretKey);
				return new Tuple<string, string>(hostEndpoint, clientEncriptedSecretKey);
			}
			else
			{
				return null;
			}
		}
		

		public string GenerateSecretKey()
		{
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            return key;

        }
		
		public bool ServiceExists(string serviceName)
		{
			return activeServices.ContainsKey(serviceName);
		}

		public string Encript(string input, string key)
		{
            //TODO
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.CBC; //ECB, CFB
            byteBuff = Encoding.UTF8.GetBytes(input);

            string encoded = Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return encoded;
        }

		public string Decript(string input, string key)
		{
            //TODO

            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.CBC; //ECB, CFB
            byteBuff = Convert.FromBase64String(input);

            string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return plaintext;
        }

        public void SendEncriptedSecretKey(string key)
        {
            //throw new NotImplementedException();


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
            Console.WriteLine("Servis {0} is closed.", hostName);
        }

        public void RegisterService(string IPAddr, string hostName, string port)
        {
        	activeServices.Add(hostName, new ServiceEntity(IPAddr, hostName, port));
            dnsTable.Add(IPAddr, hostName);
            Console.WriteLine("Servis {0} is opened.",hostName);
        }

    }
}
