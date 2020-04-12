using MapOnly.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MapOnly
{
    /// <summary>
    /// Map object extension
    /// </summary>
    public static class MapExtension
    {
        private static Dictionary<Guid, MapSetting> MapSetting = new Dictionary<Guid, MapSetting>();
        private const string SourceDestinationNOTNULL = "Source or Destination can not be null.";

        // simple map
        #region Simple map
        /// <summary>
        /// Mapping 2 objects ( from "source" to "destination") has the same properties
        /// </summary>
        /// <typeparam name="TSource">Class type of source object</typeparam>
        /// <typeparam name="TDestination">Class type of destination object</typeparam>
        /// <param name="source">Source Object</param>
        /// <param name="destination">Destination Object</param>
        /// <returns>Destionation object</returns>
        private static TDestination SimpleMap<TSource, TDestination>(this TSource source, TDestination destination)
        {
            if (source == null || destination == null)
            {
                throw new ArgumentException(SourceDestinationNOTNULL);
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
                    var mapAttribute = attribute as MapAttribute;
                    if (mapAttribute == null) continue;

                    if (mapAttribute.Ignored)
                    {
                        isIgnored = true;
                        break;
                    }
                }

                if (isIgnored) continue;

                var sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == property.Name);

                if (sourceProperty == null || property.Equals(sourceProperties))
                {
                    throw new ArgumentException($"Source doesn't have property \"{property.Name}\"");
                }

                object value = sourceProperty.GetValue(source, null);
                property.SetValue(destination, value);
            }

            return destination;
        }
        #endregion

        #region Map

        /// <summary>
        /// Mapping object "source" to "destination", base on the mapping setting
        /// </summary>
        /// <typeparam name="TSource">Class type of source object</typeparam>
        /// <typeparam name="TDestination">Class type of destination object</typeparam>
        /// <param name="source">Source Object</param>
        /// <param name="destination">Destination Object</param>
        /// <returns>Destionation object</returns>
        public static TDestination Map<TSource, TDestination>(this TSource source, TDestination destination)
        {
            if (source == null || destination == null)
            {
                throw new ArgumentException(SourceDestinationNOTNULL);
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
                PropertyInfo sourceProperty = null;
                var attributes = property.GetCustomAttributes();
                bool isIgnored = false;

                // get attribute 
                foreach (object attribute in attributes)
                {
                    MapAttribute mapAttribute = attribute as MapAttribute;
                    if (mapAttribute != null)
                    {
                        if (mapAttribute.Ignored)
                        {
                            isIgnored = true;
                            break;
                        }
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
                        //else if..
                        sourceProperty = sourceProperties.Single(x => x.Name == p.FromProperty);
                    }
                }

                if (sourceProperty == null)
                {
                    sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == property.Name);
                }

                if (sourceProperty == null)
                {
                    throw new ArgumentException($"Source doesn't have property \"{property.Name}\"");
                }

                object value = sourceProperty.GetValue(source, null);
                property.SetValue(destination, value);
            }

            return destination;
        }

        #endregion

        #region Setting

        /// <summary>
        /// Create a mapping between Source and destionation
        /// </summary>
        /// <typeparam name="TSource">Source Type</typeparam>
        /// <typeparam name="TDestination">Destination Type</typeparam>
        /// <returns>IMapObject</returns>
        public static IMapObject<TSource, TDestination> Create<TSource, TDestination>()
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);
            var map = MapSetting.FirstOrDefault(x => x.Value.Source == sourceType && x.Value.Destination == destinationType);

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
            MapSetting[settingId] = new MapSetting
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
        /// Remove a mapping
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="mapObject"></param>
        /// <returns>IMapObject</returns>
        public static IMapObject<TSource, TDestination> Remove<TSource, TDestination>(this IMapObject<TSource, TDestination> mapObject)
        {
            var map = mapObject.GetMapSettingByMapObject();
            MapSetting.Remove(map.Key);

            return mapObject;
        }

        /// <summary>
        /// Add ignored property
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="mapObject"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IMapObject<TSource, TDestination> Ignore<TSource, TDestination>(this IMapObject<TSource, TDestination> mapObject, Expression<Func<TDestination, object>> expression)
        {
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
        /// Add property into mapping setting 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="mapObject"></param>
        /// <param name="sourceExpression"></param>
        /// <param name="destinationExpression"></param>
        /// <returns></returns>
        public static IMapObject<TSource, TDestination> Add<TSource, TDestination>(this IMapObject<TSource, TDestination> mapObject,
            Expression<Func<TSource, object>> sourceExpression, Expression<Func<TDestination, object>> destinationExpression)
        {
            var map = GetMapSetting<TSource, TDestination>();
            if (map.Key == Guid.Empty) throw new ArgumentException($"Setting not found (Source: \"{ typeof(TSource).Name}\", Destination: \"{typeof(TDestination).Name}\").");

            string toProperty = GetPropertyName(destinationExpression);

            // remove if exist in ignore listing
            if (map.Value.IgnoreProperties.Any(x => x == toProperty))
            {
                map.Value.IgnoreProperties.Remove(toProperty);
            }

            var mapProperty = map.Value.MapProperties.FirstOrDefault(x => x.ToProperty == toProperty);
            if (mapProperty != null)
            {
                map.Value.MapProperties.Remove(mapProperty);
            }

            // TODO: need to check the same type
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
        /// Mapping a property to a value
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="mapObject"></param>
        /// <param name="destinationExpression"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IMapObject<TSource, TDestination> Add<TSource, TDestination>(this IMapObject<TSource, TDestination> mapObject,
            Expression<Func<TDestination, object>> destinationExpression, object value)
        {
            var map = GetMapSetting<TSource, TDestination>();
            if (map.Key == Guid.Empty) throw new ArgumentException($"Setting not found (Source: \"{ typeof(TSource).Name}\", Destination: \"{typeof(TDestination).Name}\").");

            string toProperty = GetPropertyName(destinationExpression);

            // remove added before
            var mapProperty = map.Value.MapProperties.FirstOrDefault(x => x.FromProperty == toProperty);
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
        /// Ignored all the seting.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="mapObject"></param>
        /// <returns></returns>
        private static IMapObject<TSource, TDestination> MappAll<TSource, TDestination>(this IMapObject<TSource, TDestination> mapObject)
        {
            var map = GetMapSetting<TSource, TDestination>();
            if (map.Key == Guid.Empty) return mapObject;
            map.Value.Clear();
            map.Value.IsMapAll = true;

            return mapObject;
        }
        #endregion

        #region Private method

        private static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            var body = expression.Body as MemberExpression;
            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            return body.Member.Name;
        }

        private static KeyValuePair<Guid, MapSetting> GetMapSetting<TSource, TDestination>()
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);
            var map = MapSetting.FirstOrDefault(x => x.Value.Source == sourceType && x.Value.Destination == destinationType);

            //if (map.Key == Guid.Empty)
            //{
            //    throw new ArgumentException($"Setting not found (Source: \"{sourceType.Name}\", Destination: \"{destinationType.Name}\").");
            //}

            return map;
        }

        private static KeyValuePair<Guid, MapSetting> GetMapSettingByMapObject<TSource, TDestination>(this IMapObject<TSource, TDestination> mapObject)
        {
            if (!MapSetting.ContainsKey(mapObject.MappingSettingId))
            {
                throw new ArgumentException($"Setting not found (Source: \"{ typeof(TSource).Name}\", Destination: \"{typeof(TDestination).Name}\").");
            }

            return MapSetting.Single(x => x.Key == mapObject.MappingSettingId);
        }

        #endregion 
    }
}