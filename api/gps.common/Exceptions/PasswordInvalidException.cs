using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gps.common.Exceptions
{
	public class PasswordInvalidException : Exception
	{
		public PasswordInvalidException() : base("Invalid password")
		{

		}
	}
}
