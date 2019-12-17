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
	public class TicketGrantingService : ITicketGrantingService,  IServiceConnection
    {
        //<ipAddr,hostName>
        private static Dictionary<string, string> dnsTable;
        //<hostName,ServiceID>
        private static Dictionary<string, ServiceEntity> activeServices;
		// <client, secretKey>
		private static Dictionary<string, string> createdSecretKeys;


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
				string hostEndpoint = "net.tcp://localhost:" + activeServices[serviceName].port + "/" + activeServices[serviceName].hostName;
				string secretKey = GenerateSecretKey();
				string clientEncriptedSecretKey = Encript(secretKey, hashedClientPassword);
				string serviceEncriptedSecretKey = Encript(secretKey, activeServices[serviceName].servicePassword);

                using (SendSecretKey proxy= new SendSecretKey(new NetTcpBinding(), new EndpointAddress(new Uri("net.tcp://localhost:9990/Connector"))))
                {
                    proxy.SendEncriptedSecretKey(serviceEncriptedSecretKey);
                }
				return new Tuple<string, string>(hostEndpoint, clientEncriptedSecretKey);
			}
			else
			{
				return null;
			}
		}
		

		public string GenerateSecretKey()
		{
            //SymmetricAlgorithm symmAlgorithm = null;                                                             //
            //TripleDESCryptoServiceProvider.Create();                                                            // Ovako moramo praviti tajni kljuc jer cemo koristiti 3des pri komunikaciji izmedji klijenta i servera
            //return symmAlgorithm == null ? String.Empty : ASCIIEncoding.ASCII.GetString(symmAlgorithm.Key);      //

            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            return key;
        }
		
		public bool ServiceExists(string serviceName)
		{
			return activeServices.ContainsKey(serviceName);
		}

		public string Encript(string input, string key)
		{
            byte[] byteKey = StringToByteArray(key);
            byte[] buffer = new byte[4] { 0, 0, 0, 0 };

            byte[] KeyFor3DES = new byte[byteKey.Length + buffer.Length];
            System.Buffer.BlockCopy(byteKey, 0, KeyFor3DES, 0, byteKey.Length);
            System.Buffer.BlockCopy(buffer, 0, KeyFor3DES, byteKey.Length, buffer.Length);

            TripleDESCryptoServiceProvider tripleDesCrypto = new TripleDESCryptoServiceProvider
            {
                Key = KeyFor3DES,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            };

            byte[] rawInput = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(tripleDesCrypto.CreateEncryptor().TransformFinalBlock(rawInput, 0, rawInput.Length));
        }

        // Convert the hexadecimal string to byte array
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        //visak?=>

        //public bool ValidateUser(string username, string password)
        //{
        //	throw new NotImplementedException();
        //}

        //public Tuple<string, string> SendServiceRequest(string service, string username)
        //{
        //	throw new NotImplementedException();
        //}

        //<-visak?

        public void SignOutService(string hostName)
        {
        	activeServices.Remove(hostName);
            Console.WriteLine("Servis {0} is deactivated.", hostName);
        }

        public void RegisterService(string IPAddr, string hostName, string port, string hashPassword)
        {
        	activeServices.Add(hostName, new ServiceEntity(IPAddr, hostName, port, hashPassword));
            dnsTable.Add(IPAddr, hostName);
            Console.WriteLine("Servis {0} is activated.", hostName);
        }
    }
}
