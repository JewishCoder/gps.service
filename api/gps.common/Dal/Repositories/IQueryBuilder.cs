using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace gps.common.Dal.Repositories
{
	public interface IQueryBuilder<TEntityDto> : IDisposable
		where TEntityDto : class
	{
		IQueryBuilder<TEntityDto> Where(Expression<Func<TEntityDto, bool>> expression);

		IQueryBuilder<TEntityDto> Include<T>(Expression<Func<TEntityDto, T>> expression) where T : class;

		IQueryBuilder<TEntityDto> OrderBy<T>(Expression<Func<TEntityDto, T>> expression, bool asc = true);

		IQueryBuilder<TEntityDto> Skip(int count);

		IQueryBuilder<TEntityDto> Take(int limit);

		IQueryable<T> Select<T>(Expression<Func<TEntityDto, T>> expression);

		IEnumerable<TEntityDto> Execute();

		Task<IEnumerable<TEntityDto>> ExecuteAsync(CancellationToken cancellationToken = default);
	}
}
