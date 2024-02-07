using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.DataAccess.Abstractions
{
    public interface IRepository<T> where T : class
    {
        T Find(long id);
        T Find(int id);
        IEnumerable<T> GetAll();
        IQueryable<T> Table { get; }

        T Find(Expression<Func<T, bool>> predicate);
        void Insert(T entity, bool saveNow = true);
        void InsertRange(IEnumerable<T> entities, bool saveNow = true);

        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? page = null, int? pageSize = null);

        int Count(Expression<Func<T, bool>> predicate);

        void Update(T entity, bool isSaveNow = true);
        void UpdateRange(IEnumerable<T> entities, bool isSaveNow = true);

        void Delete(object entity, bool saveNow = true);

        void Delete(T entity, bool saveNow = true);
        void DeleteRange(IEnumerable<T> entities, bool save = true);


        IRepository<T> GetRepository<T>() where T : class;

        IQueryable<T> Query();

        IQueryable<T> Query(Expression<Func<T, bool>> predicate);

        IQueryable<T> AsNoTrackingQuery(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query(params Expression<Func<T, object>>[] includeProperties);
    }

    public interface IRepositoryAsync<T> : IRepository<T> where T : class
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<T> FindAsync(params object[] keyValues);
        Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task<bool> DeleteAsync(params object[] keyValues);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task InsertAsync(T entity, bool saveNow = true);
        Task InsertRangeAsync(IEnumerable<T> entities, bool saveNow = true);
        Task UpdateAsync(T entity, bool saveNow = true);

        Task<T> FindAsync(Expression<Func<T, bool>> predicate);


        Task UpdateRangeAsync(IEnumerable<T> entities, bool saveNow = true);
    }
}
