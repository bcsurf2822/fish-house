using FishReportApi.Data;
using FishReportApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FishReportApi.Repositories
{
    public class GenericRepository<T> : ICommonRepository<T> where T : class
    {
        private readonly FishDBContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(FishDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> CreateAsync(T entity)
        {
            // Aurora DSQL doesn't support auto-increment, so generate ID manually
            var idProperty = _context.Entry(entity).Property("Id");
            if (idProperty != null && (int)idProperty.CurrentValue == 0)
            {
                // Get next ID using MAX(Id) + 1
                var maxId = await _dbSet.AnyAsync()
                    ? await _dbSet.MaxAsync(e => EF.Property<int>(e, "Id"))
                    : 0;

                idProperty.CurrentValue = maxId + 1;
                Console.WriteLine($"[GenericRepository-CreateAsync] Generated ID {idProperty.CurrentValue} for {typeof(T).Name}");
            }

            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            var existingEntity = await GetByIdAsync((int)_context.Entry(entity).Property("Id").CurrentValue);
            if (existingEntity == null) return false;

            _context.Entry(entity).State = EntityState.Modified;
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}