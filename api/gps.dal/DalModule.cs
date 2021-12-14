using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using gps.common.Dal;

namespace gps.dal
{
	public class DalModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
				.Where(x => x.GetInterfaces().Contains(typeof(IPerDepenecy)))
				.ToArray();
			builder.RegisterTypes(types)
				.AsImplementedInterfaces()
				.InstancePerDependency();

			builder.RegisterType<PostgresConnectionStringProvider>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<ContextFactory>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<DatabaseInitializationService>()
				.AsImplementedInterfaces()
				.InstancePerDependency();
		}
	}
}
