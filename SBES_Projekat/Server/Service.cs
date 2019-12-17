﻿using Contracts;
using System;
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
			Console.WriteLine("WCFService is opened.");

			/// Registrovanje servisa na TGS
            using (WCFServiceRegister proxy = new WCFServiceRegister(binding, new EndpointAddress(new Uri("net.tcp://localhost:9997/ServiceConnection"))))
            {
                string hashPass = CreateSHA1("password");
                proxy.Registration(serviceIPAddr+clientPort, clientService, id.Name , hashPass);
                                   // local host +port je IP adresa, tako su nam rekli na vezbama
            }


			/// Otvaranje servisa za prijem tajnog kljuca
			ServiceHost connectionHost = new ServiceHost(typeof(Connection));
			connectionHost.AddServiceEndpoint(typeof(IServiceKeyHandler), binding, connectionEndpoint);
			connectionHost.Open();
			Console.WriteLine("Connector service is opened.");

			while (SecretKey.secretKey == null) System.Threading.Thread.Sleep(100);

			Console.WriteLine("Client connected with secret key: " + SecretKey.secretKey);


            Console.WriteLine("Press <enter> to finish...");
			Console.ReadLine();

            //
            //Ovde bi trebalo javiti TGS-u da servis vise nije aktivan
            //
            using (WCFServiceRegister proxy = new WCFServiceRegister(binding, new EndpointAddress(new Uri("net.tcp://localhost:9997/ServiceConnection"))))
            {
                //EndpointAddress endpointAddress= new EndpointAddress(new Uri("net.tcp://localhost:9999/WCFService"), EndpointIdentity.CreateDnsIdentity("WCFService")); //DA LI OVDE PRAVIMO IDENTITI ILI NA TGS, AKO OVDE, KAKO GA POSLATI NA TGS
                proxy.serviceSingOut(clientService);
            }

            clientHost.Close();
			connectionHost.Close();
		}

        public static string CreateSHA1(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.SHA1 md5 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                int i;
                StringBuilder sb = new StringBuilder();
                for (i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                //byte[] buffer = new byte[4] { 0, 0, 0, 0 };
                //sb.Append(Encoding.UTF8.GetString(buffer));
                Console.WriteLine(sb.ToString());
                return sb.ToString();
            }
        }
    }
}
