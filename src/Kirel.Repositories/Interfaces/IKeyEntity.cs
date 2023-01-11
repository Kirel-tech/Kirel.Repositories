namespace Kirel.Repositories.Interfaces;

/// <summary>
/// A class containing a field of special entity key
/// </summary>
/// <typeparam name="TKey">Type of special entity key</typeparam>
public interface IKeyEntity<TKey> where TKey :  IComparable, IComparable<TKey>, IEquatable<TKey>
{
    /// <summary>
    /// Id of the entity
    /// </summary>
    /// <returns>Key entity</returns>
    public TKey Id { get; set; }
}