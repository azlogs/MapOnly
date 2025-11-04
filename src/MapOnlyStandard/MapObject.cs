using MapOnly.Interface;
using System;

namespace MapOnly
{
    internal sealed class MapObject<TSource, TDestination> : IMapObject<TSource, TDestination>
    {
        public MapObject()
        {
        }

        public MapObject(Guid settingId)
        {
            MappingSettingId = settingId;
        }

        public Type Source => typeof(TSource);

        public Type Destination => typeof(TDestination);

        public Guid MappingSettingId { get; set; }
    }
}