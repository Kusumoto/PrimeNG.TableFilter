using System;
using System.Linq;
using System.Linq.Expressions;

namespace PrimeNG.TableFilter.Models
{
    public class LinqContext<TEntity>
    {
        public IQueryable<TEntity> DataSet { get; set; }
        public ParameterExpression ParameterExpression { get; set; }
        public Type DataSetType { get; set; }
        public Expression<Func<TEntity, bool>> Expressions { get; set; }
    }
}