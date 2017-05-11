using HealthCatalyst.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace HealthCatalyst.DataAccess.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        void Add(T entity);
    }

    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IContext _context;

        public Repository(IContext context)
        {
            _context = context;
        }

        public IEnumerable<T> GetAll()
        {
            return this._context.Set<T>().ToList();
        }

        public void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            this._context.Set<T>().Add(entity);
            this._context.SaveChanges();
        }
    }
}