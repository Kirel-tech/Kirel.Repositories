namespace Kirel.Repositories.Core.Interfaces;

/// <summary>
/// Interface containing fields with date of creation and updating of the entity
/// </summary>
public interface ITimeTrackedEntity : ICreatedAtTrackedEntity
{
    /// <summary>
    /// Last modified DateTime UTC
    /// </summary>
    public DateTime? Updated { get; set; }
}

/// <summary>
/// Interface of creating at DateTime UTC
/// </summary>
public interface ICreatedAtTrackedEntity
{
    /// <summary>
    /// Created at DateTime UTC
    /// </summary>
    public DateTime Created { get; set; }
}