using EcommerceAPI.Data.Repository;
using EcommerceAPI.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections;
using System.Data.Entity.Infrastructure;

namespace EcommerceAPI.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcommerceDbContext _context;

        private Hashtable _repositories;

        public UnitOfWork(EcommerceDbContext context)
        {
            _context = context;
        }
       
        public bool Complete()
        {
            var numberOfAffectedRows = _context.SaveChanges();
            return numberOfAffectedRows > 0;
        }

        public async Task<bool> CompleteAsync()
        {
            var numberOfAffectedRows = await _context.SaveChangesAsync();
            return numberOfAffectedRows > 0;
        }

        public IECommerceRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Hashtable();

            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(ECommerceRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

                _repositories.Add(type, repositoryInstance);
            }
            return (IECommerceRepository<TEntity>)_repositories[type];
        }

    }
}
