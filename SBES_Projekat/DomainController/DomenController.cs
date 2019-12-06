using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace DC
{
	public class DomenController : IClientValidation
	{
		AuthenticationController AS = new AuthenticationController();
		TicketGrantingService TGS = new TicketGrantingService();

		public bool ValidateUser(string username, string password)
		{
			return AS.Authenticate(username, password);
		}
	}
}
