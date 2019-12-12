using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using Manager;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using Client.Exceptions;

namespace Client
{
	public class WCFClientAuthenticator : ChannelFactory<IClientConnection>, IDisposable
	{
		IClientConnection factory;

		public WCFClientAuthenticator(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			factory = CreateChannel();
			#region Rad sa sertifikatima
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			//string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			//this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
			//this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
			//this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			///// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			//this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
			#endregion
		}
		public string Connect(string username, string password, string service)
		{
			try
			{
				if (Authenticate(username, password))
				{
					string serviceEndpoint;
					if ((serviceEndpoint = SendSeviceRequest(service)) != null)
					{
						Console.WriteLine($"Konekcija sa '{serviceEndpoint}' uspesno ostvarena");
						return serviceEndpoint;
					}
					else
					{
						Console.WriteLine($"Servis '{service}' nije pronadjen.");
						return null;
					}
				}
				else
				{
					Console.WriteLine("Invalid password.");
					return null;
				}
			}
			catch (NoSuchUserException e)
			{
				Console.WriteLine(e.Message);
				return null; 
			}
		}
		/// <summary>
		/// Autentifikuje korisnika na osnovu prosledjenih kredencijala
		/// </summary>
		/// <param name="username">Windows ID usera</param>
		/// <param name="password"></param>
		/// <returns>Da li je korisnik validan; Exception ako ne postoji</returns>
		private bool Authenticate(string username, string password)
		{
			bool ret = false;
			try
			{
				ret = factory.ValidateUser(username, password);
			}
			catch (Exception e)
			{
				Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
			}
			return ret;
		}
		/// <summary>
		/// Salje zahtev za endpoint adresu ka trazenom servisu
		/// </summary>
		/// <param name="service">Naziv servisa</param>
		/// <returns>Endpoint addr servisa ako postoji; null ako ne postoji</returns>
		private string SendSeviceRequest(string service)
		{
			return factory.SendServiceRequest(service);
		}

		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}


        //public void TestCommunication()
        //{
        //	try
        //	{
        //		factory.TestCommunication();
        //	}
        //	catch (Exception e)
        //	{
        //		Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
        //	}
        //}
    }
}
