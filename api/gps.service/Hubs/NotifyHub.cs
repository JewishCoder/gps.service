using gps.service.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using System.Text.Json;

namespace gps.service.Hubs
{
	[Authorize]
	public class NotifyHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync()
				.ConfigureAwait(continueOnCapturedContext: false);

			var user = Context.User?.FindFirst(JwtTokenFactory.UserIdClaimType)?.Value;
			var json = JsonSerializer.Serialize(new
			{
				Context.ConnectionId,
				UserId = user,
				Message = "connected",
			});

			await Clients.All.SendAsync("Connected", json)
				.ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
