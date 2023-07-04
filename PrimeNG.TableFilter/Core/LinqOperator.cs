using PrimeNG.TableFilter.Models;
using PrimeNG.TableFilter.Utils;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

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

        public void AddFilterListProperty(
            string propertyName,
            object propertyValue,
            OperatorEnumeration operatorAction)
        {
            var property = _context.DataSetType.GetProperty(propertyName);
            var propertyType = property?.PropertyType;

            if (propertyType == null)
                return;

            var castValue = ObjectCasterUtil.CastPropertiesTypeList(property, propertyValue);
            var methodInfo = castValue.GetType()
                .GetMethod(LinqOperatorConstants.ConstantContains, new[] { propertyType });

            var list = Expression.Constant(castValue);
            var value = Expression.Property(_context.ParameterExpression, propertyName);
            AddNormalExpression(operatorAction, list, methodInfo, value);
        }

        public void AddFilterProperty(
            string propertyName, 
            object propertyValue, 
            string extensionMethod,
            OperatorEnumeration operatorAction, 
            bool isNegation = false)
        {
            PropertyInfo property = null;
            foreach (var item in propertyName.Split('.'))
            {
                if (property == null)
                    property = _context.DataSetType.GetProperty(item.FirstCharToUpper());
                else
                    property = property.PropertyType.GetProperty(item.FirstCharToUpper());
            }
            var propertyType = property?.PropertyType;
            
            if (propertyType == null)
                return;

            if (propertyType !=  typeof(string) && extensionMethod == LinqOperatorConstants.ConstantContains)
            {
                if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                {
                    extensionMethod = LinqOperatorConstants.ConstantDateIs;
                    if (DateTime.TryParse(propertyValue.ToString(), out var currentDate))
                        propertyValue = currentDate;
                    else
                        return;
                }
                else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                {
                    extensionMethod = LinqOperatorConstants.ConstantEquals;
                    if (bool.TryParse(propertyValue.ToString(),out var currentBoolean))
                        propertyValue = currentBoolean;
                    else
                        return;
                }
                else if (IsNumericType(propertyType))
                {
                    extensionMethod = LinqOperatorConstants.ConstantEquals;
                    if ((propertyType == typeof(short) || propertyType == typeof(short?)) && short.TryParse(propertyValue.ToString(), out var currentShort))
                        propertyValue = currentShort;
                    else if ((propertyType == typeof(int) || propertyType == typeof(int?)) && int.TryParse(propertyValue.ToString(), out var currentInt))
                        propertyValue = currentInt;
                    else if ((propertyType == typeof(long) || propertyType == typeof(long?)) && long.TryParse(propertyValue.ToString(), out var currentLong))
                        propertyValue = currentLong;
                    else if ((propertyType == typeof(float) || propertyType == typeof(float?)) && float.TryParse(propertyValue.ToString(), out var currentFloat))
                        propertyValue = currentFloat;
                    else if ((propertyType == typeof(double) || propertyType == typeof(double?)) && double.TryParse(propertyValue.ToString(), out var currentDouble))
                        propertyValue = currentDouble;
                    else if ((propertyType == typeof(decimal) || propertyType == typeof(decimal?)) && decimal.TryParse(propertyValue.ToString(), out var currentDecimal))
                        propertyValue = currentDecimal;
                    else if ((propertyType.IsEnum || (Nullable.GetUnderlyingType(propertyType)?.IsEnum == true)) && int.TryParse(propertyValue.ToString(), out var currentEnum))
                        propertyValue = currentEnum;
                    else
                        return;
                }
            }

            if (!IsPropertyTypeAndFilterMatchModeValid(propertyType, extensionMethod))
                throw new ArgumentException($"Property ${propertyName} not support method ${extensionMethod}");

            var castValue = ObjectCasterUtil.CastPropertiesType(property, propertyValue);
            var propertyConstant = propertyType.IsEnum ? Expression.Constant(castValue, typeof(int)) : Expression.Constant(castValue, propertyType);

            if (IsNullableType(propertyType))
            {
                switch (castValue)
                {
                    case DateTime _:
                        ComposeLambdaForDateTimeProperty(propertyName, extensionMethod, operatorAction, isNegation, castValue);
                        break;
                    case bool _:
                        // boolean only have "equals" and "notEquals" match modes
                        ComposeEqualsLinqExpression(propertyName, operatorAction, isNegation, castValue, _context.ParameterExpression);
                        break;
                    // nullable numeric type
                    default:
                        ComposeLambdaForNumericProperty(propertyName, extensionMethod, operatorAction, isNegation, castValue);
                        break;
                }
            }
            else
            {
                switch (castValue)
                {
                    case DateTime _:
                        ComposeLambdaForDateTimeProperty(propertyName, extensionMethod, operatorAction, isNegation, castValue);
                        break;
                    case bool _:
                        ComposeEqualsLinqExpression(propertyName, operatorAction, isNegation, castValue, _context.ParameterExpression);
                        return;
                    case string _:
                        {
                            MemberExpression propertyAccess = null;
                            foreach (var item in propertyName.Split('.'))
                            {
                                if (propertyAccess == null)
                                    propertyAccess = Expression.PropertyOrField(_context.ParameterExpression, item.FirstCharToUpper());
                                else
                                    propertyAccess = Expression.PropertyOrField(propertyAccess, item.FirstCharToUpper());
                            }

                            var methodInfo = propertyType.GetMethod(extensionMethod, new[] { propertyType });
                            if (isNegation)
                                AddNegationExpression(operatorAction, propertyAccess, methodInfo, propertyConstant);
                            else
                                AddNormalExpression(operatorAction, propertyAccess, methodInfo, propertyConstant);
                            break;
                        }
                    // nullable numeric type
                    default:
                        ComposeLambdaForNumericProperty(propertyName, extensionMethod, operatorAction, isNegation, castValue);
                        break;
                }
            }

        }
        private void AddNormalExpression(
            OperatorEnumeration operatorAction, 
            Expression propertyAccess,
            MethodInfo methodInfo, 
            Expression converted)
        {
            var callMethod = Expression.Call(propertyAccess,
                methodInfo ?? throw new InvalidOperationException(), converted);
            AddLambdaExpression(operatorAction, callMethod);
        }

        private void AddNormalExpression(OperatorEnumeration operatorAction, Expression propertyAccess)
        {
            AddLambdaExpression(operatorAction, propertyAccess);
        }

        private void AddNegationExpression(
            OperatorEnumeration operatorAction,
            MemberExpression propertyAccess,
            MethodInfo methodInfo,
            Expression converted)
        {
            if (propertyAccess == null) throw new ArgumentNullException(nameof(propertyAccess));
            var callMethod = Expression.Not(Expression.Call(propertyAccess,
                methodInfo ?? throw new InvalidOperationException(), converted));
            AddLambdaExpression(operatorAction, callMethod);
        }
        private void AddNegationExpression(OperatorEnumeration operatorAction, Expression propertyAccess)
        {
            AddLambdaExpression(operatorAction, Expression.Not(propertyAccess));

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
                    case OperatorEnumeration.None:
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
            PropertyInfo property = null;
            foreach (var item in orderByProperty.Split('.'))
            {
                if (property == null)
                    property = _context.DataSetType.GetProperty(item.FirstCharToUpper());
                else
                    property = property.PropertyType.GetProperty(item.FirstCharToUpper());
            }
            var propertyType = property?.PropertyType;

            if (propertyType == null)
                return;

            MemberExpression propertyAccess = null;
            foreach (var item in orderByProperty.Split('.'))
            {
                if (propertyAccess == null)
                    propertyAccess = Expression.PropertyOrField(_context.ParameterExpression, item);
                else
                    propertyAccess = Expression.PropertyOrField(propertyAccess, item);
            }

            var orderByExpression = Expression.Lambda(propertyAccess, _context.ParameterExpression);
            var resultExpression = Expression.Call(typeof(Queryable), command,
                new[] { _context.DataSetType, propertyType },
                _context.DataSet.Expression, Expression.Quote(orderByExpression));
            _context.DataSet = _context.DataSet.Provider.CreateQuery<TEntity>(resultExpression);
        }
        /// <summary>
        /// Checks if provided type is nullable
        /// </summary>
        /// <param name="propertyType">Type to check</param>
        /// <returns><code>True</code> if nullable, otherwise <code>False</code></returns>
        private static bool IsNullableType(Type propertyType)
        {
            return propertyType.IsGenericType
                       && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        /// <summary>
        /// Checks if provided type is numeric type
        /// </summary>
        /// <param name="propertyType">Type to check</param>
        /// <returns><code>True</code> if nullable, otherwise <code>False</code></returns>
        private static bool IsNumericType(Type propertyType)
        {
            return ((propertyType == typeof(short) || propertyType == typeof(short?)) || 
                (propertyType == typeof(int) || propertyType == typeof(int?)) || 
                (propertyType == typeof(long) || propertyType == typeof(long?)) || 
                (propertyType == typeof(float) || propertyType == typeof(float?)) || 
                (propertyType == typeof(double) || propertyType == typeof(double?)) || 
                (propertyType == typeof(decimal) || propertyType == typeof(decimal?)) ||
                (propertyType.IsEnum || (Nullable.GetUnderlyingType(propertyType)?.IsEnum == true)));
        }
        /// <summary>
        /// Checks if for provided <paramref name="propertyType"/>, <paramref name="extensionMethod"/> is valid
        /// </summary>
        /// <param name="propertyType">Type of filter value instance</param>
        /// <param name="extensionMethod">Method to check for provided <paramref name="propertyType"/> </param>
        /// <returns></returns>
        private static bool IsPropertyTypeAndFilterMatchModeValid(Type propertyType, string extensionMethod)
        {
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                var validDateTimeLinqMethods = new[] { LinqOperatorConstants.ConstantDateIs, LinqOperatorConstants.ConstantBefore, LinqOperatorConstants.ConstantAfter };
                return validDateTimeLinqMethods.Contains(extensionMethod);
            }

            if (propertyType == typeof(string))
            {
                var validStringLinqMethods = new[] { LinqOperatorConstants.ConstantEquals, LinqOperatorConstants.ConstantEndsWith, LinqOperatorConstants.ConstantContains, LinqOperatorConstants.ConstantStartsWith };
                return validStringLinqMethods.Contains(extensionMethod);
            }

            if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                return LinqOperatorConstants.ConstantEquals == extensionMethod;
            }

            if (!IsNumericType(propertyType)) return false;
            var validNumericMethods = new[] { LinqOperatorConstants.ConstantEquals, LinqOperatorConstants.ConstantLessThan, LinqOperatorConstants.ConstantLessThanOrEqual, LinqOperatorConstants.ConstantGreaterThan, LinqOperatorConstants.ConstantGreaterThanOrEqual };
            return validNumericMethods.Contains(extensionMethod);
        }



        /// <summary>
        /// Composes LINQ expression for numeric type of based on <paramref name="extensionMethod"/>
        /// </summary>
        /// <param name="propertyName">Name of property to compose expression</param>
        /// <param name="extensionMethod">Extension method for which expression is composed</param>
        /// <param name="operatorAction">Operator action for expression</param>
        /// <param name="isNegation">Flag that indicates if expression should be negation</param>
        /// <param name="castValue">Casted value of <paramref name="propertyName"/></param>
        private void ComposeLambdaForNumericProperty(string propertyName, string extensionMethod, OperatorEnumeration operatorAction, bool isNegation, object castValue)
        {
            LambdaExpression dynamicExpression;
            var x = _context.ParameterExpression;
            switch (extensionMethod)
            {
                case LinqOperatorConstants.ConstantEquals:
                    {
                        ComposeEqualsLinqExpression(propertyName, operatorAction, isNegation, castValue, x);
                        break;
                    }
                case LinqOperatorConstants.ConstantLessThan:
                    {
                        dynamicExpression = DynamicExpressionParser.ParseLambda(new[] { x }, null, $"{propertyName}<@0", castValue);
                        if (isNegation)
                            AddNegationExpression(operatorAction, dynamicExpression.Body);
                        else
                            AddNormalExpression(operatorAction, dynamicExpression.Body);
                        break;
                    }
                case LinqOperatorConstants.ConstantLessThanOrEqual:
                    {
                        dynamicExpression = DynamicExpressionParser.ParseLambda(new[] { x }, null, $"{propertyName}<=@0", castValue);
                        if (isNegation)
                            AddNegationExpression(operatorAction, dynamicExpression.Body);
                        else
                            AddNormalExpression(operatorAction, dynamicExpression.Body);
                        break;
                    }
                case LinqOperatorConstants.ConstantGreaterThan:
                    {
                        dynamicExpression = DynamicExpressionParser.ParseLambda(new[] { x }, null, $"{propertyName}>@0", castValue);
                        if (isNegation)
                            AddNegationExpression(operatorAction, dynamicExpression.Body);
                        else
                            AddNormalExpression(operatorAction, dynamicExpression.Body);
                        break;
                    }
                case LinqOperatorConstants.ConstantGreaterThanOrEqual:
                    {
                        dynamicExpression = DynamicExpressionParser.ParseLambda(new[] { x }, null, $"{propertyName}>=@0", castValue);
                        if (isNegation)
                            AddNegationExpression(operatorAction, dynamicExpression.Body);
                        else
                            AddNormalExpression(operatorAction, dynamicExpression.Body);
                        break;
                    }
            }
        }

        private void ComposeEqualsLinqExpression(string propertyName, OperatorEnumeration operatorAction,
            bool isNegation, object castValue, ParameterExpression x)
        {
            var dynamicExpression = DynamicExpressionParser.ParseLambda(new[] { x }, null, $"{propertyName}==@0", castValue);
            if (isNegation)
                AddNegationExpression(operatorAction, dynamicExpression.Body);
            else
                AddNormalExpression(operatorAction, dynamicExpression.Body);
        }

        /// <summary>
        /// Composes LINQ expression for date time based on <paramref name="extensionMethod"/>
        /// </summary>
        /// <param name="propertyName">Name of property to compose expression</param>
        /// <param name="extensionMethod">Extension method for which expression is composed</param>
        /// <param name="operatorAction">Operator action for expression</param>
        /// <param name="isNegation">Flag that indicates if expression should be negation</param>
        /// <param name="castValue">Casted value of <paramref name="propertyName"/></param>
        private void ComposeLambdaForDateTimeProperty(string propertyName, string extensionMethod, OperatorEnumeration operatorAction, bool isNegation, object castValue)
        {
            var dateTime = (DateTime)castValue;
            LambdaExpression dynamicExpression;
            var hoursDefined = dateTime.TimeOfDay.Hours > 0;
            var minutesDefined = dateTime.TimeOfDay.Minutes > 0;
            var secondsDefined = dateTime.TimeOfDay.Seconds > 0;
            var isTimeDefined = hoursDefined || minutesDefined || secondsDefined;
            switch (extensionMethod)
            {
                case LinqOperatorConstants.ConstantDateIs:
                    {
                        var x = _context.ParameterExpression;

                        dynamicExpression = isTimeDefined ?
                             DynamicExpressionParser.ParseLambda(new[] { x }, null, $"{propertyName}==@0", dateTime)
                            : DynamicExpressionParser.ParseLambda(new[] { x }, null, $"{propertyName}>=@0 && {propertyName}<= @1", dateTime.Date, dateTime.Date.AddDays(1).AddTicks(-1));
                        if (isNegation)
                            AddNegationExpression(operatorAction, dynamicExpression.Body);
                        else
                            AddNormalExpression(operatorAction, dynamicExpression.Body);
                        break;
                    }
                case LinqOperatorConstants.ConstantBefore:
                    {
                        var x = _context.ParameterExpression;
                        dynamicExpression = DynamicExpressionParser.ParseLambda(new[] { x }, null, $"{propertyName}<@0", dateTime);
                        if (isNegation)
                            AddNegationExpression(operatorAction, dynamicExpression.Body);
                        else
                            AddNormalExpression(operatorAction, dynamicExpression.Body);
                        break;
                    }
                case LinqOperatorConstants.ConstantAfter:
                    {
                        var x = _context.ParameterExpression;
                        dynamicExpression = DynamicExpressionParser.ParseLambda(new[] { x }, null, $"{propertyName}>@0", dateTime);
                        if (isNegation)
                            AddNegationExpression(operatorAction, dynamicExpression.Body);
                        else
                            AddNormalExpression(operatorAction, dynamicExpression.Body);
                        break;
                    }
            }
        }

        public IQueryable<TEntity> GetResult() => _context.DataSet;
    }
}