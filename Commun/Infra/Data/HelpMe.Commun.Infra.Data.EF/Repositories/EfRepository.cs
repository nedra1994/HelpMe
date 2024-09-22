using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HelpMe.commun.domain.Data;
using HelpMe.commun.domain.Data.Repositories;
using HelpMe.commun.domain.Data.Specifications;
using HelpMe.commun.domain.Seedwork;

namespace HelpMe.Commun.Infra.Data.EF.Repositories
{

    public abstract class EfRepository<T> : IRepository<T> 
        where T:class
        
    {
        protected readonly DbContext _appDbContext;

       

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _appDbContext as IUnitOfWork;
            }
        }
        public virtual T Save(T entity)
        { 
           var e = (entity as Entity);
           if ( e==null ||e.Id== default(long)) return Add(entity);
            return Update(entity);

            //  await _appDbContext.SaveChangesAsync();
        }
        //, IUnitOfWork
        protected EfRepository(DbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public virtual async Task<T> GetById(long id)
        {
            return await _appDbContext.Set<T>().FindAsync(id);
        }

        public virtual async   Task<List<T>> ListAll()
        {
            return await _appDbContext.Set<T>().ToListAsync();
        }

        public virtual async Task<T> GetSingleBySpec(ISpecification<T> spec)
        {
            var result = await List(spec);
            return result.FirstOrDefault();
        }

        public virtual async Task<List<T>> List(ISpecification<T> spec)
        {
            // fetch a Queryable that includes all expression-based includes
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(_appDbContext.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            // modify the IQueryable to include any string-based include statements
            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            // return the result of the query using the specification's criteria expression
            if (spec.Criteria != null)
            {
                return await secondaryResult
                              .Where(spec.Criteria)
                              .ToListAsync();
            }
            else
            {
                return await secondaryResult
                              .ToListAsync();
            }
        }


        public virtual T Add(T entity)
        {
            _appDbContext.Set<T>().Add(entity);
           // await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public virtual T Update(T entity)
        {
            _appDbContext.Entry(entity).State = EntityState.Modified;
            return entity;
          //  await _appDbContext.SaveChangesAsync();
        }

        public virtual void Delete(T entity)
        {
            _appDbContext.Set<T>().Remove(entity);
           // await _appDbContext.SaveChangesAsync();
        }

  
    }
}
