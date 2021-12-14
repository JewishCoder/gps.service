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

		public PointsService(
			IPointsRepository repository,
			IMapper mapper,
			UsersService usersService)
		{
			Repository = repository;
			Mapper = mapper;
			UsersService = usersService;
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

			return (await builder.ExecuteAsync()
					.ConfigureAwait(continueOnCapturedContext: false)).ToArray();
		}

		public async Task<PointDto> Create(PointModel model) 
		{
			if(model == null || string.IsNullOrWhiteSpace(model.Name) || model.X == default || model.Y == default)
			{
				throw new InvalidOperationException("data is null");
			}

			var currentUser = UsersService.GetCurrent();
			if(currentUser.Role != RoleType.Admin) 
			{
				throw new ForbiddenException("point");
			}

			var dto = Mapper.Map<PointDto>(model);
			var result = await Repository.InsertAsync(dto)
				.ConfigureAwait(continueOnCapturedContext: false);
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
			return result;
		}

		public async Task<bool> DeleteAsync(long id) 
		{
			var currentUser = UsersService.GetCurrent();
			if(currentUser.Role != RoleType.Admin)
			{
				throw new ForbiddenException("point");
			}

			return (await Repository.DeleteAsync(id)
				.ConfigureAwait(continueOnCapturedContext: false)) > 0;
		}
	}
}
