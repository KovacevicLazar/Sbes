﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Contracts
{
	[ServiceContract]
	public interface ITicketGrantingService
	{
		[OperationContract]
		bool ValidateUser(string username, string password);
		[OperationContract]
		Tuple<string, string> SendServiceRequest(string service, string username);
    }
}
