using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DC
{
    public class TicketGrantingService
    {
        //<IPaddr,HostName>
        private Dictionary<string, string> ipAddrAndHostName = new Dictionary<string, string>();

        //<IPAddr,ServiceID>
        private Dictionary<string, EndpointIdentity> activeServices = new Dictionary<string, EndpointIdentity>();

        public bool servisPostoji(string ipAddr)
        {
            if (activeServices.ContainsKey(ipAddr))
            {
                //Ovde treba generisati tajni kljuc koji treba proslediti klijentu i serveru
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public bool prijavljivanjeNovogServisa(string ipAddr,string hostName, EndpointIdentity ServiceID)
        {
            if (activeServices.ContainsKey(ipAddr))
            {
                return false; //Servis je vec na spisku startovanih servisa
            }
            else
            {
                ipAddrAndHostName[ipAddr] = hostName;
                activeServices[ipAddr] = ServiceID;
                return true;
            }
        }


        //
        //Dodavanje metode za brisanje servisa sa liste aktivnih..?
        //

    }
}
