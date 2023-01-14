using EcommerceAPI.Data.Repository.IRepository;

namespace EcommerceAPI.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IECommerceRepository<TEntity> Repository<TEntity>() where TEntity : class;
        bool Complete();
        Task<bool> CompleteAsync();
    }
}
