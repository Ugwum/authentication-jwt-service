using Microsoft.EntityFrameworkCore;
using Prospa.AuthService.Core.DataAccess.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.DataAccess.Repository
{
    public class Repository<T> : IRepository<T>, IRepositoryAsync<T> where T : class
    {
        private readonly ProspaDBContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ProspaDBContext context)
        {
            _context = (ProspaDBContext?)context;
            _dbSet = context.Set<T>();
        }

        public T Find(long id)
        {
            return _dbSet.Find(id);
        }

        public T Find(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public IQueryable<T> Table => _dbSet;

        public T Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public void Insert(T entity, bool saveNow = true)
        {
            _dbSet.Add(entity);
            if (saveNow)
                SaveChanges();
        }

        public void InsertRange(IEnumerable<T> entities, bool saveNow = true)
        {
            _dbSet.AddRange(entities);
            if (saveNow)
                SaveChanges();
        }

        public IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? page = null, int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;
            if (predicate != null)
                query = query.Where(predicate);
            if (orderBy != null)
                query = orderBy(query);
            if (page.HasValue && pageSize.HasValue)
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            return query;
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Count(predicate);
        }

        public void Update(T entity, bool isSaveNow = true)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            if (isSaveNow)
                SaveChanges();
        }

        public void UpdateRange(IEnumerable<T> entities, bool isSaveNow = true)
        {
            foreach (var entity in entities)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
            if (isSaveNow)
                SaveChanges();
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public void Delete(object entity, bool saveNow = true)
        {
            _dbSet.Remove(entity as T);
            if (saveNow)
                SaveChanges();
        }

        public void Delete(T entity, bool saveNow = true)
        {
            _dbSet.Remove(entity);
            if (saveNow)
                SaveChanges();
        }

        public void DeleteRange(IEnumerable<T> entities, bool save = true)
        {
            _dbSet.RemoveRange(entities);
            if (save)
                SaveChanges();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<T> FindAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        public async Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await _dbSet.FindAsync(cancellationToken, keyValues);
        }

        public async Task<bool> DeleteAsync(params object[] keyValues)
        {
            var entity = await FindAsync(keyValues);
            if (entity != null)
            {
                Delete(entity);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = await FindAsync(cancellationToken, keyValues);
            if (entity != null)
            {
                Delete(entity);
                return true;
            }
            return false;
        }

        public IQueryable<T> Query()
        {
            return _context.Set<T>();
        }


        public IQueryable<T> Query(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            return query;
        }

        public IQueryable<T> AsNoTrackingQuery(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking().Where(predicate);
            return query;
        }

        public IQueryable<T> Query(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }
        public async Task InsertAsync(T entity, bool saveNow = true)
        {
            _dbSet.Add(entity);
            if (saveNow)
                await SaveChangesAsync();
        }

        public async Task InsertRangeAsync(IEnumerable<T> entities, bool saveNow = true)
        {
            _dbSet.AddRange(entities);
            if (saveNow)
                await SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity, bool saveNow = true)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            if (saveNow)
                await SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, bool saveNow = true)
        {
            foreach (var entity in entities)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
            if (saveNow)
                await SaveChangesAsync();
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
    }
}
