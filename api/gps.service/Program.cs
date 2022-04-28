using Autofac;
using Autofac.Extensions.DependencyInjection;

using gps.common.Dal;
using gps.service.Extensions;
using gps.service.Hubs;
using gps.service.Middleware;
using gps.service.Services;

using Gps.Service;
using Gps.Service.Configuration;
using Gps.Service.Extensions;

using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.HttpOverrides;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, conf) =>
{
	conf.ReadFrom.Configuration(context.Configuration, "Serilog");
});
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new ApiModule()));

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders =
		ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.Configure<JwtTokenConfiguration>(
	builder.Configuration.GetSection("JwtToken"),
	x => x.BindNonPublicProperties = true);

builder.Services.Configure<DatabaseConfiguration>(
	builder.Configuration.GetSection("Database"),
	x => x.BindNonPublicProperties = true);

var jwtConfig = new JwtTokenConfiguration();
builder.Configuration.GetSection("JwtToken").Bind(jwtConfig, x => x.BindNonPublicProperties = true);

builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache(x =>
{
	x.ExpirationScanFrequency = new TimeSpan(0, 10, 0);
});
builder.Services.AddCors(x =>
{
	x.AddPolicy("Any", builder =>
	{
		builder
			.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials()
			.SetIsOriginAllowedToAllowWildcardSubdomains();

		builder
			.WithOrigins("http://localhost:3000")
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials();
	});
});


builder.Services.AddSession(x =>
{
	x.Cookie.SameSite = SameSiteMode.None;
	x.Cookie.Name = UsersService.SessionCookieKey;
	x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	x.Cookie.IsEssential = true;
});

builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddJWTAuthentication(jwtConfig);
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Switch Tech v1");
		c.RoutePrefix = "api";
	});
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseForwardedHeaders();
app.UseRouting();
app.UseSession();
app.UseCookiePolicy(new CookiePolicyOptions
{
	MinimumSameSitePolicy = SameSiteMode.None,
	HttpOnly = HttpOnlyPolicy.Always,
	Secure = CookieSecurePolicy.Always,
});
app.UseCors("Any");
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
	endpoints.MapHub<NotifyHub>("/api/notifications");
});

await app.InitializationAsync();
app.Run();
