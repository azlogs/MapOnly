using System;
using MapOnly.Interface;

namespace MapOnly
{
    /// <summary>
    /// Attribute to mark properties that should be ignored during mapping
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class MapAttribute : Attribute
    { 
        /// <summary>
        /// Initializes a new instance of the MapAttribute class
        /// </summary>
        public MapAttribute()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the property should be ignored during mapping
        /// </summary>
        public bool Ignored { get; set; }       
    }  
}