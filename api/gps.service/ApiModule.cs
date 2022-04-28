using Autofac;

using AutoMapper.Contrib.Autofac.DependencyInjection;

using gps.common.Dal;
using gps.dal;
using gps.service.Services;

using System.Text.Json;
using System.Text.Json.Serialization;

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

			builder.Register(x => new JsonSerializerOptions
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				PropertyNameCaseInsensitive = true,
			}).SingleInstance();

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
