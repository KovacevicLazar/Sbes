using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Exceptions
{
	public class NoSuchUserException : Exception
	{
		private string message;
		public override string Message => message;

		public NoSuchUserException(string message)
		{
			this.message = message;
		}
	}
}
