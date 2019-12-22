using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contracts;

namespace DC
{
    public class AuthenticationController : IClientAuthentication
    {
		private Dictionary<string, string> RegisteredUsers;
        private Dictionary<string, string> UsersAccountForServer;


        public AuthenticationController()
		{
            string password = "5BAA61E4C9B93F3F0682250B6CF8331B7EE68FD8";
          
            RegisteredUsers = new Dictionary<string, string>();
            RegisteredUsers.Add("DESKTOP-IJMHSLM\\Luka", password);
            RegisteredUsers.Add("LAZAR\\Lazar", password);

            UsersAccountForServer = new Dictionary<string, string>();
            UsersAccountForServer.Add("DESKTOP-IJMHSLM\\Luka", password);
            UsersAccountForServer.Add("LAZAR\\Lazar", password);
        }
		

        public bool Authenticate(string username, string password)
		{
			/// Nepostojeci korisnik ==> Exception
			if (!RegisteredUsers.ContainsKey(username))
			{
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Client '{username}' not found.", EventLogEntryType.Information, 203, 4);
				}
				throw new Exception("No shuch user.");
			}

            if (RegisteredUsers[username] == password)  ///	Ispravan password ==> true
			{
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Client '{username}' authentication sucessfull.", EventLogEntryType.SuccessAudit, 202, 4);
				}
				return true;
            }
			else    /// Neispravan password ==> false
			{
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Client '{username}' authentication failed.", EventLogEntryType.FailureAudit, 202, 4);
				}
				return false;
            }
		}

        public bool AuthenticateServer(string IPAddr, string hostName, string port, string hashPassword, string username)
        {
            if (!UsersAccountForServer.ContainsKey(username))
            {
                using (EventLog log = new EventLog("Application"))
                {
                    log.Source = "Application";
                    log.WriteEntry($"Client '{username}' not found.", EventLogEntryType.Information, 203, 4);
                }
                throw new Exception("No shuch user.");
            }

            if (UsersAccountForServer[username] == hashPassword)  ///	Ispravan password ==> true
			{
                using (EventLog log = new EventLog("Application"))
                {
                    log.Source = "Application";
                    log.WriteEntry($"Client '{username}' authentication sucessfull.", EventLogEntryType.SuccessAudit, 202, 4);
                }
                return true;
            }
            else    /// Neispravan password ==> false
			{
                using (EventLog log = new EventLog("Application"))
                {
                    log.Source = "Application";
                    log.WriteEntry($"Client '{username}' authentication failed.", EventLogEntryType.FailureAudit, 202, 4);
                }
                return false;
            }

        }

        public string GetHashedUserPassword(string username)
		{
			return RegisteredUsers[username];
		}
	}
}
