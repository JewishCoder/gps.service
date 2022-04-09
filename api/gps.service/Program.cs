using Autofac;
using Autofac.Extensions.DependencyInjection;

using gps.common.Dal;
using gps.service.Extensions;

using Gps.Service;
using Gps.Service.Configuration;
using Gps.Service.Extensions;

using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, conf) =>
{
	conf.ReadFrom.Configuration(context.Configuration, "Serilog");
});
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new ApiModule()));

builder.Services.Configure<JwtTokenConfiguration>(
	builder.Configuration.GetSection("JwtToken"),
	x => x.BindNonPublicProperties = true);

builder.Services.Configure<DatabaseConfiguration>(
	builder.Configuration.GetSection("Database"),
	x => x.BindNonPublicProperties = true);

var jwtConfig = new JwtTokenConfiguration();
builder.Configuration.GetSection("JwtToken").Bind(jwtConfig, x => x.BindNonPublicProperties = true);

builder.Services.AddControllers();
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
			.SetIsOriginAllowedToAllowWildcardSubdomains();
	});
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddJWTAuthentication(jwtConfig);
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("Any");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.InitializationAsync();
app.Run();
