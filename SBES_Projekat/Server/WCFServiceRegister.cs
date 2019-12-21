using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void Registration(string IPAddr, string hostName, string port, string hashPassword)
        {
            try
            {
                factory.RegisterService(IPAddr, hostName, port, hashPassword);
			}
            catch (Exception e)
            {
                Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Service '{hostName}' register failed.", EventLogEntryType.FailureAudit, 206, 4);
				}
			}
        }

		// TODO:
        public void serviceSingOut(string hostName)
        {
            try
            {
                factory.SignOutService(hostName);
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Service '{hostName}' signed out successfully.", EventLogEntryType.SuccessAudit, 205, 4);
				}
			}
            catch (Exception e)
            {
                Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
				using (EventLog log = new EventLog("Application"))
				{
					log.Source = "Application";
					log.WriteEntry($"Service '{hostName}' sign out failed.", EventLogEntryType.FailureAudit, 205, 4);
				}
			}
        }

    }
}
