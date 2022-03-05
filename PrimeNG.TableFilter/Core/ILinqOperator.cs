using System.Linq;

namespace PrimeNG.TableFilter.Core
{
    public interface ILinqOperator<out TEntity>
    {
        void AddFilterProperty(string propertyName, object propertyValue, string extensionMethod,
            OperatorEnumeration operatorAction, bool isNegation = false);
        void AddFilterListProperty(string propertyName, object propertyValue, OperatorEnumeration operatorAction);
        void WhereExecute();
        void OrderBy(string orderProperty);
        void OrderByDescending(string orderProperty);
        void ThenBy(string orderProperty);
        void ThenByDescending(string orderProperty);
        IQueryable<TEntity> GetResult();
    }
}