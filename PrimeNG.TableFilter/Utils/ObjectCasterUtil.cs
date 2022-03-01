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
            if (property?.PropertyType == typeof(DateTime))
                return arrayCast.ToObject<List<DateTime>>();
            if (property?.PropertyType == typeof(DateTime?))
                return arrayCast.ToObject<List<DateTime?>>();
            if (property?.PropertyType == typeof(bool?))
                return arrayCast.ToObject<List<bool?>>();
            if (property?.PropertyType == typeof(short?))
                return arrayCast.ToObject<List<short?>>();
            if (property?.PropertyType == typeof(int?))
                return arrayCast.ToObject<List<int?>>();
            if (property?.PropertyType == typeof(long?))
                return arrayCast.ToObject<List<long?>>();
            if (property?.PropertyType == typeof(float?))
                return arrayCast.ToObject<List<float?>>();
            if (property?.PropertyType == typeof(double?))
                return arrayCast.ToObject<List<double?>>();
            if (property?.PropertyType == typeof(decimal?))
                return arrayCast.ToObject<List<decimal?>>();

            return arrayCast.ToObject<List<string>>();
        }

        public static object CastPropertiesType(PropertyInfo property, object value)
        {
                
            if (property?.PropertyType == typeof(int))
                return Convert.ToInt32(value);
            if (property?.PropertyType == typeof(double))
                return Convert.ToDouble(value);
            if (property?.PropertyType == typeof(DateTime))
                return Convert.ToDateTime(value);
            if (property?.PropertyType == typeof(DateTime?))
                return Convert.ToDateTime(value);
            if (property?.PropertyType == typeof(bool?))
                return Convert.ToBoolean(value);
            if (property?.PropertyType == typeof(short?))
                return Convert.ToInt16(value);
            if (property?.PropertyType == typeof(int?))
                return Convert.ToInt32(value);
            if (property?.PropertyType == typeof(long?))
                return Convert.ToInt64(value);
            if (property?.PropertyType == typeof(float?))
                return Convert.ToSingle(value);
            if (property?.PropertyType == typeof(double?))
                return Convert.ToDouble(value);
            if (property?.PropertyType == typeof(decimal?))
                return Convert.ToDecimal(value);
            

            return value.ToString();
        }

        public static TableFilterContext CastJObjectToTableFilterContext(JObject obj)
            => obj.ToObject<TableFilterContext>();
    }
}