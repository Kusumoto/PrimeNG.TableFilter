using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace PrimeNG.TableFilter.Utils
{
    public static class DynamicWhereUtil
    {
        public static IQueryable<TEntity> WhereWithExtensions<TEntity>(this IQueryable<TEntity> source,
            string extensionMethod, string propertyName, object propertyValue, bool isNegation = false)
        {
            var type = typeof(TEntity);
            var property = type.GetProperty(propertyName);
            var propertyType = property?.PropertyType;

            if (propertyType != typeof(string) && extensionMethod != "Equals")
            {
                Console.WriteLine($"Property ${propertyName} not support method ${extensionMethod}");
                return source;
            }

            if (propertyType == null)
                return source;

            var castValue = CastPropertiesType(property, propertyValue);
            var parameter = Expression.Parameter(type, "e");
            var propertyAccess =
                Expression.MakeMemberAccess(parameter, property ?? throw new InvalidOperationException());
            var propertyConstant = Expression.Constant(castValue, propertyType);
            var methodInfo = propertyType.GetMethod(extensionMethod, new[] {propertyType});
            if (propertyType.IsGenericType
                && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                && propertyType == typeof(DateTime?))
            {
                var converted = Expression.Convert(propertyConstant, typeof(object));
                if (isNegation)
                {
                    var callMethod = Expression.Not(Expression.Call(propertyAccess,
                        methodInfo ?? throw new InvalidOperationException(), converted));
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(callMethod, parameter);
                    return source.Where(lambda);
                }
                else
                {
                    var callMethod = Expression.Call(propertyAccess,
                        methodInfo ?? throw new InvalidOperationException(), converted);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(callMethod, parameter);
                    return source.Where(lambda);
                }
            }
            if (isNegation)
            {
                var callMethod = Expression.Not(Expression.Call(propertyAccess,
                    methodInfo ?? throw new InvalidOperationException(), propertyConstant));
                var lambda = Expression.Lambda<Func<TEntity, bool>>(callMethod, parameter);
                return source.Where(lambda);
            }
            else
            {
                var callMethod = Expression.Call(propertyAccess,
                    methodInfo ?? throw new InvalidOperationException(), propertyConstant);
                var lambda = Expression.Lambda<Func<TEntity, bool>>(callMethod, parameter);
                return source.Where(lambda);
            }
        }

        public static IQueryable<TEntity> WhereWithList<TEntity>(this IQueryable<TEntity> source,
            string propertyName, object propertyValue)
        {
            var type = typeof(TEntity);
            var property = type.GetProperty(propertyName);
            var propertyType = property?.PropertyType;
            
            if (propertyType == null)
                return source;
            
            var castValue = CastPropertiesTypeList(property, propertyValue);
            var methodInfo = castValue.GetType().GetMethod("Contains", new[] { propertyType });

            var list = Expression.Constant(castValue);
            var param = Expression.Parameter(type, "e");
            var value = Expression.Property(param, propertyName);
            var body = Expression.Call(list, methodInfo ?? throw new InvalidOperationException(), value);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, param);
            return source.Where(lambda);
        }

        private static object CastPropertiesTypeList(PropertyInfo property, object value)
        {
            var arrayCast = (JArray)value;
            if (property?.PropertyType == typeof(int))
                return arrayCast.ToObject<List<int>>();
            if (property?.PropertyType == typeof(double))
                return arrayCast.ToObject<List<double>>();
            if (property?.PropertyType == typeof(DateTime))
                return arrayCast.ToObject<List<DateTime>>();
            if (property?.PropertyType == typeof(DateTime?))
                return arrayCast.ToObject<List<DateTime?>>();
            return arrayCast.ToObject<List<string>>();
        }

        private static object CastPropertiesType(PropertyInfo property, object value)
        {
            if (property?.PropertyType == typeof(int))
                return Convert.ToInt32(value);
            if (property?.PropertyType == typeof(double))
                return Convert.ToDouble(value);
            if (property?.PropertyType == typeof(DateTime))
                return Convert.ToDateTime(value);
            if (property?.PropertyType == typeof(DateTime?))
                return Convert.ToDateTime(value);
            return value.ToString();
        }

    }
}
