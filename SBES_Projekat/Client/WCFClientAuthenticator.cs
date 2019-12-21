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
		public Tuple<string, string> Connect(string username, string password, string service)
		{
			try
			{
				if (Authenticate(username, password))
				{
					//using (EventLog log = new EventLog("Application"))
					//{
					//	log.Source = "Application";
					//	log.WriteEntry($"Client authentication successful.", EventLogEntryType.SuccessAudit, 102, 4);
					//}

					Tuple<string, string> serviceEndpoint = factory.SendServiceRequest(service, username);
                    if (serviceEndpoint == null )
					{
						//using (EventLog eventLog = new EventLog("Application"))
						//{
						//	eventLog.Source = "Application";
						//	eventLog.WriteEntry($"Failed to get service '{service}' endpoint.", EventLogEntryType.FailureAudit, 404, 4);
						//}
						//Console.WriteLine("Trazeni servis nije aktivan.");
                        return null;
                    }
                    else
					{
						//using (EventLog log = new EventLog("Application"))
						//{
						//	log.Source = "Application";
						//	log.WriteEntry($"Service endpoint '{serviceEndpoint.Item1}' successfully found.", EventLogEntryType.SuccessAudit, 100, 4);
						//}
						//Console.WriteLine($"{serviceEndpoint.Item1} pronadjen");
						return serviceEndpoint;
					}
				}
				else
				{
					//using (EventLog log = new EventLog("Application"))
					//{
					//	log.Source = "Application";
					//	log.WriteEntry($"Invalid password. Service authentication failed.", EventLogEntryType.FailureAudit, 102, 4);
					//}
					//Console.WriteLine("Invalid password.");
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
