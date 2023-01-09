using System.Linq.Expressions;
using Kirel.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kirel.Repositories.Infrastructure.Generics;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly DbContext _dbContext;
    
    public GenericRepository(DbContext context)
    {
        _dbContext = context;
    }
    
    public async Task<TEntity> Insert(TEntity entity)
    {
         await _dbContext.Set<TEntity>().AddAsync(entity);
         await _dbContext.SaveChangesAsync();
         return entity;
    }
    public Task<TEntity?> GetById(int id)
    {
        var entity = _dbContext.Set<TEntity>().FindAsync(id);
        return entity.AsTask();
    }
    public async Task Delete(int id)
    {
        var entity = await GetById(id);
        if (entity == null) return;
        _dbContext.Set<TEntity>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<TEntity> Update(TEntity entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        return entity;
    }
    public async Task<IEnumerable<TEntity>> GetList(Expression<Func<TEntity, bool>> expression)
    {
        return await _dbContext.Set<TEntity>().Where(expression).ToListAsync();
    }
    public Task<int> Count(Expression<Func<TEntity, bool>> expression)
    {
        return _dbContext.Set<TEntity>().CountAsync(expression);
    }
    public Task<int> Count()
    {
        return _dbContext.Set<TEntity>().CountAsync();
    }
}