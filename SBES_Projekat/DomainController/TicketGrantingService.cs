using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DC
{
    public class TicketGrantingService
    {
        //<IPaddr,HostName>
        private static Dictionary<string, string> ipAddrAndHostName = new Dictionary<string, string>();

        //<hostName,ServiceID>
        private static Dictionary<string, EndpointIdentity> activeServices = new Dictionary<string, EndpointIdentity>();

        public static Dictionary<string, string> IpAddrAndHostName { get => ipAddrAndHostName; set => ipAddrAndHostName = value; }
        public static Dictionary<string, EndpointIdentity> ActiveServices { get => activeServices; set => activeServices = value; }

        public bool serserviceExists(string hostName)
        {
            if (ActiveServices.ContainsKey(hostName))
            {
                // log action

                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                try
                {
                    // Audit.ValidationSuccess(principal.Identity.Name); //Treba prslediti hostNemeServisa?
                    Audit.ValidationFailed(hostName);
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(ae.Message);
                }
                //Ovde treba generisati tajni kljuc koji treba proslediti klijentu i serveru
                return true;
            }
            else
            {
                // log action

               CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                try
                {
                    // Audit.ValidationFailed(principal.Identity.Name); //Treba prslediti hostNemeServisa?
                    Audit.ValidationFailed(hostName);
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(ae.Message);
                }

                return false;
            }
        }
        
        public bool serviceRegistration(string ipAddr,string hostName)
        {
            if (ActiveServices.ContainsKey(ipAddr))
            {
                return false; //Servis je vec na spisku startovanih servisa
            }
            else
            {

                Console.WriteLine("Servis {0}/{1} has been launched", ipAddr, hostName);
                IpAddrAndHostName[ipAddr] = hostName;
                EndpointAddress endpointAddress = new EndpointAddress(new Uri("net.tcp://localhost:9999/WCFService"), EndpointIdentity.CreateDnsIdentity("WCFService"));
                ActiveServices[hostName]= endpointAddress.Identity;
                return true;
            }
        }


        public bool serviceSingOut(string ipAddr, string hostName, string userName)
        {
            if (ActiveServices.ContainsKey(hostName))
            {
                ActiveServices.Remove(hostName);
                return true;
            }
            return false;
        }


    }
}
