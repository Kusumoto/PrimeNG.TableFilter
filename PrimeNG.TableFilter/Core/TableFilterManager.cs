using System.Collections.Generic;
using System.Linq;
using PrimeNG.TableFilter.Models;
using PrimeNG.TableFilter.Utils;

namespace PrimeNG.TableFilter.Core
{
    public class TableFilterManager<TEntity> : ITableFilterManager<TEntity>
    {
        private const string FilterTypeMatchModeStartsWith = "startsWith";
        private const string FilterTypeMatchModeContains = "contains";
        private const string FilterTypeMatchModeIn = "in";
        private const string FilterTypeMatchModeEndsWith = "endsWith";
        private const string FilterTypeMatchModeEquals = "equals";
        private const string FilterTypeMatchModeNotContains = "notContains";
        private const string FilterTypeMatchModeNotEquals = "notEquals";

        private readonly ILinqOperator<TEntity> _linqOperator;

        public TableFilterManager(IQueryable<TEntity> dataSet)
        {
            _linqOperator = new LinqOperator<TEntity>(dataSet);
        }

        public void MultipleOrderDataSet(TableFilterModel tableFilterPayload)
        {
            tableFilterPayload.MultiSortMeta.Select((value, i) => new {i, value}).ToList().ForEach(o =>
            {
                switch (o.value.Order)
                {
                    case (int) SortingEnumeration.OrderByAsc:
                        if (o.i == 0)
                            _linqOperator.OrderBy(o.value.Field.FirstCharToUpper());
                        else
                            _linqOperator.ThenBy(o.value.Field.FirstCharToUpper());
                        break;

                    case (int) SortingEnumeration.OrderByDesc:
                        if (o.i == 0)
                            _linqOperator.OrderByDescending(o.value.Field.FirstCharToUpper());
                        else
                            _linqOperator.ThenByDescending(o.value.Field.FirstCharToUpper());
                        break;

                    default:
                        throw new System.ArgumentException("Invalid Sort Order!");
                }
            });
        }

        public void SingleOrderDataSet(TableFilterModel tableFilterPayload)
        {
            switch (tableFilterPayload.SortOrder)
            {
                case (int) SortingEnumeration.OrderByAsc:
                    _linqOperator.OrderBy(tableFilterPayload.SortField.FirstCharToUpper());
                    break;

                case (int) SortingEnumeration.OrderByDesc:
                    _linqOperator.OrderByDescending(tableFilterPayload.SortField.FirstCharToUpper());
                    break;

                default:
                    throw new System.ArgumentException("Invalid Sort Order!");
            }
        }

        public void FilterDataSet(string key, TableFilterContext value)
        {
            BaseFilterDataSet(key, value, OperatorEnumeration.None);
        }

        private void BaseFilterDataSet(string key, TableFilterContext value, OperatorEnumeration operatorAction)
        {
            if (value.Value == null)
                return;

            switch (value.MatchMode)
            {
                case FilterTypeMatchModeStartsWith:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantStartsWith
                        , operatorAction);
                    break;

                case FilterTypeMatchModeContains:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantContains
                        , operatorAction);
                    break;

                case FilterTypeMatchModeIn:
                    _linqOperator.AddFilterListProperty(key.FirstCharToUpper(), value.Value
                        , operatorAction);
                    break;

                case FilterTypeMatchModeEndsWith:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantEndsWith
                        , OperatorEnumeration.None);
                    break;

                case FilterTypeMatchModeEquals:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantEquals
                        , operatorAction);
                    break;

                case FilterTypeMatchModeNotContains:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantContains
                        , OperatorEnumeration.None, true);
                    break;

                case FilterTypeMatchModeNotEquals:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantEquals
                        , operatorAction, true);
                    break;

                default:
                    throw new System.ArgumentException("Invalid Match mode!");
            }
        }

        public void FiltersDataSet(string key, IEnumerable<TableFilterContext> values)
        {
            foreach (var filterContext in values)
            {
                var operatorEnum = OperatorConstant.ConvertOperatorEnumeration(filterContext.Operator);
                BaseFilterDataSet(key, filterContext, operatorEnum);
            }
        }

        public void ExecuteFilter() => _linqOperator.WhereExecute();

        public IQueryable<TEntity> GetResult() => _linqOperator.GetResult();
    }
}