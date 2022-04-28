using gps.common.Exceptions;
using gps.service.Models;
using gps.service.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gps.service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private ILogger<AuthController> Logger { get; }

		public UsersService UsersService { get; }

		public AuthController(
			ILogger<AuthController> logger,
			UsersService usersService)
		{
			Logger = logger;
			UsersService = usersService;
		}

		[HttpPost]
		[Route("login")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenModel))]
		public async Task<IActionResult> Login(AuthModel model)
		{
			var token = await UsersService.LoginAsync(model)
				.ConfigureAwait(continueOnCapturedContext: false);
			SetCookies(token);
			return new JsonResult(token);
		}

		[HttpPost]
		[Route("refresh")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenModel))]
		public async Task<IActionResult> Refresh()
		{
			if(HttpContext == null ||
				HttpContext.Request == null ||
				HttpContext.Request.Cookies == null)
			{
				throw new UnauthorizedAccessException();
			}

			if(HttpContext.Request.Cookies.TryGetValue(UsersService.CookieKey, out var token))
			{
				var userId = UsersService.JwtTokenFactory.TryGetUserFromToken(token!);
				if(userId.HasValue)
				{
					var user = await UsersService.GetUserAsync(userId.Value)
						.ConfigureAwait(continueOnCapturedContext: false);
					if(user == null)
					{
						HttpContext.Response.Cookies.Delete(UsersService.CookieKey);
						HttpContext.Response.Cookies.Delete(UsersService.SessionCookieKey);
						throw new NotFoundException("user deleted");

					}
					var result = UsersService.JwtTokenFactory.Create(user);
					SetCookies(result);
					Logger.LogInformation("Login refreshed {userId}", user);
					
					return new JsonResult(result);
				}
				else
				{
					throw new UnauthorizedAccessException("Refresh token incorrect");
				}
			}
			else
			{
				throw new UnauthorizedAccessException();
			}
		}

		[HttpGet]
		[Route("logout/{id}")]
		[Authorize]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
		public IActionResult LogoutAsync(long id)
		{
			if(HttpContext.Request.Cookies.TryGetValue(UsersService.CookieKey, out var token))
			{
				var user = UsersService.GetCurrent();
				if(UsersService.JwtTokenFactory.ValidateToken(user, token!))
				{
					if(id == user.Id)
					{
						HttpContext.Response.Cookies.Delete(UsersService.CookieKey);
						HttpContext.Response.Cookies.Delete(UsersService.SessionCookieKey);
						return new JsonResult(true);
					}
				}
			}

			return new JsonResult(false);
		}

		private void SetCookies(TokenModel model)
		{
			HttpContext.Session.SetString(UsersService.SessionTokenKey, model.Token);
			HttpContext.Session.SetString(UsersService.SessionUserIdKey, model.User.Id.ToString());
			HttpContext.Session.SetString(UsersService.SessionRoleIdKey, model.User.Role.ToString());
			HttpContext.Response.Cookies.Append(
				UsersService.CookieKey,
				model.RefreshToken,
				new CookieOptions { MaxAge = new TimeSpan(0, 9, 0, 0) });
		}
	}
}
