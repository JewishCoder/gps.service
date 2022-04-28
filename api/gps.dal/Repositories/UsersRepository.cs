
using AutoMapper;

using gps.common.Dal;
using gps.common.Dal.Repositories;
using gps.common.Dto;
using gps.common.Exceptions;
using gps.dal.Entities;

using Microsoft.EntityFrameworkCore;

namespace gps.dal.Repositories
{
	internal class UsersRepository : RepositoryBase<UserEntity, UserDto>, IUsersRepository, IPerDepenecy
	{
		public UsersRepository(ContextFactory contextFactory, IMapper mapper)
			: base(contextFactory, mapper)
		{
		}

		public async Task<UserDto> LoginAsync(string login, string password)
		{
			using var context = ContextFactory.Create();

			var user = await GetDbSet(context)
				.AsNoTracking()
				.Where(x => x.Login == login && x.DeletedOn == null)
				.FirstOrDefaultAsync()
				.ConfigureAwait(continueOnCapturedContext: false);

			if(user == null)
			{
				return null;
			}

			if(!BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
			{
				throw new PasswordInvalidException();
			}

			var result = Mapper.Map<UserDto>(user);
			return result;
		}

		public async Task<UserDto> CreateAsync(UserDto newUser, string password, CancellationToken cancellationToken = default)
		{
			using var context = ContextFactory.Create();

			var user = Mapper.Map<UserEntity>(newUser);
			PrepareEntityBeforeInsert(user);
			cancellationToken.ThrowIfCancellationRequested();
			
			var hash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 12);
			user.Password = hash;

			GetDbSet(context).Add(user);
			await context.SaveChangesAsync(cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);

			return Mapper.Map<UserDto>(user);
		}

		public async Task<UserDto> UpdateAsync(UserDto data, CancellationToken cancellationToken = default)
		{
			using var context = ContextFactory.Create();

			var user = await GetDbSet(context).Where(x => x.Id == data.Id)
				.FirstOrDefaultAsync(cancellationToken);
			if(user == null) 
			{
				throw new NotFoundException(data?.Id ?? 0, "user", "not found");
			}

			if(user.Login != data.Login)
			{
				user.Login = data.Login;
			}

			if(user.Name != data.Name) 
			{
				user.Name = data.Name;
			}

			if(user.Role != data.Role) 
			{
				user.Role = data.Role;
			}

			user.ModifiedOn = DateTime.UtcNow;
			await context.SaveChangesAsync(cancellationToken)
					.ConfigureAwait(continueOnCapturedContext: false);

			return Mapper.Map<UserDto>(user);
		}

		public async Task<UserDto> UpdatePasswordAsync(long id, string password, CancellationToken cancellationToken = default) 
		{
			using var context = ContextFactory.Create();

			var user = await GetDbSet(context).Where(x => x.Id == id)
				.FirstOrDefaultAsync(cancellationToken);
			if(user == null)
			{
				throw new NotFoundException(id, "user", "not found");
			}

			var hash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 12);
			user.Password = hash;

			await context.SaveChangesAsync(cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);

			return Mapper.Map<UserDto>(user);
		}
	}
}
