
using AutoMapper;

using gps.common.Dal;
using gps.common.Dal.Repositories;
using gps.common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace gps.dal.Repositories
{
	abstract class RepositoryBase<TEntity, TEntityDto> : IRepository<TEntityDto>
		where TEntity : class, IEntity
		where TEntityDto : class
	{
		protected ContextFactory ContextFactory { get; }

		protected IMapper Mapper { get; }

		public RepositoryBase(ContextFactory contextFactory, IMapper mapper)
		{
			ContextFactory = contextFactory;
			Mapper = mapper;
		}

		protected virtual DbSet<TEntity> GetDbSet(Context context) => context.Set<TEntity>();

		protected virtual void PrepareEntityBeforeInsert(TEntity entity)
		{
			if(entity is ICreatedEntity createdEntity)
			{
				createdEntity.CreateOn = DateTime.UtcNow;
			}
		}

		protected virtual void ValidateEntityAfterInsert(TEntity entity)
		{

		}

		public virtual IQueryBuilder<TEntityDto> GetFetchBuilder()
		{
			return new QueryBuilder<TEntity, TEntityDto>(ContextFactory.Create(), Mapper);
		}

		public virtual async Task<TEntityDto> InsertAsync(TEntityDto data, CancellationToken cancellationToken = default)
		{
			using var context = ContextFactory.Create();
			var entity = Mapper.Map<TEntity>(data);
			var dbSet = GetDbSet(context);
			PrepareEntityBeforeInsert(entity);


			dbSet.Add(entity);

			try
			{
				await context.SaveChangesAsync(cancellationToken)
					.ConfigureAwait(continueOnCapturedContext: false);
				ValidateEntityAfterInsert(entity);
				return Mapper.Map<TEntityDto>(entity);
			}
			catch(DbUpdateConcurrencyException exc)
			{
				throw new UpdateConcurrencyException(exc);
			}
		}

		public virtual async Task<int> DeleteAsync(long id, CancellationToken cancellationToken = default)
		{
			using var context = ContextFactory.Create();
			var dbSet = GetDbSet(context);
			var entity = await dbSet
				.Where(x => x.Id == id)
				.FirstOrDefaultAsync(cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);

			if(entity == null)
			{
				throw new NotFoundException(id, typeof(TEntity).Name, "not found entity");
			}

			if(entity is IDeletedEntity deletedEntity)
			{
				deletedEntity.DeletedOn = DateTime.UtcNow;
			}
			else
			{
				dbSet.Remove(entity);
			}

			try
			{
				return await context.SaveChangesAsync(cancellationToken)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			catch(DbUpdateConcurrencyException exc)
			{
				throw new UpdateConcurrencyException(exc);
			}
		}
	}
}
