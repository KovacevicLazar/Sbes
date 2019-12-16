using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC
{ 
    public class ServiceConnection : IServiceConnection
    {
        TicketGrantingService TGS = new TicketGrantingService();

        public void RegisterService(string IPAddr, string hostName, string port)
        {
            TGS.RegisterService(IPAddr,hostName,port);
        }

        public void SignOutService(string hostName)
        {
            TGS.SignOutService(hostName);
        }
    }
}
