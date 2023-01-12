using System.Linq.Expressions;
using Kirel.Repositories.Sorts;

namespace Kirel.Repositories.Interfaces;

/// <summary>
/// Interface for generic repository
/// </summary>
/// <typeparam name="TKey">Type of special entity key</typeparam>
/// <typeparam name="TEntity">Type of entity</typeparam>
public interface IKirelGenericEntityFrameworkRepository<in TKey, TEntity> where TEntity : IKeyEntity<TKey> where TKey :  IComparable, IComparable<TKey>, IEquatable<TKey>
{
    /// <summary>
    /// Add new entity
    /// </summary>
    /// <param name="entity">Entity for adding</param>
    public Task<TEntity> Insert(TEntity entity);
    /// <summary>
    /// Get entity by ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>Entity</returns>
    public Task<TEntity> GetById(TKey id);
    /// <summary>
    /// Delete Entity
    /// </summary>
    /// <param name="id">Entity ID</param>
    public Task Delete(TKey id);
    /// <summary>
    /// Changes the accepted entity
    /// </summary>
    /// <param name="entity">New entity for updating</param>
    /// <returns>Updated entity</returns>
    public Task<TEntity> Update(TEntity entity);
    /// <summary>
    /// Get entity list
    /// </summary>
    /// <param name="expression">Filter</param>
    /// <param name="orderBy">Sorting</param>
    /// <param name="includes">Includes</param>
    /// <param name="page">Current page</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of entities</returns>
    public Task<IEnumerable<TEntity>> GetList(Expression<Func<TEntity, bool>>? expression = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null, int page = 0,
        int pageSize = 0);
    /// <summary>
    /// Get Entity List (search in all fields)
    /// </summary>
    /// <param name="search">Search string</param>
    /// <param name="orderBy">Sorting</param>
    /// <param name="orderDirection">Sorting direction</param>
    /// <param name="page">Current page</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of entities</returns>
    public Task<IEnumerable<TEntity>> GetList(string search = "", string orderBy = "",
        SortDirection orderDirection = SortDirection.Asc,
        int page = 0, int pageSize = 0);
    /// <summary>
    /// Count Elements with specified filter
    /// </summary>
    /// <param name="expression">Filter</param>
    /// <returns>Number of entities</returns>
    public Task<int> Count(Expression<Func<TEntity, bool>>? expression = null);
    /// <summary>
    /// Count Elements with search string (search in all fields)
    /// </summary>
    /// <param name="search">Search string</param>
    /// <returns>Number of entities</returns>
    public Task<int> Count(string search);
}