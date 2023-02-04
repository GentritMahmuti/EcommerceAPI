using System.Linq.Expressions;

namespace DataAccess.Repository.IRepository
{
    public interface IECommerceRepository<Tentity> where Tentity : class
    {
        IQueryable<Tentity> GetByCondition(Expression<Func<Tentity, bool>> expression);
        IQueryable<Tentity> GetByConditionWithIncludes(Expression<Func<Tentity, bool>> expression, string? includeRelations = null);

        IQueryable<Tentity> GetByConditionPaginated(Expression<Func<Tentity, bool>> expression, Expression<Func<Tentity, object>> orderBy, int page, int pageSize, bool orderByDescending = true);


        IQueryable<Tentity> GetAll();

        IQueryable<Tentity> GetById(Expression<Func<Tentity, bool>> expression);

        void Create(Tentity entity);
        void CreateRange(List<Tentity> entity);

        void CreateRangeList(List<List<Tentity>> entities);

        void Delete(Tentity entity);
        void DeleteRange(List<Tentity> entity);

        void Update(Tentity entity);
        void UpdateRange(List<Tentity> entity);
        int Count(Expression<Func<Tentity, bool>> expression);
    }
}
