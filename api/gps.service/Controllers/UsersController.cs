using System.Net;

using gps.service.Models;
using gps.service.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gps.Service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UsersController : ControllerBase
	{
		public UsersService UsersService { get; }

		public UsersController(UsersService usersService)
		{
			UsersService = usersService;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var users = await UsersService.GetUsersAsync();
			return new JsonResult(users);
		}

		[HttpPost]
		public async Task<IActionResult> Create(NewUserModel model)
		{
			var user = await UsersService.CreateAsync(model)
				.ConfigureAwait(continueOnCapturedContext: false);
			return new JsonResult(user)
			{
				StatusCode = (int)HttpStatusCode.Created,
			};
		}

		[HttpPut]
		[Route("{id}")]
		public async Task<IActionResult> Update(long id, UpdateUserModel model)
		{
			var user = await UsersService.UpdateAsync(id, model)
				.ConfigureAwait(continueOnCapturedContext: false);
			return new JsonResult(user);
		}

		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> Delete(long id)
		{
			var isDeleted = await UsersService.DeleteAsync(id)
				.ConfigureAwait(continueOnCapturedContext: false);
			return new JsonResult(isDeleted);
		}
	}
}
