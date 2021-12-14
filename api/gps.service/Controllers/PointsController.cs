using System.Net;

using gps.service.Models;
using gps.service.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gps.service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PointsController : ControllerBase
	{
		public PointsService PointsService { get; }

		public PointsController(PointsService pointsService)
		{
			PointsService = pointsService;
		}

		[HttpGet]
		public async Task<IActionResult> Get([FromQuery]string[] names = null) 
		{
			var points = await PointsService.GetAsync(new PointFilter { Names = names, })
				.ConfigureAwait(continueOnCapturedContext: false);
			return new JsonResult(points);
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> Get(long id)
		{
			var points = await PointsService.GetAsync(new PointFilter { Id = id, })
				.ConfigureAwait(continueOnCapturedContext: false);
			return new JsonResult(points);
		}

		[HttpPost]
		public async Task<IActionResult> Create(PointModel model) 
		{
			var point = await PointsService.Create(model)
				.ConfigureAwait(continueOnCapturedContext: false);
			return new JsonResult(point)
			{
				StatusCode = (int)HttpStatusCode.Created,
			};
		}

		[HttpPut]
		[Route("{id}")]
		public async Task<IActionResult> Update(long id, PointModel model) 
		{
			var point = await PointsService.Update(id, model)
				.ConfigureAwait(continueOnCapturedContext: false);

			return new JsonResult(point);
		}

		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> Delete(long id) 
		{
			var isDeleted = await PointsService.DeleteAsync(id)
				.ConfigureAwait(continueOnCapturedContext: false);
			return new JsonResult(isDeleted);
		}
	}
}
