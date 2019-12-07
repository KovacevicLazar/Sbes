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

		public AuthenticationController()
		{
			RegisteredUsers = new Dictionary<string, string>();
			RegisteredUsers.Add("DESKTOP-IJMHSLM\\Luka", "password");
            RegisteredUsers.Add("LAZAR\\Lazar", "password");
        }

		public bool Authenticate(string username, string password)
		{
			if (!RegisteredUsers.ContainsKey(username))
				throw new Exception("No shuch user.");

			if (RegisteredUsers[username] == password)
				return true;
			return false;
		}
    }
}
