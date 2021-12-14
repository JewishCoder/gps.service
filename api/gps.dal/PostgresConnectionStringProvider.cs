using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gps.common.Dal;

using Npgsql;

namespace gps.dal
{
	class PostgresConnectionStringProvider : DatabaseConfiguration, IConnectionStringProvider
	{
		private string _cachedString;

		public override void SetConfiguration(IConnectionConfiguration configuration)
		{
			base.SetConfiguration(configuration);
			_cachedString = null;
		}

		public string Get()
		{
			if(_cachedString == null)
			{
				var builder = new NpgsqlConnectionStringBuilder
				{
					Host = Host,
					Port = Port,
					Database = Database,
					Username = Username,
					Password = Password,
					SearchPath = "public",
				};

				_cachedString = builder.ToString();
			}

			return _cachedString;
		}
	}
}
