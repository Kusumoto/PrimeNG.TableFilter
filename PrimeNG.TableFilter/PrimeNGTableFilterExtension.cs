using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PrimeNG.TableFilter.Core;
using PrimeNG.TableFilter.Models;
using PrimeNG.TableFilter.Utils;

namespace PrimeNG.TableFilter
{
    public static class PrimeNGTableFilterExtension
    {
        public static IEnumerable<T> PrimengTableFilter<T>(this IEnumerable<T> dataSet,
            TableFilterModel tableFilterPayload, out int totalRecord)
        {
            var resultSet = dataSet.AsQueryable();
            resultSet = resultSet.PrimengTableFilter(tableFilterPayload, out totalRecord);
            return resultSet.AsEnumerable();
        }

        public static IQueryable<T> PrimengTableFilter<T>(this IQueryable<T> dataSet,
            TableFilterModel tableFilterPayload, out int totalRecord)
        {
            ITableFilterManager<T> tableFilterManager = new TableFilterManager<T>(dataSet);


            if (tableFilterPayload.Filters != null && tableFilterPayload.Filters.Any())
            {
                foreach (var filterContext in tableFilterPayload.Filters)
                {
                    var filterPayload = filterContext.Value.ToString();
                    var filterToken = JToken.Parse(filterPayload);
                    switch (filterToken)
                    {
                        case JArray _:
                        {
                            var filters = filterToken.ToObject<List<TableFilterContext>>();
                            tableFilterManager.FiltersDataSet(filterContext.Key, filters);
                            break;
                        }
                        case JObject _:
                        {
                                //var filter = filterToken.ToObject<TableFilterContext>(JsonSerializer.Create( new JsonSerializerSettings() { DateParseHandling= DateParseHandling.DateTime })) ;
                                var filter = filterToken.ToObject<TableFilterContext>();
                                tableFilterManager.FilterDataSet(filterContext.Key, filter);
                            break;
                        }
                    }
                }
                tableFilterManager.ExecuteFilter();
            }

            if (!string.IsNullOrEmpty(tableFilterPayload.SortField))
            {
                tableFilterManager.SingleOrderDataSet(tableFilterPayload);
            }

            if (tableFilterPayload.MultiSortMeta != null && tableFilterPayload.MultiSortMeta.Any())
            {
                tableFilterManager.MultipleOrderDataSet(tableFilterPayload);
            }

            dataSet = tableFilterManager.GetResult();
            totalRecord = dataSet.Count();
            dataSet = dataSet.Skip(tableFilterPayload.First).Take(tableFilterPayload.Rows);
            return dataSet;
        }
    }
}