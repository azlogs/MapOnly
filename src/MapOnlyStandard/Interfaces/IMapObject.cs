using System;

namespace MapOnly.Interface
{
    /// <summary>
    /// Represents a mapping configuration between a source and destination type
    /// </summary>
    /// <typeparam name="TSource">The source type</typeparam>
    /// <typeparam name="TDestination">The destination type</typeparam>
    public interface IMapObject<TSource, TDestination>
    {
        /// <summary>
        /// Gets the source type
        /// </summary>
        Type Source { get; }

        /// <summary>
        /// Gets the destination type
        /// </summary>
        Type Destination { get; }

        /// <summary>
        /// Gets or sets the unique identifier for this mapping configuration
        /// </summary>
        Guid MappingSettingId { get; set; }
    }
}