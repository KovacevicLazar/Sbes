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
		public void SendMessage(string message, byte[] sign)
		{
			string s = "";
			foreach (byte b in sign)
				s += b;
			Console.WriteLine($"Klijent poslao: {message};\nPotpisao: {s}.");
		}

		public void TestCommunication()
		{
			throw new NotImplementedException();
		}
	}
}
