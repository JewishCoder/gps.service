
using AutoMapper;

using gps.common.Dal;
using gps.common.Dal.Repositories;
using gps.common.Dto;
using gps.common.Exceptions;
using gps.dal.Entities;

using Microsoft.EntityFrameworkCore;

namespace gps.dal.Repositories
{
	internal class PointsRepository : RepositoryBase<PointEntity, PointDto>, IPointsRepository, IPerDepenecy
	{
		public PointsRepository(ContextFactory contextFactory, IMapper mapper)
			: base(contextFactory, mapper)
		{
		}

		public async Task<PointDto> UpdateAsync(PointDto data, CancellationToken cancellationToken = default)
		{
			using var context = ContextFactory.Create();

			var point = await GetDbSet(context).Where(x => x.Id == data.Id)
				.FirstOrDefaultAsync(cancellationToken);
			if(point == null)
			{
				throw new NotFoundException(data?.Id ?? 0, "point", "not found");
			}

			var oldModified = point.ModifiedOn;
			if(point.Name != data.Name)
			{
				point.Name = data.Name;
				point.ModifiedOn = DateTime.UtcNow;
			}
			if(data.X != default && point.X != data.X)
			{
				point.X = data.X;
				point.ModifiedOn = DateTime.UtcNow;
			}
			if(data.Y != default && point.Y != data.Y)
			{
				point.Y = data.Y;
				point.ModifiedOn = DateTime.UtcNow;
			}

			if(oldModified != point.ModifiedOn)
			{
				await context.SaveChangesAsync(cancellationToken)
					.ConfigureAwait(continueOnCapturedContext: false);
			}

			return Mapper.Map<PointDto>(point);
		}
	}
}
