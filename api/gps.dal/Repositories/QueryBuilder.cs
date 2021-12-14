using System.Linq.Expressions;

using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;

using gps.common.Dal;
using gps.common.Dal.Repositories;

using Microsoft.EntityFrameworkCore;

namespace gps.dal.Repositories
{
	class QueryBuilder<TEntity, TEntityDto> : IQueryBuilder<TEntityDto>
		where TEntity : class, IEntity
		where TEntityDto : class
	{
		private readonly Func<bool> _hasDeletionFilter;
		private IQueryable<TEntity> _query;

		private IMapper Mapper { get; }

		private Context Context { get; }

		public bool IsDisposed { get; private set; }

		public QueryBuilder(Context context, IMapper mapper)
		{
			Context = context;
			Mapper = mapper;

			_hasDeletionFilter = () => typeof(TEntity).GetInterface(typeof(IDeletedEntity).Name) != null;
			Initialization();
		}

		public void Dispose()
		{
			if(IsDisposed) return;
			Disposed(true);
			GC.SuppressFinalize(this);
			IsDisposed = true;
		}

		protected virtual void Disposed(bool disposed)
		{
			if(disposed)
			{
				_query = null;
				Context.Dispose();
			}
		}

		protected virtual void Initialization()
		{
			_query = Context.Set<TEntity>().AsNoTracking();
			SetDefaultFilters();
		}

		public IQueryBuilder<TEntityDto> Where(Expression<Func<TEntityDto, bool>> expression)
		{
			var expr = Mapper.MapExpression<Expression<Func<TEntity, bool>>>(expression);

			_query = _query.Where(expr);
			return this;
		}

		public IQueryBuilder<TEntityDto> Include<T>(Expression<Func<TEntityDto, T>> expression) where T : class
		{
			if(expression.Body.NodeType == ExpressionType.MemberAccess)
			{
				var member = expression.Body as MemberExpression;
				_query = _query.Include(member.Member.Name);
			}

			return this;
		}

		public IQueryBuilder<TEntityDto> OrderBy<T>(Expression<Func<TEntityDto, T>> expression, bool asc = true)
		{
			var expr = Mapper.MapExpression<Expression<Func<TEntity, T>>>(expression);
			if(asc)
			{
				_query = _query.OrderBy(expr);
			}
			else
			{
				_query = _query.OrderByDescending(expr);
			}

			return this;
		}

		public IQueryBuilder<TEntityDto> Skip(int count)
		{
			_query = _query.Skip(count);
			return this;
		}

		public IQueryBuilder<TEntityDto> Take(int limit)
		{
			_query = _query.Take(limit);
			return this;
		}

		public IQueryable<T> Select<T>(Expression<Func<TEntityDto, T>> expression)
		{
			var expr = Mapper.MapExpression<Expression<Func<TEntity, T>>>(expression);
			return _query.Select(expr);
		}

		public IEnumerable<TEntityDto> Execute()
		{
			var result = _query.ToArray();
			return Mapper.Map<TEntityDto[]>(result);
		}

		public async Task<IEnumerable<TEntityDto>> ExecuteAsync(CancellationToken cancellationToken = default)
		{
			var result = await _query.ToArrayAsync(cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			return Mapper.Map<TEntityDto[]>(result);
		}

		private void SetDefaultFilters()
		{
			if(_hasDeletionFilter())
			{
				_query = _query.Where(x => ((IDeletedEntity)x).DeletedOn == null);
			}
		}
	}
}
