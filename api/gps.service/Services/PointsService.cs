using AutoMapper;

using gps.common.Dal;
using gps.common.Dal.Repositories;
using gps.common.Dto;
using gps.common.Enums;
using gps.common.Exceptions;
using gps.service.Models;

namespace gps.service.Services
{
	public class PointsService : IPerDepenecy
	{
		private IPointsRepository Repository { get; }

		private IMapper Mapper { get; }

		private UsersService UsersService { get; }

		private NotifyService NotifyService { get; }

		public PointsService(
			IPointsRepository repository,
			IMapper mapper,
			UsersService usersService,
			NotifyService notifyService)
		{
			Repository = repository;
			Mapper = mapper;
			UsersService = usersService;
			NotifyService = notifyService;
		}

		public async Task<PointDto[]> GetAsync(PointFilter filter)
		{
			var currentUser = UsersService.GetCurrent();
			if(currentUser == null)
			{
				throw new ForbiddenException("point");
			}

			var builder = Repository.GetFetchBuilder();
			if(filter != null)
			{
				if(filter.Id.HasValue)
				{
					builder.Where(x => x.Id == filter.Id.Value);
				}
				if(filter.Names != null && filter.Names.Length > 0)
				{
					builder.Where(x => filter.Names.Contains(x.Name));
				}
			}

			var result = (await builder.ExecuteAsync()
					.ConfigureAwait(continueOnCapturedContext: false)).ToArray();

			return result;
		}

		public async Task<PointDto> Create(PointModel model)
		{
			if(model == null || string.IsNullOrWhiteSpace(model.Name) || model.Latitude == default || model.Longitude == default)
			{
				throw new BadRequestException("Необходимо указать имя, широту и долготу точки!");
			}

			var currentUser = UsersService.GetCurrent();
			if(currentUser.Role != RoleType.Admin)
			{
				throw new ForbiddenException("point");
			}

			using var builder = Repository.GetFetchBuilder();
			var pointExists = await builder
				.Where(x => x.Name == model.Name)
				.ExecuteAsync()
				.ConfigureAwait(continueOnCapturedContext: false);

			if(pointExists.Any())
			{
				throw new BadRequestException("Такая точка уже существует!");
			}

			var dto = Mapper.Map<PointDto>(model);
			var result = await Repository.InsertAsync(dto)
				.ConfigureAwait(continueOnCapturedContext: false);

			_ = NotifyService.NewPointAsync(currentUser.Id, result);
			return result;
		}

		public async Task<PointDto> Update(long id, PointModel model)
		{
			if(model == null)
			{
				throw new InvalidOperationException("data is null");
			}

			var currentUser = UsersService.GetCurrent();
			if(currentUser.Role != RoleType.Admin)
			{
				throw new ForbiddenException("point");
			}

			var dto = Mapper.Map<PointDto>(model);
			dto.Id = id;

			var result = await Repository.UpdateAsync(dto)
				.ConfigureAwait(continueOnCapturedContext: false);


			_ = NotifyService.UpdatedPointAsync(currentUser.Id, result);
			return result;
		}

		public async Task<bool> DeleteAsync(long id)
		{
			var currentUser = UsersService.GetCurrent();
			if(currentUser.Role != RoleType.Admin)
			{
				throw new ForbiddenException("point");
			}

			var isDeleted = (await Repository.DeleteAsync(id)
				.ConfigureAwait(continueOnCapturedContext: false)) > 0;


			_ = NotifyService.DeletedPointAsync(currentUser.Id, id);
			return isDeleted;
		}
	}
}
