using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;

namespace gps.common
{
	public interface IInitializationService
	{
		int Priority { get; }

		Task InitializationAsync(IComponentContext context);
	}
}
