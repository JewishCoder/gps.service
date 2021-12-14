using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gps.common.Dal
{
	public interface IConnectionConfiguration
	{
		string Host { get; }

		int Port { get; }

		string Database { get; }

		string Username { get; }

		string Password { get; }

		void SetConfiguration(IConnectionConfiguration configuration);
	}
}
