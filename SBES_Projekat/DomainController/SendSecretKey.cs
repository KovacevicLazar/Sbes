using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DC
{
    public class SendSecretKey : ChannelFactory<IServiceKeyHandler>, IDisposable
    {
        IServiceKeyHandler factory;

        public SendSecretKey(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            factory = CreateChannel();
        }

        public void SendEncriptedSecretKey(string key)
        {
            factory.SendEncriptedSecretKey(key);
        }
    }
}
