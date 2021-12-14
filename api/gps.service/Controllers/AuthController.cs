using gps.service.Models;
using gps.service.Services;

using Microsoft.AspNetCore.Mvc;

namespace gps.service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		public UsersService UsersService { get; }

		public AuthController(UsersService usersService)
		{
			UsersService = usersService;
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login(AuthModel model)
		{
			var token = await UsersService.LoginAsync(model)
				.ConfigureAwait(continueOnCapturedContext: false);
			return new JsonResult(token);
		}
	}
}
