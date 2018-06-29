using Shopigol.Core.Models;
using System.Linq;

namespace Shopigol.Core.Contracts
{
    public interface IRepository<T> where T : BaseEntity
    {
        void Add(T t);

        IQueryable<T> Collection();

        void Commit();

        void Delete(string id);

        T Find(string id);

        void Update(T t);
    }
}