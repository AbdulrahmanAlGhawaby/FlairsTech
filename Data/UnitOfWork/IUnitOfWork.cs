using Data.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.UnitOfWork
{
    public interface IUnitOfWork<TContext> where TContext : DbContext
    {
        IRepository<T> GetRepository<T>() where T : class, new();
        void Dispose();
        int SaveChanges(string userId = "");
    }
}
