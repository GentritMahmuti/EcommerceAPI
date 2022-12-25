using EcommerceAPI.Data.Repository.IRepository;
using System.Linq.Expressions;

namespace EcommerceAPI.Data.Repository
{
    public class ECommerceRepository<Tentity> : IECommerceRepository<Tentity> where Tentity : class
    {
        private readonly EcommerceDbContext _dbContext;

        public ECommerceRepository(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Tentity> GetByCondition(Expression<Func<Tentity, bool>> expression)
        {
            return _dbContext.Set<Tentity>().Where(expression);
        }

        public IQueryable<Tentity> GetByConditionPaginated(Expression<Func<Tentity, bool>> expression, Expression<Func<Tentity, object>> orderBy, int page, int pageSize, bool orderByDescending = true)
        {
            const int defaultPageNumber = 1;

            var query = _dbContext.Set<Tentity>().Where(expression);

            // Check if the page number is greater then zero - otherwise use default page number
            if (page <= 0)
            {
                page = defaultPageNumber;
            }

            // It is necessary sort items before it
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IQueryable<Tentity> GetAll()
        {
            var result = _dbContext.Set<Tentity>();

            return result;
        }

        public IQueryable<Tentity> GetById(Expression<Func<Tentity, bool>> expression)
        {
            return _dbContext.Set<Tentity>().Where(expression);
        }

        public void Create(Tentity entity)
        {
            _dbContext.Set<Tentity>().Add(entity);
        }

        public void CreateRange(List<Tentity> entities)
        {
            _dbContext.Set<Tentity>().AddRange(entities);
        }

        public void Delete(Tentity entity)
        {
            _dbContext.Set<Tentity>().Remove(entity);
        }

        public void DeleteRange(List<Tentity> entities)
        {
            _dbContext.Set<Tentity>().RemoveRange(entities);
        }

        public void Update(Tentity entity)
        {
            _dbContext.Set<Tentity>().Update(entity);
        }

        public void UpdateRange(List<Tentity> entities)
        {
            _dbContext.Set<Tentity>().UpdateRange(entities);
        }

        public IQueryable<Tentity> GetByConditionWithIncludes(Expression<Func<Tentity, bool>> expression, string? includeRelations = null)
        {
            var query = _dbContext.Set<Tentity>().Where(expression);

            if (!string.IsNullOrEmpty(includeRelations))
            {
                var relations = includeRelations.Split(", ");

                foreach (var relation in relations)
                {
                    query = query.Include(relation);
                }
            }

            return query;
        }
    }
}
