using System;
using MapOnly.Interface;

namespace MapOnly
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class MapAttribute : Attribute
    { 
        public MapAttribute()
        {
        }

        public bool Ignored { get; set; }       
    }  
}