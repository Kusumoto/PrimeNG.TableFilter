using System.Collections.Generic;
using System.Linq;
using PrimeNG.TableFilter.Models;
using PrimeNG.TableFilter.Utils;

namespace PrimeNG.TableFilter
{
    public static class PrimeNGTableFilterExtension
    {
        public const string FilterTypeMatchModeStartsWith = "startsWith";
        public const string FilterTypeMatchModeContains = "contains";
        public const string FilterTypeMatchModeIn = "in";
        public const string FilterTypeMatchModeEndsWith = "endsWith";
        public const string FilterTypeMatchModeEquals = "equals";

        public static IQueryable<T> PrimengTableFilter<T>(this IQueryable<T> dataSet, TableFilterModel tableFilterPayload, ref int totalRecord)
        {
            foreach (var filter in tableFilterPayload.Filters)
            {
                dataSet = FilterDataSet(dataSet, filter);
            }
            totalRecord = dataSet.Count();
            if (!string.IsNullOrEmpty(tableFilterPayload.SortField))
            {
                dataSet = SingleOrderDataSet(dataSet, tableFilterPayload);
            }
            if (tableFilterPayload.MultiSortMeta != null && tableFilterPayload.MultiSortMeta.Any())
            {
                dataSet = MultipleOrderDataSet(dataSet, tableFilterPayload);
            }
            dataSet = dataSet.Skip(tableFilterPayload.First).Take(tableFilterPayload.Rows);
            return dataSet;
        }

        private static IQueryable<T> MultipleOrderDataSet<T>(IQueryable<T> dataSet, TableFilterModel tableFilterPayload)
        {
            tableFilterPayload.MultiSortMeta.Select((value, i) => new { i, value }).ToList().ForEach(o =>
            {
                switch (o.value.Order)
                {
                    case (int)SortingEnumeration.OrderByAsc:
                        if (o.i == 0)
                            dataSet = dataSet.OrderBy(o.value.Field.FirstCharToUpper(), false);
                        else
                            dataSet = dataSet.ThenOrderBy(o.value.Field.FirstCharToUpper(), false);
                        break;

                    case (int)SortingEnumeration.OrderByDesc:
                        if (o.i == 0)
                            dataSet = dataSet.OrderBy(o.value.Field.FirstCharToUpper(), true);
                        else
                            dataSet = dataSet.ThenOrderBy(o.value.Field.FirstCharToUpper(), true);
                        break;

                    default:
                        throw new System.ArgumentException("Sort Order is invalid");
                }
            });
            return dataSet;
        }

        private static IQueryable<T> SingleOrderDataSet<T>(IQueryable<T> dataSet, TableFilterModel tableFilterPayload)
        {
            switch (tableFilterPayload.SortOrder)
            {
                case (int)SortingEnumeration.OrderByAsc:
                    dataSet = dataSet.OrderBy(tableFilterPayload.SortField.FirstCharToUpper(), false);
                    break;

                case (int)SortingEnumeration.OrderByDesc:
                    dataSet = dataSet.OrderBy(tableFilterPayload.SortField.FirstCharToUpper(), true);
                    break;

                default:
                    throw new System.ArgumentException("Sort Order is invalid");
            }

            return dataSet;
        }

        private static IQueryable<T> FilterDataSet<T>(IQueryable<T> dataSet, KeyValuePair<string, TableFilterContext> filter)
        {
            switch (filter.Value.MatchMode)
            {
                case FilterTypeMatchModeStartsWith:
                    dataSet = dataSet.WhereWithExtensions("StartsWith", filter.Key.FirstCharToUpper(), filter.Value.Value);
                    break;

                case FilterTypeMatchModeContains:
                    dataSet = dataSet.WhereWithExtensions("Contains", filter.Key.FirstCharToUpper(), filter.Value.Value);
                    break;

                case FilterTypeMatchModeIn:
                    dataSet = dataSet.WhereWithList(filter.Key.FirstCharToUpper(), filter.Value.Value);
                    break;

                case FilterTypeMatchModeEndsWith:
                    dataSet = dataSet.WhereWithExtensions("EndsWith", filter.Key.FirstCharToUpper(), filter.Value.Value);
                    break;

                case FilterTypeMatchModeEquals:
                    dataSet = dataSet.WhereWithExtensions("Equals", filter.Key.FirstCharToUpper(), filter.Value.Value);
                    break;

                default:
                    throw new System.ArgumentException("Match mode is invalid");
            }

            return dataSet;
        }
    }
}
