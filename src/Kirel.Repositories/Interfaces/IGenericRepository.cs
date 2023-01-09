using System.Linq.Expressions;

namespace Kirel.Repositories.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    public Task<TEntity> Insert(TEntity entity);
    public Task<TEntity?> GetById(int id);
    public Task Delete(int id);
    public Task<TEntity> Update(TEntity entity);
    public Task<IEnumerable<TEntity>> GetList(Expression<Func<TEntity, bool>> expression);
    public Task<int> Count(Expression<Func<TEntity, bool>> expression);
    public Task<int> Count();
}