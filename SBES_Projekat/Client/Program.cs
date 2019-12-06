using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using Manager;

namespace Client
{
	public class Program
	{
		static void Main(string[] args)
		{

			/// Define the expected service certificate. It is required to establish cmmunication using certificates.
			string srvCertCN = "wcfservice";

			/// Define the expected certificate for signing ("<username>_sign" is the expected subject name).
			/// .NET WindowsIdentity class provides information about Windows user running the given process
			string signCertCN = String.Empty;

			/// Define subjectName for certificate used for signing which is not as expected by the service
			string wrongCertCN = String.Empty;

			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

			/// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
			EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9999/Receiver"),
									  new X509CertificateEndpointIdentity(srvCert));

			using (WCFClient proxy = new WCFClient(binding, address))
			{
				/// 1. Communication test
				proxy.TestCommunication();
				Console.WriteLine("TestCommunication() finished. Press <enter> to continue ...");
				Console.ReadLine();

				/// 2. Digital Signing test				
				string message = "Exercise 02";

				/// Create a signature based on the "signCertCN"
				X509Certificate2 signCert = null;

				/// Create a signature using SHA1 hash algorithm
				//byte[] signature = DigitalSignature.Create();
				//proxy.SendMessage();

				Console.WriteLine("SendMessage() using {0} certificate finished. Press <enter> to continue ...", signCertCN);
				Console.ReadLine();


				/// For the same message, create a signature based on the "wrongCertCN"
				X509Certificate2 wrongSignCert = null;

				/// Create a signature using SHA1 hash algorithm
				//byte[] signature1 = DigitalSignature.Create();
				//proxy.SendMessage();

				Console.WriteLine("SendMessage() using {0} certificate finished. Press <enter> to continue ...", wrongCertCN);
				Console.ReadLine();
			}
		}
	}
}
