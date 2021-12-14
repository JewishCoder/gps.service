using Autofac;

using gps.common;
using gps.common.Dal;

using Microsoft.Extensions.Options;

namespace gps.service.Extensions
{
	public static class HostExtensions
	{
		public static async Task InitializationAsync(this IHost host)
		{
			var context = (IComponentContext)host.Services.GetService(typeof(IComponentContext));
			var connectionOptions = context.Resolve<IOptions<DatabaseConfiguration>>();
			var connectionConfig = context.Resolve<IConnectionConfiguration>();
			connectionConfig.SetConfiguration(connectionOptions.Value);

			var services = context.Resolve<IInitializationService[]>().OrderBy(x => x.Priority);
			foreach(var service in services)
			{
				await service.InitializationAsync(context)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}
	}
}
