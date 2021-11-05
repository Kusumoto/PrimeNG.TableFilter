using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using PrimeNG.TableFilter.Models;

namespace PrimeNG.TableFilter.Utils
{
    public static class ObjectCasterUtil
    {
        public static object CastPropertiesTypeList(PropertyInfo property, object value)
        {
            var arrayCast = (JArray) value;
            if (property?.PropertyType == typeof(int))
                return arrayCast.ToObject<List<int>>();
            if (property?.PropertyType == typeof(double))
                return arrayCast.ToObject<List<double>>();
            if (property?.PropertyType == typeof(int?))
                return arrayCast.ToObject<List<int?>>();
            if (property?.PropertyType == typeof(double?))
                return arrayCast.ToObject<List<double?>>();
            if (property?.PropertyType == typeof(DateTime))
                return arrayCast.ToObject<List<DateTime>>();
            if (property?.PropertyType == typeof(DateTime?))
                return arrayCast.ToObject<List<DateTime?>>();
            return arrayCast.ToObject<List<string>>();
        }

        public static object CastPropertiesType(PropertyInfo property, object value)
        {
            if (property?.PropertyType == typeof(int))
                return Convert.ToInt32(value);
            if (property?.PropertyType == typeof(double))
                return Convert.ToDouble(value);
            if (property?.PropertyType == typeof(int?))
                return Convert.ToInt32(value);
            if (property?.PropertyType == typeof(double?))
                return Convert.ToDouble(value);
            if (property?.PropertyType == typeof(DateTime))
                return Convert.ToDateTime(value);
            if (property?.PropertyType == typeof(DateTime?))
                return Convert.ToDateTime(value);
            return value.ToString();
        }

        public static TableFilterContext CastJObjectToTableFilterContext(JObject obj)
            => obj.ToObject<TableFilterContext>();
    }
}