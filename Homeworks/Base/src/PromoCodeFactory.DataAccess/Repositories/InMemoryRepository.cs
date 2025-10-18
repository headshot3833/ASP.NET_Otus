using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IList<T> Data { get; set; }

        public InMemoryRepository(IList<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> CreateUser(T entity)
        {
            Data.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<T> UpdateUser(T entity)
        {
            var User = Data.FirstOrDefault(x=> x.Id == entity.Id);
            if (User == null)
                return Task.FromResult<T>(null);

            Data.Remove(User);
            Data.Add(entity);
            return Task.FromResult(entity);

        }

        public Task<T> DeleteUser(Guid id)
        {
            var user = Data.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return Task.FromResult<T>(null);
            Data.Remove(user);
            return Task.FromResult<T>(user);
        }
    }
}