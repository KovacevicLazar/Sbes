using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
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

                string haspass = CreateSHA1("password");
                Console.WriteLine(haspass);
                serviceEndpointAndKey = authenticator.Connect(username, haspass, service);
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
