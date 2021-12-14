using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gps.common.Dal.Repositories
{
	public interface IRepository<TEntityDto>
		where TEntityDto : class
	{
		IQueryBuilder<TEntityDto> GetFetchBuilder();

		Task<TEntityDto> InsertAsync(TEntityDto data, CancellationToken cancellationToken = default);

		Task<int> DeleteAsync(long id, CancellationToken cancellationToken = default);
	}
}
