using System.Collections.Generic;
using System.Linq;
using PrimeNG.TableFilter.Models;
using PrimeNG.TableFilter.Utils;

namespace PrimeNG.TableFilter.Core
{
    public class TableFilterManager<TEntity> : ITableFilterManager<TEntity>
    {
        private const string ConstantTypeMatchModeStartsWith = "startsWith";
        private const string ConstantTypeMatchModeContains = "contains";
        private const string ConstantTypeMatchModeNotContains = "notContains";
        private const string ConstantTypeMatchModeEndsWith = "endsWith";
        private const string ConstantTypeMatchModeEquals = "equals";
        private const string ConstantTypeMatchModeNotEquals = "notEquals";
        private const string ConstantTypeMatchModeIn = "in";
        private const string ConstantTypeMatchModeLessThan = "lt";
        private const string ConstantTypeMatchModeLessOrEqualsThan = "lte";
        private const string ConstantTypeMatchModeGreaterThan = "gt";
        private const string ConstantTypeMatchModeGreaterOrEqualsThan = "gte";
        private const string ConstantTypeMatchModeBetween = "between";
        private const string ConstantTypeMatchModeIs = "is";
        private const string ConstantTypeMatchModeIsNot = "isNot";
        private const string ConstantTypeMatchModeBefore = "before";
        private const string ConstantTypeMatchModeAfter = "after";
        private const string ConstantTypeMatchModeDateIs = "dateIs";
        private const string ConstantTypeMatchModeDateIsNot = "dateIsNot";
        private const string ConstantTypeMatchModeDateBefore = "dateBefore";
        private const string ConstantTypeMatchModeDateAfter = "dateAfter";


        private readonly ILinqOperator<TEntity> _linqOperator;

        public TableFilterManager(IQueryable<TEntity> dataSet) => _linqOperator = new LinqOperator<TEntity>(dataSet);

        public void MultipleOrderDataSet(TableFilterModel tableFilterPayload)
        {
            tableFilterPayload.MultiSortMeta.Select((value, i) => new { i, value }).ToList().ForEach(o =>
              {
                  switch (o.value.Order)
                  {
                      case (int)SortingEnumeration.OrderByAsc:
                          if (o.i == 0)
                              _linqOperator.OrderBy(o.value.Field.FirstCharToUpper());
                          else
                              _linqOperator.ThenBy(o.value.Field.FirstCharToUpper());
                          break;

                      case (int)SortingEnumeration.OrderByDesc:
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
                case (int)SortingEnumeration.OrderByAsc:
                    _linqOperator.OrderBy(tableFilterPayload.SortField.FirstCharToUpper());
                    break;

                case (int)SortingEnumeration.OrderByDesc:
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
                case ConstantTypeMatchModeStartsWith:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantStartsWith
                        , operatorAction);
                    break;

                case ConstantTypeMatchModeContains:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantContains
                        , operatorAction);
                    break;

                case ConstantTypeMatchModeIn:
                    _linqOperator.AddFilterListProperty(key.FirstCharToUpper(), value.Value
                        , operatorAction);
                    break;

                case ConstantTypeMatchModeEndsWith:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantEndsWith
                        , OperatorEnumeration.None);
                    break;

                case ConstantTypeMatchModeEquals:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantEquals
                        , operatorAction);
                    break;

                case ConstantTypeMatchModeNotContains:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantContains
                        , OperatorEnumeration.None, true);
                    break;

                case ConstantTypeMatchModeNotEquals:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantEquals
                        , operatorAction, true);
                    break;
                case ConstantTypeMatchModeDateIs:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantDateIs
                        , operatorAction);
                    break;
                case ConstantTypeMatchModeDateIsNot:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantDateIs
                        , operatorAction, true);
                    break;
                case ConstantTypeMatchModeDateBefore:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantBefore
                        , operatorAction);
                    break;
                case ConstantTypeMatchModeDateAfter:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantAfter
                        , operatorAction);
                    break;
                case ConstantTypeMatchModeLessThan:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantLessThan
                        , operatorAction);
                    break;
                case ConstantTypeMatchModeLessOrEqualsThan:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantLessThanOrEqual
                        , operatorAction);
                    break;
                case ConstantTypeMatchModeGreaterThan:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantGreaterThan
                        , operatorAction);
                    break;
                case ConstantTypeMatchModeGreaterOrEqualsThan:
                    _linqOperator.AddFilterProperty(key.FirstCharToUpper(), value.Value,
                        LinqOperatorConstants.ConstantGreaterThanOrEqual
                        , operatorAction);
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