using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.DataBaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DataBaseContextSqlLite _dataBaseContextSqlLite;
        private readonly DbSet<T> _dbSet;

        public EfRepository(DataBaseContextSqlLite dataBaseContextSqlLite, DbSet<T> dbSet)
        {
            _dataBaseContextSqlLite = dataBaseContextSqlLite;
            _dbSet = _dataBaseContextSqlLite.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(T Entity)
        {
            await _dbSet.AddAsync(Entity);
            await _dataBaseContextSqlLite.SaveChangesAsync();
        }

        public async Task UpdateAsync(T Entity)
        {
            _dbSet.Update(Entity);
            _dataBaseContextSqlLite.SaveChanges();
        }

        public async Task Delete(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _dataBaseContextSqlLite.SaveChanges();
            }
        }
    }
}
