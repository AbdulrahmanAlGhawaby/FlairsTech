using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IRepository<T> where T : class
    {
        T Find(byte id, params Expression<Func<T, object>>[] navigationProperties);
        T Find(object id, params Expression<Func<T, object>>[] navigationProperties);

        T Find(byte id);
        T Find(object id);
        T Add(T entity);
        void AddRange(List<T> lsstEntities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(List<T> lstEntities);
        List<T> GetAll(params Expression<Func<T, object>>[] navigationProperties);

        IQueryable<T> GetListItems();
        IQueryable<T> GetListItems(Expression<Func<T, bool>> whereCondition);
        IQueryable<T> GetListItems(params Expression<Func<T, object>>[] navigationProperties);
        IQueryable<T> GetListItems(Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] navigationProperties);

        IQueryable<T> GetListItems(string userId);
        IQueryable<T> GetListItems(string userId, Expression<Func<T, bool>> whereCondition);
        IQueryable<T> GetListItems(string userId, params Expression<Func<T, object>>[] navigationProperties);
        IQueryable<T> GetListItems(string userId, Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] navigationProperties);
        T First(Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] navigationProperties);
        int Max(Func<T, int> p);
        int Count();
        bool Any(Expression<Func<T, bool>> whereCondition);
    }
}
