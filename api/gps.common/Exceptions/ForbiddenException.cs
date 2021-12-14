using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gps.common.Exceptions
{
	public class ForbiddenException : Exception
	{
		public ForbiddenException() : base("No access")
		{

		}

		public ForbiddenException(string entityType)
			: base($"{entityType} no access")
		{
		}
	}
}
