using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;

namespace Client
{
	public class WCFClient : ChannelFactory<IWCFContracts>, IDisposable
	{
		IWCFContracts factory;

		public WCFClient(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			factory = CreateChannel();
		}
		public void SendMessage(string message, string key)
		{
			try
			{
				///TODO: enkripcija poruke pre slanja
				///string encriptedMessage = Encrypt(message, key);
				///factory.SendMessage(encryptedMessage);
			}
			catch (Exception e)
			{
				Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
			}
		}

		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		} 

		//public void TestCommunication()
		//{
		//	try
		//	{
		//		factory.TestCommunication();
		//	}
		//	catch (Exception e)
		//	{
		//		Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
		//	}
		//}
	}
}
