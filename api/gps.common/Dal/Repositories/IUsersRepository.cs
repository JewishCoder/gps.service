using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gps.common.Dto;

namespace gps.common.Dal.Repositories
{
	public interface IUsersRepository : IRepository<UserDto>
	{
		Task<UserDto> LoginAsync(string login, string password);

		Task<UserDto> CreateAsync(UserDto newUser, string password, CancellationToken cancellationToken = default);

		Task<UserDto> UpdateAsync(UserDto data, CancellationToken cancellationToken = default);
	}
}
