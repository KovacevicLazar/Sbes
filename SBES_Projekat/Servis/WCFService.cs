using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Servis
{
	public class WCFService : IDataManagement, IWCFContracts
	{
		public bool Read()
		{
			Console.WriteLine("Klijent pozvao Read");
			return true;
		}

		public void SendMessage(string message, byte[] sign)
		{
			string s = "";
			foreach(byte b in sign)
			{
				s += b;
			}
			Console.WriteLine($"Klijent pozvao SendMessage. \nPoslao: {message}; \nPotpisao: {s}");
		}

		public void TestCommunication()
		{
			throw new NotImplementedException();
		}

		public bool Write(string text)
		{
			Console.WriteLine("Klijent pozvao Write");
			return true;
		}
	}
}
