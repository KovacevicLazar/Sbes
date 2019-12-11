using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DC
{
    public class AuthenticationController
    {
		private Dictionary<string, string> RegisteredUsers;
        private static Dictionary<string, string> registeredService = new Dictionary<string, string>();
        

        public AuthenticationController()
		{
			RegisteredUsers = new Dictionary<string, string>();
			RegisteredUsers.Add("DESKTOP-IJMHSLM\\Luka", "password");
            RegisteredUsers.Add("LAZAR\\Lazar", "password");
        }

        public static Dictionary<string, string> RegisteredService { get => registeredService; set => registeredService = value; }

        public bool Authenticate(string username, string password)
		{
			if (!RegisteredUsers.ContainsKey(username))
				throw new Exception("No shuch user.");

            if (RegisteredUsers[username] == password)
            {
                // log action

               // CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal; //proveriti zasto je null
                try
                {
                    // Audit.AuthenticationSuccess(principal.Identity.Name);
                    Audit.AuthenticationSuccess(username);
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(ae.Message);
                }
               
                return true;
            }
            else
            {
                // log action

                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                try
                {
                    // Audit.AuthenticationFailed(principal.Identity.Name);
                    Audit.AuthenticationFailed(username);
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(ae.Message);
                }

            }
			return false;
		}
        public bool serviceRegistration(string userName)
        {
            if (RegisteredService.ContainsKey(userName))
            {
                return false;
            }
            RegisteredService[userName] = "password";
            return true;
        }
        public bool serviceSingOut(string userName)
        {
            if (RegisteredService.ContainsKey(userName))
            {
                RegisteredService.Remove(userName);
                return true;
            }
            return false;
        }
    }
}
