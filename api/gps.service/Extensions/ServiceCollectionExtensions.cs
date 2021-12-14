using Gps.Service.Configuration;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Gps.Service.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gps Service Api", Version = "v1" });
				c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = JwtBearerDefaults.AuthenticationScheme,
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
				});
				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id   = JwtBearerDefaults.AuthenticationScheme,
							},
						},
						new List<string>()
					},
				});
			});
		}

		public static void AddJWTAuthentication(this IServiceCollection services, JwtTokenConfiguration config)
		{
			services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(x =>
			{
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = config.Issuer,
					ValidateAudience = true,
					RequireAudience = true,
					ValidAudience = config.Audience,
					ValidateLifetime = true,
					RequireExpirationTime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = config.GetSecurityKey(),
				};
			});
		}
	}
}
