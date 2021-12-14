using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gps.common.Exceptions
{
	public class UpdateConcurrencyException : Exception
	{
		public UpdateConcurrencyException(Exception innerException) : base(null, innerException)
		{

		}
	}
}
