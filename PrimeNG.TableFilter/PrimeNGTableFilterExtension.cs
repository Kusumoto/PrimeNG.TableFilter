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
            }

            totalRecord = dataSet.Count();
            dataSet = dataSet.Skip(tableFilterPayload.First).Take(tableFilterPayload.Rows);

            if (string.IsNullOrEmpty(tableFilterPayload.SortField)) return dataSet;

            switch (tableFilterPayload.SortOrder)
            {
                case (int)SortingEnumeration.OrderByAsc:
                    dataSet = dataSet.OrderBy(tableFilterPayload.SortField, false);
                    break;

                case (int)SortingEnumeration.OrderByDesc:
                    dataSet = dataSet.OrderBy(tableFilterPayload.SortField, true);
                    break;

                default:
                    throw new System.ArgumentException("Sort Order is invalid");
            }
            return dataSet;
        }

    }
}
