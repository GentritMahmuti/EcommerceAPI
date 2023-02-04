

using Persistence.Repository.IRepository;

namespace Persistence.UnitOfWork.IUnitOfWork
{
    public interface IUnitOfWork
    {
        public IECommerceRepository<TEntity> Repository<TEntity>() where TEntity : class;
        bool Complete();
        Task<bool> CompleteAsync();
    }
}
