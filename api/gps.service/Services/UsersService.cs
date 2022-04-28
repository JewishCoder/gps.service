
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
		public const string SessionCookieKey = ".Api.Session";
		public const string SessionUserIdKey = "sw.u";
		public const string SessionRoleIdKey = "sw.r";
		public const string SessionTokenKey = "sw.st";
		public const string CookieKey = ".sw.c";

		private static readonly Func<string, object> _cacheKey = x => $"user_{x}";

		private IUsersRepository Repository { get; }

		private IMapper Mapper { get; }

		private IHttpContextAccessor HttpContextAccessor { get; }

		private IMemoryCache Cache { get; }

		public JwtTokenFactory JwtTokenFactory { get; }

		public NotifyService NotifyService { get; }

		public UsersService(
			IUsersRepository repository,
			IMapper mapper,
			IHttpContextAccessor httpContextAccessor,
			IMemoryCache memoryCache,
			JwtTokenFactory jwtTokenFactory,
			NotifyService notifyService)
		{
			Repository = repository;
			JwtTokenFactory = jwtTokenFactory;
			NotifyService = notifyService;
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
				throw new AuthException($"Пользователь не найден.");
			}

			Cache.Set(_cacheKey(claims.Value), user);
			return user;
		}

		public async Task<TokenModel> LoginAsync(AuthModel model)
		{
			if(string.IsNullOrWhiteSpace(model.Login) || string.IsNullOrWhiteSpace(model.Password))
			{
				throw new AuthException("Логин и пароль не должны быть пустыми!");
			}

			var user = await Repository.LoginAsync(model.Login, model.Password)
				.ConfigureAwait(continueOnCapturedContext: false);
			if(user == null)
			{
				throw new AuthException("Пользователь не найден");
			}

			return JwtTokenFactory.Create(user);
		}

		public async Task<UserDto> GetUserAsync(long id)
		{
			using var builder = Repository.GetFetchBuilder();
			builder.Where(x => x.Id == id);

			var users = await builder.ExecuteAsync()
				.ConfigureAwait(continueOnCapturedContext: false);
			return users.FirstOrDefault();
		}

		public async Task<UserDto[]> GetUsersAsync()
		{
			var currentUser = GetCurrent();
			if(currentUser.Role != RoleType.Admin)
			{
				throw new ForbiddenException(currentUser.Login);
			}

			using var builder = Repository.GetFetchBuilder();
			var users = await builder.ExecuteAsync()
				.ConfigureAwait(continueOnCapturedContext: false);
			return users.ToArray();
		}

		public async Task<UserDto> CreateAsync(NewUserModel model)
		{
			if(string.IsNullOrWhiteSpace(model.Login) ||
				string.IsNullOrWhiteSpace(model.Name) ||
				string.IsNullOrWhiteSpace(model.Password))
			{
				throw new AuthException("Логин, имя и пароль не должны быть пустыми!");
			}

			var currentUser = GetCurrent();
			if(currentUser.Role != RoleType.Admin)
			{
				throw new ForbiddenException("user");
			}

			using var builder = Repository.GetFetchBuilder();
			var userExists = await builder
				.Where(x => x.Login == model.Login && x.DeletedOn == null)
				.ExecuteAsync()
				.ConfigureAwait(continueOnCapturedContext: false);
			if(userExists.Any())
			{
				throw new BadRequestException("Такой пользователь уже существует!");
			}

			var dto = Mapper.Map<UserDto>(model);
			var user = await Repository.CreateAsync(dto, model.Password)
				.ConfigureAwait(continueOnCapturedContext: false);
			_ = NotifyService.NewUserAsync(currentUser.Id, user);
			return user;
		}

		public async Task<UserDto> UpdateAsync(long id, UpdateUserModel model)
		{
			var currentUser = GetCurrent();
			if(currentUser.Role != RoleType.Admin)
			{
				throw new ForbiddenException("user");
			}

			var dto = new UserDto { Id = id, Role = model.Role };
			if(!string.IsNullOrWhiteSpace(model.Login))
			{
				dto.Login = model.Login;
			}
			if(!string.IsNullOrWhiteSpace(model.Name))
			{
				dto.Name = model.Name;
			}

			var user = await Repository.UpdateAsync(dto)
				.ConfigureAwait(continueOnCapturedContext: false);
			if(!string.IsNullOrWhiteSpace(model.Password))
			{
				user = await Repository.UpdatePasswordAsync(id, model.Password)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			_ = NotifyService.NewUserAsync(currentUser.Id, user);
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
			_ = NotifyService.DeletedUserAsync(currentUser, id);
			return isDeleted;
		}
	}
}
