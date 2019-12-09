using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
				return true;
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
