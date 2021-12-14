using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gps.common.Dto;

namespace gps.common.Dal.Repositories
{
	public interface IPointsRepository : IRepository<PointDto>
	{
		Task<PointDto> UpdateAsync(PointDto data, CancellationToken cancellationToken = default);
	}
}
