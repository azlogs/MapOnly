using System;
using System.Collections.Generic;

namespace MapOnly
{
    internal class MapSetting
    { 
        public MapSetting()
        {
            IgnoreProperties = new List<string>();
            MapProperties = new List<MapProperty>();
            IsMapAll = true;
        }

        public string PropertyName { get; set; }

        public Type Source { get; set; }

        public Type Destination { get; set; }

        public List<string> IgnoreProperties { get; private set; }

        public List<MapProperty> MapProperties { get; private set; }

        public bool IsMapAll { get; set; }

        public void Clear()
        {
            IgnoreProperties = new List<string>();
            MapProperties = new List<MapProperty>();
            IsMapAll = true;
        }
    }

    internal class MapProperty
    {
        public string FromProperty { get; set; }

        public string ToProperty { get; set; }

        public MapType MapType { get; set; }

        public  object Value { get; set; }
    }

    enum MapType
    {
        MapProperty = 0,
        MapValue = 1
            
    }
}