using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contracts;

namespace DC
{
	public class TicketGrantingService : ITicketGrantingService
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
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Service '{serviceName}' not found.", EventLogEntryType.FailureAudit, 204, 4);
				}
				return null;
			}
		}
		

		public string GenerateSecretKey()
		{
            // Ovako moramo praviti tajni kljuc jer cemo koristiti 3des pri komunikaciji izmedji klijenta i servera
            SymmetricAlgorithm symmAlgorithm = null;                                                             
            symmAlgorithm = TripleDESCryptoServiceProvider.Create();                                             
            return symmAlgorithm == null ? String.Empty : ASCIIEncoding.ASCII.GetString(symmAlgorithm.Key);      
        }

        public bool ServiceExists(string serviceName)
		{
			return activeServices.ContainsKey(serviceName);
		}

		public string Encript(string input, string key)
		{
            byte[] rawInput = Encoding.UTF8.GetBytes(input); //pravimo niz bajtova od unetog stringa
            byte[] encrypted; //pomocni niz u koga enkriptujemo

            //pravimo kljuc za 3DES
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

            //Mora sa ovim algoritmom, sa starim radi samo u  ECB modu
            tripleDesCrypto.GenerateIV();
            ICryptoTransform tripleDesEncrypt = tripleDesCrypto.CreateEncryptor();

            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(mStream, tripleDesEncrypt, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(rawInput, 0, rawInput.Length);
                    encrypted = tripleDesCrypto.IV.Concat(mStream.ToArray()).ToArray();
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        // Convert the hexadecimal string to byte array
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public void SignOutService(string hostName)
		{
			activeServices.Remove(hostName);
            Console.WriteLine("Servis {0} is deactivated.", hostName);
			using (EventLog log = new EventLog("Application"))
			{
				log.Source = "Application";
				log.WriteEntry($"Service '{hostName}' signed out.", EventLogEntryType.Information, 205, 4);
			}
        }

        public void RegisterService(string IPAddr, string hostName, string port, string hashPassword)
        {
			if (ServiceExists(hostName))
			{
				Console.WriteLine($"Service {hostName} already exists.");
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Service '{hostName}' could not be registered because it already exists.", EventLogEntryType.FailureAudit, 206, 4);
				}
				throw new Exception("Service already registered.");
			}

            EndpointAddress endpointAdress = new EndpointAddress(new Uri("net.tcp://localhost:" + port +  "/" + hostName), EndpointIdentity.CreateUpnIdentity("admin@w7ent"));
            activeServices.Add(hostName, new ServiceEntity(IPAddr, hostName, port, hashPassword,endpointAdress.Identity));
            dnsTable.Add(IPAddr, hostName);
            Console.WriteLine("Servis {0} is activated.", hostName);
			using (EventLog log = new EventLog("Application"))
			{
				log.Source = "Application";
				log.WriteEntry($"Service '{hostName}' registered successfully.", EventLogEntryType.SuccessAudit, 206, 4);
			}
		}
    }
}
