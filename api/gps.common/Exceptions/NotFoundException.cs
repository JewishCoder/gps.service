using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gps.common.Exceptions
{
	public class NotFoundException : Exception
	{
		public long? Id { get; }

		public string Entity { get; }

		public NotFoundException()
		{

		}

		public NotFoundException(string message) : base(message)
		{

		}

		public NotFoundException(long id, string entity, string message) : base(message)
		{
			Id = Id;
			Entity = entity;
		}
	}
}
