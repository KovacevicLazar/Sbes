using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Manager;

namespace Server
{
    class WCFServiceRegister : ChannelFactory<IClientConnection>, IDisposable
    {
        IClientConnection factory;

        public WCFServiceRegister(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            /// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
            //string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            //this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            //this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            //this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
            //this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = CreateChannel();
        }

        public void Registration(string ipAddr, string hostName, string username)
        {
            try
            {
                factory.serviceRegistration(ipAddr,hostName,username);
            }
            catch (Exception e)
            {
                Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
            }
        }

        public void serviceSingOut(string ipAddr, string hostName, string userName)
        {
            try
            {
                factory.serviceSingOut(ipAddr, hostName,userName);
            }
            catch (Exception e)
            {
                Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
            }
        }

    }
}
