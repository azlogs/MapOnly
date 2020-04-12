using MapOnly.Interface;
using System;

namespace MapOnly
{
    internal class MapObject<TSource, TDestination>: IMapObject<TSource, TDestination>
    {
        public MapObject()
        {
        }

        public MapObject(Guid settingId)
        {
            MappingSettingId = settingId;
        }

        public Type Source
        {
            get
            {
                return typeof(TSource);
            }
        }

        public Type Destination
        {
            get
            {
                return typeof(TDestination);
            }
        }

        public Guid MappingSettingId { get; set; }
    }
}