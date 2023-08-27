using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Linq;
using Data.Repository;

namespace Data.UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IDisposable where TContext : DbContext
    {

        public UnitOfWork(TContext dbContext)
        {
            Dbcontext = dbContext;
        }

        private DbContext Dbcontext { get; set; }
        public IRepository<T> GetRepository<T>() where T : class, new()
        {
            return new Repository<T>(Dbcontext);
        }

        public void Dispose()
        {
            Dbcontext.Dispose();
        }

        public int SaveChanges(string userId = "")
        {
            int isSaved;
            var entities = from e in Dbcontext.ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e.Entity;
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(entity, validationContext);
            }


            isSaved = Dbcontext.SaveChanges();


            return isSaved;
        }
    }
}
