using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using Manager;
using System.Security.Principal;
using System.Threading;

namespace Client
{
	public class Program
	{
		static void Main(string[] args)
		{
			// Debugger.Launch();
			//DESKTOP-IJMHSLM\Luka
			WindowsIdentity id = WindowsIdentity.GetCurrent();
			Console.WriteLine(id.Name);

			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Mode = SecurityMode.Transport;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

			string address = "net.tcp://localhost:";
			string servicePort = "9999";
			string authenticationPort = "9998";
			string authenticationService = "DomenController";
			string service = "WCFService";



			using (WCFClientAuthenticator authenticator = new WCFClientAuthenticator(binding, new EndpointAddress(new Uri(address + authenticationPort + "/" + authenticationService))))
			{
				if(authenticator.Authenticate(id.Name, "password"))
				{
					Console.WriteLine(id.Name + " successfully logged in.");
				}
				else
				{
					Console.WriteLine("Invalid password.");
				}
			}

			using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(address + servicePort + "/" + service))))
			{
				proxy.SendMessage("msg", new byte[] { 1, 2, 3 });
			}

				/// Create a signature using SHA1 hash algorithm
				//byte[] signature = DigitalSignature.Create();
				//proxy.SendMessage();
				


				/// For the same message, create a signature based on the "wrongCertCN"
				//X509Certificate2 wrongSignCert = null;

				/// Create a signature using SHA1 hash algorithm
				//byte[] signature1 = DigitalSignature.Create();
				//proxy.SendMessage();
				
			

			Console.ReadLine();
		
		}
	}
}
