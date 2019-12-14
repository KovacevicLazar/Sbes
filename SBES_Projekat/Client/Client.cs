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
	public class Client
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

			string username = "DESKTOP-IJMHSLM\\Luka";

			string address = "net.tcp://localhost:";
			string servicePort = "9999";
			string authenticationPort = "9998";
			string authenticationService = "DomenController";
			string service = "WCFService";

			Tuple<string, string> serviceEndpointAndKey;
			string secretKey = null;


			using (WCFClientAuthenticator authenticator = new WCFClientAuthenticator(binding, new EndpointAddress(new Uri(address + authenticationPort + "/" + authenticationService))))
			{
				///	Slanje ID klijenta, PASSWORD klijenta
				serviceEndpointAndKey = authenticator.Connect(username, "password", service);
				/// TODO: dekriptovanje tajnog kljuca
				/// secretKey = DecryptSecretKey(serviceEndpointAndKey);
				while (serviceEndpointAndKey == null) System.Threading.Thread.Sleep(100);
			}
			
			///	TODO: Komunikacija sa servisom
                using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(serviceEndpointAndKey.Item1))))
                {
                    proxy.SendMessage("msg", secretKey);
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
