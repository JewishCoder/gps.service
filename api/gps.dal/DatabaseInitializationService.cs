using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using gps.common;

namespace gps.dal
{
	internal class DatabaseInitializationService : IInitializationService
	{
		public int Priority { get; } = 0;

		public async Task InitializationAsync(IComponentContext context)
		{
			var factory = context.Resolve<ContextFactory>();


			using var dbContext = factory.Create();
			await dbContext.Database.EnsureCreatedAsync()
				.ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
