using Autofac;

using AutoMapper.Contrib.Autofac.DependencyInjection;

using gps.common.Dal;
using gps.dal;
using gps.service.Services;

namespace Gps.Service
{
	public class ApiModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);
			builder.RegisterAutoMapper(x =>
			{
				x.AddMaps(typeof(ApiModule).Assembly, typeof(DalModule).Assembly);
			});

			builder.RegisterModule<DalModule>();

			builder.RegisterType<JwtTokenFactory>()
				.AsSelf()
				.InstancePerDependency();

			var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
				.Where(x => x.GetInterfaces().Contains(typeof(IPerDepenecy)))
				.ToArray();

			builder.RegisterTypes(types)
				.AsSelf()
				.AsImplementedInterfaces()
				.InstancePerDependency();
		}
	}
}
