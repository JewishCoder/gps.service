using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using gps.common;
using gps.common.Dal.Repositories;

namespace gps.dal
{
	internal class DatabaseInitializationService : IInitializationService
	{
		public int Priority { get; } = 0;

		public async Task InitializationAsync(IComponentContext context)
		{
			var factory = context.Resolve<ContextFactory>();
			var userRep = context.Resolve<IUsersRepository>();

			using var dbContext = factory.Create();
			await dbContext.Database.EnsureCreatedAsync()
				.ConfigureAwait(continueOnCapturedContext: false);

			await CreateDefaultUserAsync(userRep);
		}

		private static async Task CreateDefaultUserAsync(IUsersRepository repository) 
		{
			using var builder = repository.GetFetchBuilder();
			builder.Where(x => x.Login == "gps.default.admin");
			var adminExists = (await builder.ExecuteAsync()
				.ConfigureAwait(continueOnCapturedContext: false))
				.Any();
			if(adminExists) 
			{
				return;
			}

			await repository.CreateAsync(new common.Dto.UserDto
			{
				Login = "gps.default.admin",
				Name = "Администратор",
				Role = common.Enums.RoleType.Admin,
			}, "gps#admin").ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
