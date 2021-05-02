using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PrimeNG.TableFilter.Models;
using PrimeNG.TableFilter.Utils;

namespace PrimeNG.TableFilter.Core
{
    public class LinqOperator<TEntity> : ILinqOperator<TEntity>
    {
        private readonly LinqContext<TEntity> _context;

        public LinqOperator(IQueryable<TEntity> dataSet)
        {
            _context = new LinqContext<TEntity>
            {
                DataSet = dataSet,
                DataSetType = typeof(TEntity),
                ParameterExpression = Expression.Parameter(typeof(TEntity), "e")
            };
        }

        public void AddFilterListProperty(string propertyName, object propertyValue, OperatorEnumeration operatorAction)
        {
            var property = _context.DataSetType.GetProperty(propertyName);
            var propertyType = property?.PropertyType;

            if (propertyType == null)
                return;

            var castValue = ObjectCasterUtil.CastPropertiesTypeList(property, propertyValue);
            var methodInfo = castValue.GetType()
                .GetMethod(LinqOperatorConstants.ConstantContains, new[] {propertyType});

            var list = Expression.Constant(castValue);
            var value = Expression.Property(_context.ParameterExpression, propertyName);
            AddNormalExpression(operatorAction, list, methodInfo, value);
        }

        public void AddFilterProperty(string propertyName, object propertyValue, string extensionMethod,
            OperatorEnumeration operatorAction, bool isNegation = false)
        {
            var property = _context.DataSetType.GetProperty(propertyName);
            var propertyType = property?.PropertyType;
            if (propertyType != typeof(string) && extensionMethod != LinqOperatorConstants.ConstantEquals)
                throw new ArgumentException($"Property ${propertyName} not support method ${extensionMethod}");

            if (propertyType == null)
                return;

            var propertyAccess =
                Expression.MakeMemberAccess(_context.ParameterExpression,
                    property ?? throw new InvalidOperationException());
            var methodInfo = propertyType.GetMethod(extensionMethod, new[] {propertyType});
            var castValue = ObjectCasterUtil.CastPropertiesType(property, propertyValue);
            var propertyConstant = Expression.Constant(castValue, propertyType);
            if (propertyType.IsGenericType
                && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                && propertyType == typeof(DateTime?))
            {
                var converted = Expression.Convert(propertyConstant, typeof(object));
                if (isNegation)
                    AddNegationExpression(operatorAction, propertyAccess, methodInfo, converted);
                else
                    AddNormalExpression(operatorAction, propertyAccess, methodInfo, converted);
            }
            else
            {
                if (isNegation)
                    AddNegationExpression(operatorAction, propertyAccess, methodInfo, propertyConstant);
                else
                    AddNormalExpression(operatorAction, propertyAccess, methodInfo, propertyConstant);
            }
        }

        private void AddNormalExpression(OperatorEnumeration operatorAction, Expression propertyAccess,
            MethodInfo methodInfo, Expression converted)
        {
            var callMethod = Expression.Call(propertyAccess,
                methodInfo ?? throw new InvalidOperationException(), converted);
            AddLambdaExpression(operatorAction, callMethod);
        }

        private void AddNegationExpression(OperatorEnumeration operatorAction, MemberExpression propertyAccess,
            MethodInfo methodInfo, Expression converted)
        {
            if (propertyAccess == null) throw new ArgumentNullException(nameof(propertyAccess));
            var callMethod = Expression.Not(Expression.Call(propertyAccess,
                methodInfo ?? throw new InvalidOperationException(), converted));
            AddLambdaExpression(operatorAction, callMethod);
        }

        private void AddLambdaExpression(OperatorEnumeration operatorAction, Expression callMethod)
        {
            var lambda = Expression.Lambda<Func<TEntity, bool>>(callMethod, _context.ParameterExpression);
            if (_context.Expressions == null)
                _context.Expressions = lambda;
            else
            {
                switch (operatorAction)
                {
                    case OperatorEnumeration.And:

                        _context.Expressions =
                            Expression.Lambda<Func<TEntity, bool>>(
                                Expression.AndAlso(_context.Expressions.Body, lambda.Body),
                                _context.ParameterExpression);
                        break;
                    case OperatorEnumeration.Or:
                        _context.Expressions =
                            Expression.Lambda<Func<TEntity, bool>>(
                                Expression.OrElse(_context.Expressions.Body, lambda.Body),
                                _context.ParameterExpression);
                        break;
                    default:
                        _context.Expressions =
                            Expression.Lambda<Func<TEntity, bool>>(
                                Expression.AndAlso(_context.Expressions.Body, lambda.Body),
                                _context.ParameterExpression);
                        break;
                }
            }
        }

        public void WhereExecute() =>
            _context.DataSet = _context.Expressions != null
                ? _context.DataSet.Where(_context.Expressions)
                : _context.DataSet;

        public void OrderBy(string orderProperty) =>
            BaseOrderExecute(LinqOperatorConstants.ConstantOrderBy, orderProperty);

        public void OrderByDescending(string orderProperty) =>
            BaseOrderExecute(LinqOperatorConstants.ConstantOrderByDescending, orderProperty);

        public void ThenBy(string orderProperty) =>
            BaseOrderExecute(LinqOperatorConstants.ConstantThenBy, orderProperty);

        public void ThenByDescending(string orderProperty) =>
            BaseOrderExecute(LinqOperatorConstants.ConstantThenByDescending, orderProperty);

        private void BaseOrderExecute(string command, string orderByProperty)
        {
            var property = _context.DataSetType.GetProperty(orderByProperty);
            var propertyAccess =
                Expression.MakeMemberAccess(_context.ParameterExpression,
                    property ?? throw new InvalidOperationException());
            var orderByExpression = Expression.Lambda(propertyAccess, _context.ParameterExpression);
            var resultExpression = Expression.Call(typeof(Queryable), command,
                new[] {_context.DataSetType, property.PropertyType},
                _context.DataSet.Expression, Expression.Quote(orderByExpression));
            _context.DataSet = _context.DataSet.Provider.CreateQuery<TEntity>(resultExpression);
        }

        public IQueryable<TEntity> GetResult() => _context.DataSet;
    }
}