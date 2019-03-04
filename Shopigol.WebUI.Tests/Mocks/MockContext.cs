using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shopigol.WebUI.Tests.Mocks
{
    public class MockContext<T> : IRepository<T> where T : BaseEntity
    {

        private readonly List<T> _items;
        private readonly string _className;

        public MockContext()
        {
            _items = new List<T>();
        }

        public void Commit()
        {
            return;
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
