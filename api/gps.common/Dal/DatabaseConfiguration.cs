using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gps.common.Dal
{
	public class DatabaseConfiguration : IConnectionConfiguration
	{
		public string Host { get; private set; }

		public int Port { get; private set; }

		public string Database { get; private set; }

		public string Username { get; private set; }

		public string Password { get; private set; }

		public virtual void SetConfiguration(IConnectionConfiguration configuration)
		{
			Host = configuration.Host;
			Port = configuration.Port;
			Database = configuration.Database;
			Username = configuration.Username;
			Password = configuration.Password;
		}
	}
}
