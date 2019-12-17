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

		public DomenController()
		{
		}

		public bool ValidateUser(string username, string password)
		{
			return AS.Authenticate(username, password);
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="username"></param>
        /// <returns>Ime servisa i hesovan tajni kljuc</returns>
		public Tuple<string, string> SendServiceRequest(string service, string username)
		{
			// TODO: provera greske
			return TGS.GetServiceEndpointAndSecretKey(service, AS.GetHashedUserPassword(username));
		}
	}
}
