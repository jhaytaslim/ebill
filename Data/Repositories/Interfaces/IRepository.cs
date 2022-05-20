using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ebill.Data.Models;
using System.Linq.Expressions;

namespace ebill.Data.Repository.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Add(TEntity entity);
        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);

        TEntity Update(TEntity entity);
        IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities);

        bool Remove(TEntity entity);
        bool RemoveRange(IEnumerable<TEntity> entities);

        int Count();

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        TEntity Get(int id);

        IEnumerable<TEntity> GetAll();
    }

    public interface IItemRepository : IRepository<Item>
    {

    }

    public interface IProductRepository : IRepository<Product>
    {

    }

    public interface ISettingsRepository : IRepository<Settings>
    {

    }

}
