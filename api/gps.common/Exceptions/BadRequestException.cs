using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gps.common.Exceptions
{
	public class BadRequestException : Exception
	{
		public BadRequestException() : base("некорректный запрос")
		{

		}

		public BadRequestException(string message) : base(message)
		{

		}
	}
}
