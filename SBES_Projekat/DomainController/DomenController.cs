using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace DC
{
	public class DomenController : IClientConnection
	{
		AuthenticationController AS = new AuthenticationController();
		TicketGrantingService TGS = new TicketGrantingService();

		public DomenController(AuthenticationController aS, TicketGrantingService tGS)
		{
			AS = aS ?? throw new ArgumentNullException(nameof(aS));
			TGS = tGS ?? throw new ArgumentNullException(nameof(tGS));
		}


		public bool ValidateUser(string username, string password)
		{
			return AS.Authenticate(username, password);
		}
		public Tuple<string, string> SendServiceRequest(string service)
		{
			string serviceEndpoint = TGS.GetServiceEndpoint(service);
		}
	}
}
