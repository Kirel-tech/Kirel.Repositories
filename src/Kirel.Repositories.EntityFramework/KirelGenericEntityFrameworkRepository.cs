using System.Linq.Expressions;
using Kirel.Repositories.Core.Interfaces;
using Kirel.Repositories.Core.Models;
using Kirel.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kirel.Repositories.EntityFramework;

/// <summary>
/// Implementation of IKirelGenericEntityFrameworkRepository interface via Entity
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TDbContext"></typeparam>
public class KirelGenericEntityFrameworkRepository<TKey, TEntity, TDbContext> : IKirelGenericEntityRepository<TKey, TEntity> 
    where TEntity : class, ICreatedAtTrackedEntity, IKeyEntity<TKey>
    where TKey :  IComparable, IComparable<TKey>, IEquatable<TKey>
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    internal virtual IQueryable<TEntity> Reader
    {
        get => Writer.AsQueryable();
    }
    internal virtual DbSet<TEntity> Writer
    {
        get => _dbContext.Set<TEntity>();
    }
    /// <summary>
    /// GenericRepository constructor
    /// </summary>
    /// <param name="context">DbContext</param>
    public KirelGenericEntityFrameworkRepository(TDbContext context)
    {
        _dbContext = context;
    }
    /// <summary>
    /// Add new Entity
    /// </summary>
    /// <param name="entity">New entity for creating</param>
    /// <returns>Created Entity</returns>
    public async Task<TEntity> Insert(TEntity entity)
    {
        Writer.Add(entity);
        _dbContext.Entry(entity).State = EntityState.Added;
        await _dbContext.SaveChangesAsync();
        return entity;
    }
    /// <summary>
    /// Get Entity by ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>Entity</returns>
    public async Task<TEntity> GetById(TKey id)
    {
        var entity = await Reader.FirstOrDefaultAsync(e => e.Id.Equals(id));
        if (entity == null)
            throw new KeyNotFoundException($"Entity with specified id {id} was not found");
        return entity;
    }
    /// <summary>
    /// Delete Entity
    /// </summary>
    /// <param name="id">Entity ID</param>
    public async Task Delete(TKey id)
    {
        var entity = await GetById(id);
        if (entity == null)
            throw new KeyNotFoundException($"Entity with specified id {id} was not found");
        Writer.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
    /// <summary>
    /// Updating entity
    /// </summary>
    /// <param name="entity">New entity for updating</param>
    /// <returns>Updated entity</returns>
    public async Task<TEntity> Update(TEntity entity)
    {
        Writer.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        return entity;
    }
    /// <summary>
    /// Get entity list
    /// </summary>
    /// <param name="expression">Filter</param>
    /// <param name="orderBy">Sorting</param>
    /// <param name="includes">Includes</param>
    /// <param name="page">Current page</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of entities</returns>
    public async Task<IEnumerable<TEntity>> GetList(Expression<Func<TEntity, bool>>? expression = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, 
            IQueryable<TEntity>>? includes = null, int page = 0, int pageSize = 0)
    {
        IQueryable<TEntity> query = Reader;
        if (expression != null)
            query = query.Where(expression);
        if (includes != null)
            query = includes(query);
        
        if (orderBy != null)
            query = orderBy(query);
        else
            query = query.OrderByDescending(e => e.Created);

        if (page > 0 && pageSize > 0)
            query = query.Skip(pageSize * (page - 1)).Take(pageSize);
        
        return await query.ToListAsync();
    }
    /// <summary>
    /// Get Entity List (search in all fields)
    /// </summary>
    /// <param name="search">Search string</param>
    /// <param name="orderBy">Sorting</param>
    /// <param name="orderDirection">Sorting direction</param>
    /// <param name="page">Current page</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of entities</returns>
    public async Task<IEnumerable<TEntity>> GetList(string search = "", string orderBy = "", SortDirection orderDirection = SortDirection.Asc, int page = 0,
        int pageSize = 0)
    {
        var pageContext = page < 1 ? 0 : page;
        var pageSizeContext = pageSize < 1 ? 0 : pageSize;
        Expression<Func<TEntity, bool>>? filterExpression = null;
        Expression<Func<TEntity, object>>? orderExpression = null;

        if (!string.IsNullOrEmpty(search))
        {
            filterExpression = PredicateBuilder.PredicateSearchInAllFields<TEntity>(search);
        }

        IEnumerable<TEntity>? entities = null;
        if (!string.IsNullOrEmpty(orderBy))
        {
            orderExpression = PredicateBuilder.ToLambda<TEntity>(orderBy);
        }

        if (orderExpression != null)
        { 
            entities = orderDirection switch
            {
                SortDirection.Asc => await GetList(filterExpression, c => c.OrderBy(orderExpression),null,
                    pageContext, pageSizeContext),
                SortDirection.Desc => await GetList(filterExpression, c => c.OrderByDescending(orderExpression),null,
                    pageContext, pageSizeContext),
                _ => throw new ArgumentOutOfRangeException(nameof(orderDirection), orderDirection, null)
            };
        }
        return entities ?? await GetList(filterExpression, query => query.OrderByDescending(e => e.Created), null, page, pageSize);
    }

    /// <summary>
    /// Count Elements with specified filter
    /// </summary>
    /// <param name="expression">Filter</param>
    /// <returns>Number of entities</returns>
    public async Task<int> Count(Expression<Func<TEntity, bool>>? expression)
    {
        if (expression != null)
            return await Reader.Where(expression).CountAsync();
        return await Reader.CountAsync();
    }
    /// <summary>
    /// Count Elements with search string (search in all fields)
    /// </summary>
    /// <param name="search">Search string</param>
    /// <returns>Number of entities</returns>
    public async Task<int> Count(string search)
    {
        Expression<Func<TEntity, bool>>? filterExpression = null;
        
        if (!string.IsNullOrEmpty(search))
        {
            filterExpression = PredicateBuilder.PredicateSearchInAllFields<TEntity>(search);
        }
        return await Count(filterExpression);
    }
}