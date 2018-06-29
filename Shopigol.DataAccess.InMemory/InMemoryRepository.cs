using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Shopigol.DataAccess.InMemory
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ObjectCache _cache = MemoryCache.Default;
        private readonly List<T> _items;
        private readonly string _className;

        public InMemoryRepository()
        {
            _className = typeof(T).Name;
            _items = _cache[_className] as List<T> ?? new List<T>();
        }

        public void Commit()
        {
            _cache[_className] = _items;
        }

        public void Add(T t)
        {
            _items.Add(t);
        }

        public void Update(T t)
        {
            T tToUpdate = _items.Find(i => i.Id == t.Id);

            if (tToUpdate != null)
            {
                tToUpdate = t;
            }
            else
            {
                throw new Exception(_className + " not found");
            }
        }

        public T Find(string id)
        {
            T t = _items.Find(i => i.Id == id);

            if (t != null)
            {
                return t;
            }
            throw new Exception(_className + " not found");
        }

        public IQueryable<T> Collection()
        {
            return _items.AsQueryable();
        }

        public void Delete(string id)
        {
            T tToDelete = _items.Find(i => i.Id == id);

            if (tToDelete != null)
            {
                _items.Remove(tToDelete);
            }
            else
            {
                throw new Exception(_className + " not found");
            }
        }
    }
}
