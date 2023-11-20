using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PrimeNG.TableFilter.Core;
using PrimeNG.TableFilter.Models;

namespace PrimeNG.TableFilter
{
	/// <summary>
	/// Static class contains the extension method for invoke PrimeNG load lazy filter
	/// </summary>
	public static class PrimeNGTableFilterExtension
	{
		/// <summary>
		/// Extension method for filter Iterable Object use PrimeNG load lazy filter payload
		/// </summary>
		/// <remarks>This extension method for object type IEnumerable</remarks>
		/// <typeparam name="T">Type of entity for filter</typeparam>
		/// <param name="dataSet">Data for filter</param>
		/// <param name="tableFilterPayload">PrimeNG filter payload</param>
		/// <param name="totalRecord">Total of record</param>
		/// <returns></returns>
		public static IEnumerable<T> PrimengTableFilter<T>(this IEnumerable<T> dataSet,
			TableFilterModel tableFilterPayload, out int totalRecord)
		{
			var resultSet = dataSet.AsQueryable();
			resultSet = resultSet.PrimengTableFilter(tableFilterPayload, out totalRecord);
			return resultSet.AsEnumerable();
		}

		/// <summary>
		/// Extension method for filter Iterable Object use PrimeNG load lazy filter payload
		/// </summary>
		/// <remarks>This extension method for object type IQueryable</remarks>
		/// <typeparam name="T">Type of entity for filter</typeparam>
		/// <param name="dataSet">Data for filter</param>
		/// <param name="tableFilterPayload">PrimeNG filter payload</param>
		/// <param name="totalRecord">Total of record</param>
		/// <returns></returns>
		public static IQueryable<T> PrimengTableFilter<T>(this IQueryable<T> dataSet,
			TableFilterModel tableFilterPayload, out int totalRecord)
		{
			ITableFilterManager<T> tableFilterManager = new TableFilterManager<T>(dataSet);

			ApplyFilters(tableFilterPayload, tableFilterManager);

			return FilterDataSet(tableFilterPayload, tableFilterManager, out totalRecord);
		}

		public static IQueryable<T> PrimengTableFilter<T>(this IQueryable<T> dataSet,
			TableFilterModel tableFilterPayload, string[] globalFilters, out int totalRecord)
		{
			ITableFilterManager<T> tableFilterManager = new TableFilterManager<T>(dataSet);

			ApplyGlobalFilter(tableFilterPayload, globalFilters, tableFilterManager);
			ApplyFilters(tableFilterPayload, tableFilterManager);

			return FilterDataSet(tableFilterPayload, tableFilterManager, out totalRecord);
		}

		private static void ApplyFilters<T>(TableFilterModel tableFilterPayload, ITableFilterManager<T> tableFilterManager)
		{
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
		}

		private static void ApplyGlobalFilter<T>(TableFilterModel tableFilterPayload, string[] props, ITableFilterManager<T> tableFilterManager)
		{
			if (string.IsNullOrEmpty(tableFilterPayload.GlobalFilter))
				return;
			foreach (var prop in props)
			{
				var filter = new TableFilterContext
				{
					MatchMode = "contains",
					Operator = "or",
					Value = tableFilterPayload.GlobalFilter
				};
				tableFilterManager.FilterDataSet(prop, filter, OperatorEnumeration.Or);
			}

			if (tableFilterPayload.Filters == null || !tableFilterPayload.Filters.Any())
			{
				tableFilterManager.ExecuteFilter();
			}
		}

		private static IQueryable<T> FilterDataSet<T>(TableFilterModel tableFilterPayload, ITableFilterManager<T> tableFilterManager, out int totalRecord)
		{
			var dataSet = tableFilterManager.GetResult();
			totalRecord = dataSet.Count();
			dataSet = dataSet.Skip(tableFilterPayload.First).Take(tableFilterPayload.Rows);
			return dataSet;
		}
	}
}