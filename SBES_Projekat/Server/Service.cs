using Contracts;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace Server
{
	class Service
	{
		static void Main(string[] args)
		{
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            NetTcpBinding binding = new NetTcpBinding();
			SecretKey.secretKey = null;

			string serviceIPAddr = "net.tcp://localhost";
			string clientPort = "9999";
			string clientService = "WCFService";
			string connectionPort = "9990";
			string connectionService = "Connector";

			string serviceEndpoint = serviceIPAddr + ":" + clientPort + "/" + clientService;
			string connectionEndpoint = serviceIPAddr + ":" + connectionPort + "/" + connectionService;

			/// Otvaranje klijentskog servisa
            ServiceHost clientHost = new ServiceHost(typeof(WCFService));
			clientHost.AddServiceEndpoint(typeof(IWCFContracts), binding, serviceEndpoint);

			clientHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			clientHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

			clientHost.Open();
			using (EventLog log = new EventLog("Application"))
			{
				log.Source = "Application";
				log.WriteEntry($"Service '{clientService}' opened successfully.", EventLogEntryType.SuccessAudit, 207, 4);
			}
			Console.WriteLine("WCFService is opened.");

            /// Registrovanje servisa na TGS
            using (WCFServiceRegister proxy = new WCFServiceRegister(binding, new EndpointAddress(new Uri("net.tcp://localhost:9997/ServiceConnection"))))
            {
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Service atempting to register.", EventLogEntryType.Information, 206, 4);
				}
                string hashPass = CreateSHA1("password");
                proxy.Registration(serviceIPAddr + clientPort, clientService, clientPort, hashPass);
				//localHost+port=IPAdresa
			}


			/// Otvaranje servisa za prijem tajnog kljuca
			ServiceHost connectionHost = new ServiceHost(typeof(Connection));
			connectionHost.AddServiceEndpoint(typeof(IServiceKeyHandler), binding, connectionEndpoint);
			connectionHost.Open();
			Console.WriteLine("Connector service is opened.");
			using (EventLog log = new EventLog("Application"))
			{
				log.Source = "Application";
				log.WriteEntry($"Connection service '{connectionService}' opened successfully.", EventLogEntryType.SuccessAudit, 209, 4);
			}

			while (SecretKey.secretKey == null) System.Threading.Thread.Sleep(100);

			using (EventLog log = new EventLog("Application"))
			{
				log.Source = "Application";
				log.WriteEntry($"Client connected to service. Secret key for communication recieved.", EventLogEntryType.Information, 208, 4);
			}

			Console.WriteLine("Client connected with secret key: " + SecretKey.secretKey);

            Console.WriteLine("Press <enter> to finish...");
			Console.ReadLine();

           
            using (WCFServiceRegister proxy = new WCFServiceRegister(binding, new EndpointAddress(new Uri("net.tcp://localhost:9997/ServiceConnection"))))
            { 
                proxy.serviceSingOut(clientService);
			}

            clientHost.Close();
			using (EventLog log = new EventLog("Application"))
			{
				log.Source = "Application";
				log.WriteEntry($"Service '{clientService}' closed successfully.", EventLogEntryType.Information, 210, 4);
			}
			connectionHost.Close();
			using (EventLog log = new EventLog("Application"))
			{
				log.Source = "Application";
				log.WriteEntry($"Service '{connectionService}' closed successfully.", EventLogEntryType.Information, 210, 4);
			}
		}

        public static string CreateSHA1(string input)
        {
            // Use input string to calculate SHA1 hash
            using (System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                int i;
                StringBuilder sb = new StringBuilder();
                for (i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
