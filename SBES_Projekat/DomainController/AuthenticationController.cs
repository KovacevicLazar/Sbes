using System;
using System.Collections.Generic;
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
        
        public AuthenticationController()
		{
            StringBuilder sb = new StringBuilder();
            byte[] buffer = new byte[4] { 0, 0, 0, 0 };
            //string password = "5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8";
            string password = "5BAA61E4C9B93F3F0682250B6CF8331B7EE68FD8";
            sb.Append(password);
            //sb.Append(Encoding.UTF8.GetString(buffer));

            RegisteredUsers = new Dictionary<string, string>();
            RegisteredUsers.Add("DESKTOP-IJMHSLM\\Luka", password);
            RegisteredUsers.Add("LAZAR\\Lazar", password);
        }
		

        public bool Authenticate(string username, string password)
		{
			/// Nepostojeci korisnik ==> Exception
			if (!RegisteredUsers.ContainsKey(username))
				throw new Exception("No shuch user.");

            if (RegisteredUsers[username] == password)	///	Ispravan password ==> true
            {
                // log action
                return true;
            }
			else	/// Neispravan password ==> false
            {
                // log action
				return false;
            }
		}

		public string GetHashedUserPassword(string username)
		{
			return RegisteredUsers[username];
		}
	}
}
