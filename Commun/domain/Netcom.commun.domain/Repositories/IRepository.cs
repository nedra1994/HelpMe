using HelpMe.commun.domain.Data.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace HelpMe.commun.domain.Data.Repositories
{
    public interface IRepository<T> 
    {

        IUnitOfWork UnitOfWork { get; }
        Task<T> GetById(int id);
        Task<List<T>> ListAll();
        Task<T> GetSingleBySpec(ISpecification<T> spec);
        Task<List<T>> List(ISpecification<T> spec);
        T Add(T entity);
        T Update(T entity);
        void Delete(T entity);
    }
}
