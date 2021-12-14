using AutoMapper;

using gps.common.Dal;
using gps.common.Dal.Repositories;
using gps.common.Dto;
using gps.common.Enums;
using gps.common.Exceptions;
using gps.service.Models;

using Microsoft.Extensions.Caching.Memory;

namespace gps.service.Services
{
	public class UsersService : IPerDepenecy
	{
		private static readonly Func<string, object> _cacheKey = x => $"user_{x}";

		private IUsersRepository Repository { get; }

		private JwtTokenFactory JwtTokenFactory { get; }

		private IMapper Mapper { get; }

		private IHttpContextAccessor HttpContextAccessor { get; }
		
		private IMemoryCache Cache { get; }

		public UsersService(
			IUsersRepository repository, 
			IMapper mapper,
			IHttpContextAccessor httpContextAccessor,
			IMemoryCache memoryCache,
			JwtTokenFactory jwtTokenFactory)
		{
			Repository = repository;
			JwtTokenFactory = jwtTokenFactory;
			Mapper = mapper;
			HttpContextAccessor = httpContextAccessor;
			Cache = memoryCache;
		}

		public UserDto GetCurrent() 
		{
			var claims = HttpContextAccessor?.HttpContext?.User?.FindFirst(x => x.Type == JwtTokenFactory.UserIdClaimType);
			if(claims == null) 
			{
				throw new InvalidOperationException("can't get current user");
			}

			if(Cache.TryGetValue<UserDto>(_cacheKey(claims.Value), out var value))
			{
				return value;
			}

			var userId = long.Parse(claims.Value);
			using var builder = Repository.GetFetchBuilder();
			
			var user = builder.Where(x => x.Id == userId).Execute().FirstOrDefault();
			if(user == null) 
			{
				throw new InvalidOperationException($"can't get current user. userid={userId} not found");
			}

			Cache.Set(_cacheKey(claims.Value), user);
			return user;
		}

		public async Task<TokenModel> LoginAsync(AuthModel model) 
		{
			if(string.IsNullOrWhiteSpace(model.Login) || string.IsNullOrWhiteSpace(model.Password)) 
			{
				throw new AuthException("data is empty");
			}

			var user = await Repository.LoginAsync(model.Login, model.Password)
				.ConfigureAwait(continueOnCapturedContext: false);
			if(user == null) 
			{
				throw new AuthException("user not found");
			}

			return JwtTokenFactory.Create(user);
		}

		public async Task<UserDto> CreateAsync(NewUserModel model) 
		{
			if(string.IsNullOrWhiteSpace(model.Login) || 
				string.IsNullOrWhiteSpace(model.Name) || 
				string.IsNullOrWhiteSpace(model.Password))
			{
				throw new AuthException("data is empty");
			}

			var currentUser = GetCurrent();
			if(currentUser.Role != RoleType.Admin) 
			{
				throw new ForbiddenException("user");
			}

			var dto = Mapper.Map<UserDto>(model);
			var user = await Repository.CreateAsync(dto, model.Password)
				.ConfigureAwait(continueOnCapturedContext:false);
			return user;
		}

		public async Task<UserDto> UpdateAsync(long id, string name) 
		{
			if(string.IsNullOrWhiteSpace(name)) 
			{
				throw new InvalidOperationException("data is null");
			}

			var currentUser = GetCurrent();
			if(currentUser.Role != RoleType.Admin)
			{
				throw new ForbiddenException("user");
			}

			var dto = new UserDto { Id = id, Name = name };
			var user = await Repository.UpdateAsync(dto)
				.ConfigureAwait(continueOnCapturedContext: false);
			return user;
		}

		public async Task<bool> DeleteAsync(long id) 
		{
			var currentUser = GetCurrent();
			if(currentUser.Role != RoleType.Admin)
			{
				throw new ForbiddenException("user");
			}

			var isDeleted = (await Repository.DeleteAsync(id)
				.ConfigureAwait(continueOnCapturedContext: false)) > 0;
			if(isDeleted) 
			{
				Cache.Remove(_cacheKey(id.ToString()));
			}
			return isDeleted;
		}
	}
}
