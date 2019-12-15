using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Server
{
	public class Connector
	{
		public void SendEncriptedSecretKey(string key)
		{
			SecretKey.secretKey = key;
		}
	}
}
