using System;

namespace MapOnly.Interface
{
    public interface IMapObject<TSource, TDestination>
    {
        Type Source { get; }

        Type Destination { get; }

        Guid MappingSettingId { get; set; }
    }
}