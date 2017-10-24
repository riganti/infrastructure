using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// Builds a predicate expression from an <see cref="FilterDTOBase" />.
    /// </summary>
    /// <typeparam name="T">The type of filtered objects.</typeparam>
    public class FilterPredicateBuilder<T>
    {
        private readonly Type objectType = typeof(T);

        /// <summary>
        /// Builds a predicate expression from the given <see cref="FilterDTOBase" />.
        /// </summary>
        /// <param name="filter">The filter to match objects of type <typeparamref name="T" />.</param>
        public Expression<Func<T, bool>> BuildPredicate(FilterDTOBase filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var parameter = Expression.Parameter(objectType, "x");
            var body = BuildFilterExpression(filter, parameter);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private Expression BuildFilterExpression(FilterDTOBase filter, Expression parameter)
        {
            if (!filter.IsValid())
            {
                throw new ArgumentException("the filter is invalid", nameof(filter));
            }

            if (filter is FilterGroupDTO group)
            {
                return BuildGroupExpression(group, parameter);
            }

            if (filter is FilterConditionDTO condition)
            {
                return BuildConditionExpression(condition, parameter);
            }

            throw new NotSupportedException();
        }

        private Expression BuildGroupExpression(FilterGroupDTO group, Expression parameter)
        {
            if (group.IsEmpty())
            {
                throw new ArgumentException("the group is empty", nameof(group));
            }

            Expression left = null, right = null;
            var logicExpressionType = GetLogicExpressionType(group.Logic);

            foreach (var filter in group.GetValidFilters())
            {
                if (left == null)
                {
                    left = BuildFilterExpression(filter, parameter);
                }
                else if (right == null)
                {
                    right = BuildFilterExpression(filter, parameter);
                }
                else
                {
                    left = Expression.MakeBinary(logicExpressionType, left, right);
                    right = BuildFilterExpression(filter, parameter);
                }
            }

            var result = right != null
                ? Expression.MakeBinary(logicExpressionType, left, right)
                : left;

            return ShouldNegate(group.Logic)
                ? Expression.Not(result)
                : result;
        }

        private Expression BuildConditionExpression(FilterConditionDTO condition, Expression parameter)
        {
            if (string.IsNullOrEmpty(condition.FieldName))
            {
                throw new ArgumentException("the condition.FieldName is null or empty", nameof(condition));
            }

            var property = Expression.Property(parameter, condition.FieldName);
            var valueType = GetValueType(property.Type);

            var value = Expression.Constant(GetFilterValue(condition, valueType), valueType);

            if (IsMethodOperator(condition.Operator))
            {
                var operatorMethodInfo = GetOperatorMethodInfo(property.Type, condition.Operator);

                return operatorMethodInfo.IsStatic 
                    ? Expression.Call(null, operatorMethodInfo, property, value) 
                    : Expression.Call(property, operatorMethodInfo, value);
            }

            var operatorExpressionType = GetOperatorExpressionType(condition.Operator);
            return Expression.MakeBinary(operatorExpressionType, property, value);
        }

        private ExpressionType GetLogicExpressionType(FilterLogicType filterLogic)
        {
            switch (filterLogic)
            {
                case FilterLogicType.And:
                case FilterLogicType.NotAnd:
                    return ExpressionType.AndAlso;
                case FilterLogicType.Or:
                case FilterLogicType.NotOr:
                    return ExpressionType.OrElse;
            }

            throw new NotSupportedException();
        }

        private ExpressionType GetOperatorExpressionType(FilterOperatorType? filterOperator)
        {
            switch (filterOperator)
            {
                case FilterOperatorType.Equal:
                case FilterOperatorType.Null:
                case FilterOperatorType.True:
                case FilterOperatorType.False:
                    return ExpressionType.Equal;
                case FilterOperatorType.NotEqual:
                case FilterOperatorType.NotNull:
                    return ExpressionType.NotEqual;
                case FilterOperatorType.GreaterThan:
                    return ExpressionType.GreaterThan;
                case FilterOperatorType.GreaterThanOrEqual:
                    return ExpressionType.GreaterThanOrEqual;
                case FilterOperatorType.LessThan:
                    return ExpressionType.LessThan;
                case FilterOperatorType.LessThanOrEqual:
                    return ExpressionType.LessThanOrEqual;
            }

            throw new NotSupportedException();
        }

        private MethodInfo GetOperatorMethodInfo(Type propertyType, FilterOperatorType? filterOperator)
        {
            MethodInfo method;
            var methodName = filterOperator.ToString();

            if (IsLinqOperator(filterOperator) && IsEnumerable(propertyType, out var elementType))
            {
                method = typeof(Enumerable).GetTypeInfo().GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Single(mi => mi.Name == methodName && mi.GetParameters().Length == 2)
                    .MakeGenericMethod(elementType);
            }
            else
            {
                method = propertyType.GetTypeInfo().GetMethod(methodName, new[] { propertyType });
            }

            if (method == null)
            {
                throw new InvalidOperationException($"Method '{methodName}' was not found on type '{propertyType.FullName}'.");
            }

            return method;
        }

        private object GetFilterValue(FilterConditionDTO filter, Type valueType)
        {
            var value = filter.Value;

            if (filter.Operator == FilterOperatorType.True)
            {
                return true;
            }

            if (filter.Operator == FilterOperatorType.False)
            {
                return false;
            }

            if (value == null || IsNullOperator(filter.Operator))
            {
                return null;
            }

            if (value.GetType() == valueType)
            {
                return value;
            }

            if (valueType.GetTypeInfo().IsEnum && valueType.GetTypeInfo().IsEnumDefined(value))
            {
                return Enum.Parse(valueType, value.ToString());
            }

            if (value is IConvertible)
            {
                return Convert.ChangeType(value, valueType);
            }

            return value;
        }

        private Type GetValueType(Type propertyType)
        {
            if (IsEnumerable(propertyType, out var elementType))
            {
                return elementType;
            }

            if (propertyType.GetTypeInfo().IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Nullable.GetUnderlyingType(propertyType);
            }

            return propertyType;
        }

        private bool IsMethodOperator(FilterOperatorType? filterOperator)
        {
            return IsLinqOperator(filterOperator) || filterOperator == FilterOperatorType.StartsWith || filterOperator == FilterOperatorType.EndsWith;
        }

        private bool IsLinqOperator(FilterOperatorType? filterOperator)
        {
            return filterOperator == FilterOperatorType.Contains;
        }

        private bool IsNullOperator(FilterOperatorType? filterOperator)
        {
            return filterOperator == FilterOperatorType.Null || filterOperator == FilterOperatorType.NotNull;
        }

        private bool ShouldNegate(FilterLogicType filterLogic)
        {
            return filterLogic == FilterLogicType.NotAnd || filterLogic == FilterLogicType.NotOr;
        }

        private bool IsEnumerable(Type propertyType, out Type elementType)
        {
            if (propertyType == typeof(string))
            {
                elementType = null;
                return false;
            }

            bool IsEnumerableCheck(Type type)
                => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);

            if (IsEnumerableCheck(propertyType))
            {
                elementType = propertyType.GetTypeInfo().GetGenericArguments()[0];
                return true;
            }

            foreach (var interfaceType in propertyType.GetTypeInfo().GetInterfaces())
            {
                if (IsEnumerableCheck(interfaceType))
                {
                    elementType = interfaceType.GetTypeInfo().GetGenericArguments()[0];
                    return true;
                }
            }

            elementType = null;
            return false;
        }
    }
}