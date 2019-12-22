using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using Client.Exceptions;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Client
{
	public class WCFClientAuthenticator : ChannelFactory<IClientConnection>, IDisposable
	{
		IClientConnection factory;

		public WCFClientAuthenticator(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			factory = CreateChannel();
		}
		/// <summary>
		/// Trazi registrovani servis
		/// </summary>
		/// <param name="service"></param>
		/// <param name="username"></param>
		/// <returns>servis endpoint; tajni kljuc</returns>
		public Tuple<string, string> ServiceRequest(string service, string username)
		{
			return factory.SendServiceRequest(service, username);
		}
       
        /// <summary>
        /// Autentifikuje korisnika na osnovu prosledjenih kredencijala
        /// </summary>
        /// <param name="username">Windows ID usera</param>
        /// <param name="password"></param>
        /// <returns>Da li je korisnik validan; Exception ako ne postoji</returns>
        public bool Authenticate(string username, string password)
		{
			bool ret = false;
			try
			{
				ret = factory.ValidateUser(username, password);
			}
			catch (Exception e)
			{
				Console.WriteLine("[Authenticate] No such user found.");
			}
			return ret;
		}

		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}
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
