using gps.common.Dal;
using gps.common.Dto;
using gps.service.Hubs;

using Microsoft.AspNetCore.SignalR;

using System.Text.Json;

namespace gps.service.Services
{
	public class NotifyService : IPerDepenecy
	{
		private IHubContext<NotifyHub> HubContext { get; }

		private ILogger<NotifyService> Logger { get; }

		private JsonSerializerOptions JsonSerializerOptions { get; }

		public NotifyService(
			IHubContext<NotifyHub> hubContext, 
			ILogger<NotifyService> logger,
			JsonSerializerOptions jsonSerializerOptions)
		{
			HubContext = hubContext;
			Logger = logger;
			JsonSerializerOptions = jsonSerializerOptions;
		}

		public async Task NewPointAsync(long userId, PointDto value) 
		{
			var json = JsonSerializer.Serialize(new 
			{
				UserId = userId,
				Point  = value,
			}, JsonSerializerOptions);

			await HubContext.Clients.All
				.SendAsync("NewPointEvent", json)
				.ConfigureAwait(continueOnCapturedContext: false);
			Logger.LogInformation("NewPointEvent user={userId} point={pointId}", userId, value.Id);
		}

		public async Task UpdatedPointAsync(long userId, PointDto value)
		{
			var json = JsonSerializer.Serialize(new
			{
				UserId = userId,
				Point = value,
			}, JsonSerializerOptions);

			await HubContext.Clients.All
				.SendAsync("UpdatedPointEvent", json)
				.ConfigureAwait(continueOnCapturedContext: false);
			Logger.LogInformation("UpdatedPointEvent user={userId} point={pointId}", userId, value.Id);
		}

		public async Task DeletedPointAsync(long userId, long id)
		{
			var json = JsonSerializer.Serialize(new
			{
				UserId = userId,
				PointId = id,
			}, JsonSerializerOptions);

			await HubContext.Clients.All
				.SendAsync("DeletedPointEvent", json)
				.ConfigureAwait(continueOnCapturedContext: false);
			Logger.LogInformation("DeletedPointEvent user={userId} point={pointId}", userId, id);
		}

		public async Task NewUserAsync(long userId, UserDto value) 
		{
			var json = JsonSerializer.Serialize(new
			{
				UserId = userId,
				NewUserId = value.Id,
				value.Login,
			}, JsonSerializerOptions);

			await HubContext.Clients.All
				.SendAsync("NewUserEvent", json)
				.ConfigureAwait(continueOnCapturedContext: false);
			Logger.LogInformation("NewUserEvent user={userId} user={newUserId}", userId, value.Id);
		}

		public async Task UpdatedUserAsync(long userId, UserDto value)
		{
			var json = JsonSerializer.Serialize(new
			{
				UserId = userId,
				UpdatedUserId = value.Id,
			}, JsonSerializerOptions);

			await HubContext.Clients.All
				.SendAsync("UpdatedUserEvent", json)
				.ConfigureAwait(continueOnCapturedContext: false);
			Logger.LogInformation("UpdatedUserEvent user={userId} user={updatedUserId}", userId, value.Id);
		}

		public async Task DeletedUserAsync(UserDto user, long value)
		{
			var json = JsonSerializer.Serialize(new
			{
				UserId = user.Id,
				Login = user.Login,
				DeletedUserId = value,
			}, JsonSerializerOptions);

			await HubContext.Clients.All
				.SendAsync("DeletedUserEvent", json)
				.ConfigureAwait(continueOnCapturedContext: false);
			Logger.LogInformation("DeletedUserEvent user={userId} user={deletedUserId}", user.Id, value);
		}
	}
}
