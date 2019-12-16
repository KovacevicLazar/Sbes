﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using Client.Exceptions;
using System.Security.Cryptography;

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
		public Tuple<string, string> Connect(string username, string password, string service)
		{
			try
			{
				if (Authenticate(username, password))
				{
					Tuple<string, string> serviceEndpoint = factory.SendServiceRequest(service, username);


                    string encriptSecretKey = serviceEndpoint.Item2;
                    string secretKey= Decript(encriptSecretKey, username);

                    serviceEndpoint.Item2.Replace(serviceEndpoint.Item2, secretKey);


                    if (serviceEndpoint != null)
					{
						Console.WriteLine($"'{serviceEndpoint}' pronadjen");
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

		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
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
