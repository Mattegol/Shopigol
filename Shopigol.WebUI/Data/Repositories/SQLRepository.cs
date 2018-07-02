using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;

namespace Shopigol.WebUI.Data.Repositories
{
    public class SqlRepository<T> : IRepository<T> where T: BaseEntity
    {
        public ApplicationDbContext Context;
        public DbSet<T> DbSet;

        public SqlRepository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        public void Add(T t)
        {
            DbSet.Add(t);
        }

        public IQueryable<T> Collection()
        {
            return DbSet;
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        public void Delete(string id)
        {
            var t = Find(id);
            if (Context.Entry(t).State == EntityState.Detached)
                DbSet.Attach(t);

            DbSet.Remove(t);
        }

        public T Find(string id)
        {
            return DbSet.Find(id);
        }

        public void Update(T t)
        {
            DbSet.Attach(t);
            Context.Entry(t).State = EntityState.Modified;
        }
    }
}
