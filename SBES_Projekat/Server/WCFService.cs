using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class WCFService : IWCFContracts
	{
		private string secretKey;

		public void SetSecretKey(string key)
		{
			secretKey = key; 
		}

		public void SendMessage(string message)
		{
			///TODO: implementirati dekripciju
			///string decryptedMessage = Decrypt(message, secretKey);


		}

		public void TestCommunication()
		{
			throw new NotImplementedException();
		}
	}
}
