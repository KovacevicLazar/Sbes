﻿using System;
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
			host.AddServiceEndpoint(typeof(IClientConnection), binding, address);

            host.Open();

            //host za prijavu i odjavu servera->

            string addressForServer = "net.tcp://localhost:9997/ServiceConnection";
            NetTcpBinding bindingForServer = new NetTcpBinding();

            ServiceHost hostForServer = new ServiceHost(typeof(TicketGrantingService));
            hostForServer.AddServiceEndpoint(typeof(IServiceConnection), bindingForServer, addressForServer);

            hostForServer.Open();


            Console.WriteLine("Domain controller is opened. Press <enter> to finish...");
			Console.ReadLine();

			host.Close();
		}
    }
}
