

using DataAccess.Repository.IRepository;

namespace DataAccess.UnitOfWork.IUnitOfWork
{
    public interface IUnitOfWork
    {
        public IECommerceRepository<TEntity> Repository<TEntity>() where TEntity : class;
        bool Complete();
        Task<bool> CompleteAsync();
    }
}
