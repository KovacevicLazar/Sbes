using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace DC
{
	public class DomenController : IClientConnection, IServiceConnection
    {
		AuthenticationController AS = new AuthenticationController();
		TicketGrantingService TGS = new TicketGrantingService();

		public DomenController(AuthenticationController aS, TicketGrantingService tGS)
		{
			AS = aS;
			TGS = tGS;

			if(AS == null)
			{
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Authentication service open failed.", EventLogEntryType.FailureAudit, 200, 4);
				}
				throw new ArgumentNullException("AS was null");
			}
			else
			{
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Authentication service opened successfully.", EventLogEntryType.SuccessAudit, 200, 4);
				}
			}
			if (TGS == null)
			{
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Ticked granting service open failed.", EventLogEntryType.FailureAudit, 201, 4);
				}
				throw new ArgumentNullException("TGS was null");
			}
			else
			{
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Ticket granting service opened successfully.", EventLogEntryType.SuccessAudit, 201, 4);
				}
			}
		}

		public DomenController()
		{
		}

		public bool ValidateUser(string username, string password)
		{
			return AS.Authenticate(username, password);
		}

        /// <summary>
        /// Vraca ime servisa i enkriptovan tajni kljuc
        /// </summary>
        /// <param name="service"></param>
        /// <param name="username"></param>
        /// <returns>Ime servisa i enkriptovan tajni kljuc</returns>
		public Tuple<string, string> SendServiceRequest(string service, string username)
		{
			using (EventLog log = new EventLog("Application"))
			{
				log.Source = "Application";
				log.WriteEntry($"Recieved request for service endpoint and secret key.", EventLogEntryType.Information, 203, 4);
			}
			return TGS.GetServiceEndpointAndSecretKey(service, AS.GetHashedUserPassword(username));
		}


        public bool RegisterService(string IPAddr, string hostName, string port, string hashPassword, string username)
        {
            if (AS.AuthenticateServer(IPAddr, hostName, port, hashPassword, username))
            {
                TGS.RegisterService(IPAddr, hostName, port, hashPassword);
                Console.WriteLine("Usmesna autentifikacija Servera");
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SignOutService(string hostName)
        {
            TGS.SignOutService(hostName);
        }
    }
}
