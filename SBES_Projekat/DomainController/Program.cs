using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace DC
{
    class Program
    {
        static void Main(string[] args)
		{
			string address = "net.tcp://localhost:9998/DomenController";
			NetTcpBinding binding = new NetTcpBinding();

			ServiceHost host = new ServiceHost(typeof(DomenController));
			host.AddServiceEndpoint(typeof(ITicketGrantingService), binding, address);

			host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            //host.Authorization.ServiceAuthorizationManager = new CustomServiceAuthorizationManager();

            //host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            //List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            //policies.Add(new CustomAuthorizationPolicy());
            //host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            // podesavanje logovanja
            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
            newAudit.SuppressAuditFailure = true;

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit);

            host.Open();
			Console.WriteLine("Domain controller is opened. Press <enter> to finish...");
			Console.ReadLine();

			host.Close();
		}
    }
}
