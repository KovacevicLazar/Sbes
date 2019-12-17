using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Server
{
    class WCFServiceRegister : ChannelFactory<IServiceConnection>, IDisposable
    {
        IServiceConnection factory;

        public WCFServiceRegister(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            factory = CreateChannel();
        }

        public void Registration(string IPAddr, string hostNane, string port, string hashPassword)
        {
            try
            {
                factory.RegisterService(IPAddr, hostNane, port, hashPassword);
            }
            catch (Exception e)
            {
                Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
            }
        }

		// TODO:
        public void serviceSingOut(string hostName)
        {
            try
            {
                factory.SignOutService(hostName);
            }
            catch (Exception e)
            {
                Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
            }
        }

    }
}
