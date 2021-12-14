using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gps.common.Dal;

using Microsoft.EntityFrameworkCore;

namespace gps.dal
{
	internal class ContextFactory
	{
		private IConnectionStringProvider StringProvider { get; }

		public ContextFactory(IConnectionStringProvider stringProvider)
		{
			StringProvider = stringProvider;
		}

		public Context Create()
		{
			var builder = new DbContextOptionsBuilder();
			builder.UseNpgsql(StringProvider.Get());
			return new Context(builder.Options);
		}
	}
}
