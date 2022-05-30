using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ebill.Data.Repository.Interface;

namespace ebill.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _entities;
        protected readonly ILogger _log;

        public Repository(DbContext context, ILogger<dynamic> log)
        {
            _context = context;
            _entities = context.Set<TEntity>();
            _log = log;
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            // await _context.AddAsync(entity);
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            // In case AsNoTracking is used
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        // public virtual TEntity Add(TEntity entity)
        // {
        //     try
        //     {
        //         _entities.Add(entity);
        //         _context.SaveChanges();
        //         return entity;
        //     }
        //     catch (Exception ex)
        //     {
        //         _log.LogInformation(ex.Message + " : " + ex.InnerException + " : " + ex.StackTrace);
        //         return null;
        //     }

        // }

        public virtual IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            try
            {
                _entities.AddRange(entities);
                _context.SaveChanges();
                return entities;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message + " : " + ex.InnerException + " : " + ex.StackTrace);
                return null;
            }
        }


        // public virtual TEntity Update(TEntity entity)
        // {
        //     try
        //     {
        //         _entities.Update(entity);
        //         _context.SaveChanges();
        //         return entity;
        //     }
        //     catch (Exception ex)
        //     {
        //         _log.LogInformation(ex.Message + " : " + ex.InnerException + " : " + ex.StackTrace);
        //         return null;
        //     }
        // }

        public virtual IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities)
        {
            try
            {
                _entities.UpdateRange(entities);
                _context.SaveChanges();
                return entities;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message + " : " + ex.InnerException + " : " + ex.StackTrace);
                return null;
            }
        }



        public virtual bool Remove(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                _entities.Remove(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message + " : " + ex.InnerException + " : " + ex.StackTrace);
                return false;
            }
        }

        public virtual bool RemoveRange(IEnumerable<TEntity> entities)
        {
            try
            {
                _entities.RemoveRange(entities);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message + " : " + ex.InnerException + " : " + ex.StackTrace);
                return false;
            }
        }


        public virtual int Count()
        {
            return _entities.Count();
        }


        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.Where(predicate).ToList();
        }

        public virtual TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.FirstOrDefault(predicate);
        }

        public virtual TEntity Get(int id)
        {
            return _entities.Find(id);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _entities.ToList();
        }
    }

}
