using MapOnly.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MapOnly
{
    /// <summary>
    /// Map object extension methods for mapping between objects
    /// </summary>
    public static class MapExtension
    {
        private static readonly Dictionary<Guid, MapSetting> MapSettings = new Dictionary<Guid, MapSetting>();
        private const string SourceDestinationNotNull = "Source and Destination cannot be null.";
        private const string PropertyNotFound = "Source doesn't have property \"{0}\".";

        // simple map
        #region Simple map
        /// <summary>
        /// Mapping 2 objects (from "source" to "destination") that have the same properties
        /// </summary>
        /// <typeparam name="TSource">Class type of source object</typeparam>
        /// <typeparam name="TDestination">Class type of destination object</typeparam>
        /// <param name="source">Source Object</param>
        /// <param name="destination">Destination Object</param>
        /// <returns>Destination object</returns>
        private static TDestination SimpleMap<TSource, TDestination>(this TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            if (source == null || destination == null)
            {
                throw new ArgumentNullException(nameof(source), SourceDestinationNotNull);
            }

            // get all properties
            var sourceProperties = source.GetType().GetProperties();
            var destinationProperties = destination.GetType().GetProperties();

            // do mapping
            foreach (var property in destinationProperties)
            {
                var attributes = property.GetCustomAttributes();
                bool isIgnored = false;

                // get attribute 
                foreach (object attribute in attributes)
                {
                    if (attribute is MapAttribute mapAttribute && mapAttribute.Ignored)
                    {
                        isIgnored = true;
                        break;
                    }
                }

                if (isIgnored) continue;

                var sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == property.Name);

                if (sourceProperty == null)
                {
                    throw new ArgumentException(string.Format(PropertyNotFound, property.Name));
                }

                object? value = sourceProperty.GetValue(source, null);
                property.SetValue(destination, value);
            }

            return destination;
        }
        #endregion

        #region Map

        /// <summary>
        /// Mapping object "source" to "destination", based on the mapping setting
        /// </summary>
        /// <typeparam name="TSource">Class type of source object</typeparam>
        /// <typeparam name="TDestination">Class type of destination object</typeparam>
        /// <param name="source">Source Object</param>
        /// <param name="destination">Destination Object</param>
        /// <returns>Destination object</returns>
        public static TDestination Map<TSource, TDestination>(this TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), SourceDestinationNotNull);
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination), SourceDestinationNotNull);
            }

            // check exist setting 
            var mapSetting = GetMapSetting<TSource, TDestination>();
            bool hasSetting = mapSetting.Key != Guid.Empty;

            // get all properties
            var sourceProperties = source.GetType().GetProperties();
            var destinationProperties = destination.GetType().GetProperties();

            // do mapping
            foreach (var property in destinationProperties)
            {
                PropertyInfo? sourceProperty = null;
                bool isIgnored = false;

                // Check if destination property has ignore attribute
                var destinationAttributes = property.GetCustomAttributes();
                foreach (object attribute in destinationAttributes)
                {
                    if (attribute is MapAttribute mapAttribute && mapAttribute.Ignored)
                    {
                        isIgnored = true;
                        break;
                    }
                }

                if (isIgnored) continue;

                if (hasSetting)
                {
                    // check in ignore list
                    if (mapSetting.Value.IgnoreProperties.Any(x => x == property.Name))
                    {
                        continue;
                    }

                    // check in map list
                    var p = mapSetting.Value.MapProperties.FirstOrDefault(x => x.ToProperty == property.Name);
                    if (p != null)
                    {
                        if (p.MapType == MapType.MapValue)
                        {
                            property.SetValue(destination, p.Value);
                            continue;
                        }
                        
                        if (!string.IsNullOrEmpty(p.FromProperty))
                        {
                            sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == p.FromProperty);
                        }
                    }
                }

                if (sourceProperty == null)
                {
                    sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == property.Name);
                }

                if (sourceProperty == null)
                {
                    // Skip properties that don't exist in source
                    continue;
                }

                // Check if source property has ignore attribute
                var sourceAttributes = sourceProperty.GetCustomAttributes();
                foreach (object attribute in sourceAttributes)
                {
                    if (attribute is MapAttribute mapAttribute && mapAttribute.Ignored)
                    {
                        isIgnored = true;
                        break;
                    }
                }

                if (isIgnored) continue;

                object? value = sourceProperty.GetValue(source, null);
                property.SetValue(destination, value);
            }

            return destination;
        }

        /// <summary>
        /// Maps a source object to a new destination object instance
        /// </summary>
        /// <typeparam name="TSource">Class type of source object</typeparam>
        /// <typeparam name="TDestination">Class type of destination object</typeparam>
        /// <param name="source">Source Object</param>
        /// <returns>New destination object with mapped values</returns>
        public static TDestination Map<TSource, TDestination>(this TSource source)
            where TSource : class
            where TDestination : class, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), SourceDestinationNotNull);
            }

            var destination = new TDestination();
            return Map(source, destination);
        }

        #endregion

        #region Setting

        /// <summary>
        /// Create a mapping configuration between Source and Destination types
        /// </summary>
        /// <typeparam name="TSource">Source Type</typeparam>
        /// <typeparam name="TDestination">Destination Type</typeparam>
        /// <returns>IMapObject for fluent configuration</returns>
        public static IMapObject<TSource, TDestination> Create<TSource, TDestination>()
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);
            var map = MapSettings.FirstOrDefault(x => x.Value.Source == sourceType && x.Value.Destination == destinationType);

            // check exist.
            if (map.Key != Guid.Empty)
            {
                return new MapObject<TSource, TDestination>
                {
                    MappingSettingId = map.Key
                };
            }

            Guid settingId = Guid.NewGuid();

            // add to setting
            MapSettings[settingId] = new MapSetting
            {
                Destination = destinationType,
                Source = sourceType
            };

            return new MapObject<TSource, TDestination>
            {
                MappingSettingId = settingId
            };
        }

        /// <summary>
        /// Remove a mapping configuration
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="mapObject">Map object to remove</param>
        /// <returns>IMapObject</returns>
        public static IMapObject<TSource, TDestination> Remove<TSource, TDestination>(this IMapObject<TSource, TDestination> mapObject)
        {
            if (mapObject == null)
            {
                throw new ArgumentNullException(nameof(mapObject));
            }

            var map = mapObject.GetMapSettingByMapObject();
            MapSettings.Remove(map.Key);

            return mapObject;
        }

        /// <summary>
        /// Add a property to the ignore list
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="mapObject">Map object</param>
        /// <param name="expression">Expression to select the property to ignore</param>
        /// <returns>IMapObject for fluent configuration</returns>
        public static IMapObject<TSource, TDestination> Ignore<TSource, TDestination>(
            this IMapObject<TSource, TDestination> mapObject, 
            Expression<Func<TDestination, object>> expression)
        {
            if (mapObject == null)
            {
                throw new ArgumentNullException(nameof(mapObject));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var map = mapObject.GetMapSettingByMapObject();
            string propertyName = GetPropertyName(expression);

            // remove from map setting
            var mapProperty = map.Value.MapProperties.FirstOrDefault(x => x.ToProperty == propertyName);
            if (mapProperty != null)
            {
                map.Value.MapProperties.Remove(mapProperty);
            }

            // add into ignore listing if not exist.
            if (!map.Value.IgnoreProperties.Any(x => x == propertyName))
            {
                map.Value.IgnoreProperties.Add(propertyName);
            }

            return mapObject;
        }

        /// <summary>
        /// Add a property mapping between source and destination
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="mapObject">Map object</param>
        /// <param name="sourceExpression">Expression to select the source property</param>
        /// <param name="destinationExpression">Expression to select the destination property</param>
        /// <returns>IMapObject for fluent configuration</returns>
        public static IMapObject<TSource, TDestination> Add<TSource, TDestination>(
            this IMapObject<TSource, TDestination> mapObject,
            Expression<Func<TSource, object>> sourceExpression, 
            Expression<Func<TDestination, object>> destinationExpression)
        {
            if (mapObject == null)
            {
                throw new ArgumentNullException(nameof(mapObject));
            }

            if (sourceExpression == null)
            {
                throw new ArgumentNullException(nameof(sourceExpression));
            }

            if (destinationExpression == null)
            {
                throw new ArgumentNullException(nameof(destinationExpression));
            }

            var map = GetMapSetting<TSource, TDestination>();
            if (map.Key == Guid.Empty) 
            {
                throw new InvalidOperationException($"Mapping not found. Source: \"{typeof(TSource).Name}\", Destination: \"{typeof(TDestination).Name}\". Please call Create<TSource, TDestination>() first.");
            }

            string toProperty = GetPropertyName(destinationExpression);

            // remove if exist in ignore listing
            if (map.Value.IgnoreProperties.Contains(toProperty))
            {
                map.Value.IgnoreProperties.Remove(toProperty);
            }

            var mapProperty = map.Value.MapProperties.FirstOrDefault(x => x.ToProperty == toProperty);
            if (mapProperty != null)
            {
                map.Value.MapProperties.Remove(mapProperty);
            }

            string fromProperty = GetPropertyName(sourceExpression);
            map.Value.MapProperties.Add(new MapProperty
            {
                ToProperty = toProperty,
                MapType = MapType.MapProperty,
                FromProperty = fromProperty
            });

            return mapObject;
        }

        /// <summary>
        /// Map a destination property to a constant value
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="mapObject">Map object</param>
        /// <param name="destinationExpression">Expression to select the destination property</param>
        /// <param name="value">Value to set</param>
        /// <returns>IMapObject for fluent configuration</returns>
        public static IMapObject<TSource, TDestination> Add<TSource, TDestination>(
            this IMapObject<TSource, TDestination> mapObject,
            Expression<Func<TDestination, object>> destinationExpression, 
            object? value)
        {
            if (mapObject == null)
            {
                throw new ArgumentNullException(nameof(mapObject));
            }

            if (destinationExpression == null)
            {
                throw new ArgumentNullException(nameof(destinationExpression));
            }

            var map = GetMapSetting<TSource, TDestination>();
            if (map.Key == Guid.Empty) 
            {
                throw new InvalidOperationException($"Mapping not found. Source: \"{typeof(TSource).Name}\", Destination: \"{typeof(TDestination).Name}\". Please call Create<TSource, TDestination>() first.");
            }

            string toProperty = GetPropertyName(destinationExpression);

            // remove added before
            var mapProperty = map.Value.MapProperties.FirstOrDefault(x => x.ToProperty == toProperty);
            if (mapProperty != null)
            {
                map.Value.MapProperties.Remove(mapProperty);
            }

            map.Value.MapProperties.Add(new MapProperty
            {
                MapType = MapType.MapValue,
                Value = value,
                ToProperty = toProperty
            });

            return mapObject;
        }

        /// <summary>
        /// Clear all mapping settings and enable automatic mapping for all properties
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="mapObject">Map object</param>
        /// <returns>IMapObject for fluent configuration</returns>
        private static IMapObject<TSource, TDestination> MapAll<TSource, TDestination>(this IMapObject<TSource, TDestination> mapObject)
        {
            if (mapObject == null)
            {
                throw new ArgumentNullException(nameof(mapObject));
            }

            var map = GetMapSetting<TSource, TDestination>();
            if (map.Key == Guid.Empty) return mapObject;
            map.Value.Clear();
            map.Value.IsMapAll = true;

            return mapObject;
        }
        #endregion

        #region Private methods

        private static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var body = expression.Body as MemberExpression;
            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            if (body == null)
            {
                throw new ArgumentException("Expression must be a member access expression", nameof(expression));
            }

            return body.Member.Name;
        }

        private static KeyValuePair<Guid, MapSetting> GetMapSetting<TSource, TDestination>()
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);
            var map = MapSettings.FirstOrDefault(x => x.Value.Source == sourceType && x.Value.Destination == destinationType);

            return map;
        }

        private static KeyValuePair<Guid, MapSetting> GetMapSettingByMapObject<TSource, TDestination>(this IMapObject<TSource, TDestination> mapObject)
        {
            if (mapObject == null)
            {
                throw new ArgumentNullException(nameof(mapObject));
            }

            if (!MapSettings.ContainsKey(mapObject.MappingSettingId))
            {
                throw new InvalidOperationException($"Mapping setting not found. Source: \"{typeof(TSource).Name}\", Destination: \"{typeof(TDestination).Name}\".");
            }

            return MapSettings.Single(x => x.Key == mapObject.MappingSettingId);
        }

        #endregion 
    }
}