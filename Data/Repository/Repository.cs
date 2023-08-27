using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repository
{
    public class Repository<T> : IDisposable, IRepository<T> where T : class
    {
        protected DbContext DbContext { get; set; }
        protected DbSet<T> DbSet { get; set; }

        public Repository(DbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }

        private T LoadNavigationProperties(T entity, params Expression<Func<T, object>>[] navigationProperties)
        {
            if (navigationProperties != null)
            {
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                {
                    if (navigationProperty.AsPath().Split('.').Count() > 1 && navigationProperty.ToString().ToLower().Contains("select"))
                    {
                        var collectionEntry = DbContext.Entry(entity).Collection(navigationProperty.AsPath().Split('.')[0]);
                        collectionEntry.Load();
                        foreach (var col1 in collectionEntry.CurrentValue)
                        {
                            var entry = DbContext.Entry(col1).Reference(navigationProperty.AsPath().Split('.')[1]);
                            entry.Load();

                            if (navigationProperty.AsPath().Split('.').Length > 2)
                            {
                                var lstNavProp = navigationProperty.AsPath().Split('.').ToList().Skip(2);
                                foreach (var nav in lstNavProp)
                                {
                                    DbContext.Entry(entry.CurrentValue).Reference(nav).Load();
                                }

                            }

                        }
                    }
                    else if (navigationProperty.AsPath().Split('.').Count() > 1 && !navigationProperty.ToString().ToLower().Contains("select"))
                    {
                        var entry = DbContext.Entry(entity).Reference(navigationProperty.AsPath().Split('.')[0]);
                        entry.Load();
                        if (entry.CurrentValue != null)
                        {
                            DbContext.Entry(entry.CurrentValue).Reference(navigationProperty.AsPath().Split('.')[1]).Load();
                        }
                    }
                    else
                    {
                        try
                        {
                            DbContext.Entry(entity).Reference(navigationProperty).Load();
                        }
                        catch (Exception ex)
                        {
                            DbContext.Entry(entity).Collection(navigationProperty.AsPath()).Load();
                        }
                    }
                }
            }

            return entity;
        }

        public T Find(byte id, params Expression<Func<T, object>>[] navigationProperties)
        {
            var entity = DbContext.Set<T>().Find(id);
            entity = LoadNavigationProperties(entity, navigationProperties);
            return entity;
        }

        public T Find(object id, params Expression<Func<T, object>>[] navigationProperties)
        {
            var entity = DbContext.Set<T>().Find(id);

            entity = LoadNavigationProperties(entity, navigationProperties);

            return entity;
        }

        public T Find(byte id)
        {
            var entity = DbContext.Set<T>().Find(id);
            return entity;
        }

        public T Find(object id)
        {
            var entity = DbContext.Set<T>().Find(id);
            return entity;
        }

        public T First(Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] navigationProperties)
        {
            return GetList(null, whereCondition, navigationProperties).FirstOrDefault();
        }

        public T Add(T entity)
        {
            return DbContext.Set<T>().Add(entity).Entity;
        }

        public void AddRange(List<T> lstEntities)
        {
            DbContext.Set<T>().AddRange(lstEntities);
        }

        public void Update(T entity)
        {
            if (entity == null)
                return;

            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public List<T> GetAll(params Expression<Func<T, object>>[] navigationProperties)
        {
            DbContext.Set<T>().Where(s => true);

            IQueryable<T> dbQuery = DbContext.Set<T>();

            if (navigationProperties != null && navigationProperties.Count() != 0)
            {
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                    //dbQuery = dbQuery.Include<T, object>(navigationProperty);
                    dbQuery = dbQuery.Include(navigationProperty.AsPath());
            }

            var entity = dbQuery.ToList();
            return entity;
        }

        public IQueryable<T> GetListItems()
        {
            return GetListItems(null, null, null);
        }
        public IQueryable<T> GetListItems(Expression<Func<T, bool>> whereCondition)
        {
            return GetList(null, whereCondition, null);
        }
        public IQueryable<T> GetListItems(params Expression<Func<T, object>>[] navigationProperties)
        {
            return GetList(null, null, navigationProperties);
        }
        public IQueryable<T> GetListItems(Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] navigationProperties)
        {
            return GetList(null, whereCondition, navigationProperties);
        }
        public IQueryable<T> GetListItems(string userId)
        {
            return GetList(userId, null, null);
        }
        public IQueryable<T> GetListItems(string userId, Expression<Func<T, bool>> whereCondition)
        {
            return GetList(userId, whereCondition, null);
        }
        public IQueryable<T> GetListItems(string userId, params Expression<Func<T, object>>[] navigationProperties)
        {
            return GetList(userId, null, navigationProperties);
        }
        public IQueryable<T> GetListItems(string userId, Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] navigationProperties)
        {
            return GetList(userId, whereCondition, navigationProperties);
        }
        private IQueryable<T> GetList(string userId, Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] navigationProperties)// where T : class
        {
            //TODO check if exist in cache before load from DB
            DbContext.Set<T>().Where(s => true);

            IQueryable<T> dbQuery = DbContext.Set<T>();

            if (navigationProperties != null)
            {
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                    //dbQuery = dbQuery.Include<T, object>(navigationProperty);
                    dbQuery = dbQuery.Include(navigationProperty.AsPath());
            }

            if (whereCondition != null)
            {
                dbQuery = dbQuery.AsNoTracking().Where(whereCondition).AsQueryable<T>();
            }

            else
                dbQuery = dbQuery.AsNoTracking();

            return dbQuery;
        }

        public void Remove(T entity)
        {
            if (entity == null)
                return;

            DbContext.Entry(entity).State = EntityState.Deleted;
        }

        public void RemoveRange(List<T> lstEntities)
        {
            DbContext.Set<T>().RemoveRange(lstEntities);
        }

        public int Max(Func<T, int> p)
        {
            return DbSet.Max(p);
        }

        public int Count()
        {
            return DbSet.Count();
        }
        public bool Any(Expression<Func<T, bool>> whereCondition)
        {
            return DbSet.Any(whereCondition);
        }
    }

    /// <summary>
    ///     Provides extension methods to the <see cref="Expression" /> class.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        ///     Converts the property accessor lambda expression to a textual representation of it's path. <br />
        ///     The textual representation consists of the properties that the expression access flattened and separated by a dot character (".").
        /// </summary>
        /// <param name="expression">The property selector expression.</param>
        /// <returns>The extracted textual representation of the expression's path.</returns>
        public static string AsPath(this LambdaExpression expression)
        {
            if (expression == null)
                return null;

            TryParsePath(expression.Body, out var path);

            return path;
        }

        /// <summary>
        ///     Recursively parses an expression tree representing a property accessor to extract a textual representation of it's path. <br />
        ///     The textual representation consists of the properties accessed by the expression tree flattened and separated by a dot character (".").
        /// </summary>
        /// <param name="expression">The expression tree to parse.</param>
        /// <param name="path">The extracted textual representation of the expression's path.</param>
        /// <returns>True if the parse operation succeeds; otherwise, false.</returns>
        private static bool TryParsePath(Expression expression, out string path)
        {
            var noConvertExp = RemoveConvertOperations(expression);
            path = null;

            switch (noConvertExp)
            {
                case MemberExpression memberExpression:
                    {
                        var currentPart = memberExpression.Member.Name;

                        if (!TryParsePath(memberExpression.Expression, out var parentPart))
                            return false;

                        path = string.IsNullOrEmpty(parentPart) ? currentPart : string.Concat(parentPart, ".", currentPart);
                        break;
                    }

                case MethodCallExpression callExpression:
                    switch (callExpression.Method.Name)
                    {
                        case nameof(Queryable.Select) when callExpression.Arguments.Count == 2:
                            {
                                if (!TryParsePath(callExpression.Arguments[0], out var parentPart))
                                    return false;

                                if (string.IsNullOrEmpty(parentPart))
                                    return false;

                                if (!(callExpression.Arguments[1] is LambdaExpression subExpression))
                                    return false;

                                if (!TryParsePath(subExpression.Body, out var currentPart))
                                    return false;

                                if (string.IsNullOrEmpty(parentPart))
                                    return false;

                                path = string.Concat(parentPart, ".", currentPart);
                                return true;
                            }

                        case nameof(Queryable.Where):
                            throw new NotSupportedException("Filtering an Include expression is not supported");
                        case nameof(Queryable.OrderBy):
                        case nameof(Queryable.OrderByDescending):
                            throw new NotSupportedException("Ordering an Include expression is not supported");
                        default:
                            return false;
                    }
            }

            return true;
        }

        /// <summary>
        ///     Removes all casts or conversion operations from the nodes of the provided <see cref="Expression" />.
        ///     Used to prevent type boxing when manipulating expression trees.
        /// </summary>
        /// <param name="expression">The expression to remove the conversion operations.</param>
        /// <returns>The expression without conversion or cast operations.</returns>
        private static Expression RemoveConvertOperations(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
                expression = ((UnaryExpression)expression).Operand;

            return expression;
        }
    }
}
