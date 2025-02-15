using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ShardingCore.Core.Internal.Visitors.Selects;
using ShardingCore.Exceptions;
using ShardingCore.Extensions;
using ShardingCore.Sharding.MergeContexts;
using ShardingCore.Sharding.Visitors.Selects;

namespace ShardingCore.Core.Internal.Visitors
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: Wednesday, 13 January 2021 11:04:50
    * @Email: 326308290@qq.com
    */
    internal class QueryableExtraDiscoverVisitor : ShardingExpressionVisitor
    {
        private GroupByContext _groupByContext = new GroupByContext();
        private SelectContext _selectContext = new SelectContext();
        private PaginationContext _paginationContext = new PaginationContext();
        private OrderByContext _orderByContext = new OrderByContext();


        public SelectContext GetSelectContext()
        {
            return _selectContext;
        }

        public GroupByContext GetGroupByContext()
        {
            return _groupByContext;
        }

        public PaginationContext GetPaginationContext()
        {
            return _paginationContext;
        }
        public OrderByContext GetOrderByContext()
        {
            return _orderByContext;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;
            if (node.Method.Name == nameof(Queryable.Skip))
            {
                if (_paginationContext.HasSkip())
                    throw new ShardingCoreInvalidOperationException("more than one skip found");
                _paginationContext.Skip = (int)GetExpressionValue(node.Arguments[1]);
            }
            else if (node.Method.Name == nameof(Queryable.Take))
            {
                if (_paginationContext.HasTake())
                    throw new ShardingCoreInvalidOperationException("more than one take found");
                _paginationContext.Take = (int)GetExpressionValue(node.Arguments[1]);
            }
            else if (method.Name == nameof(Queryable.OrderBy) || method.Name == nameof(Queryable.OrderByDescending) || method.Name == nameof(Queryable.ThenBy) || method.Name == nameof(Queryable.ThenByDescending))
            {
                if (typeof(IOrderedQueryable).IsAssignableFrom(node.Type))
                {
                    var expression = (((node.Arguments[1] as UnaryExpression).Operand as LambdaExpression).Body as MemberExpression);
                    if (expression == null)
                        throw new NotSupportedException("sharding order not support ");
                    List<string> properties = new List<string>();
                    GetProperty(properties, expression);
                    if (!properties.Any())
                        throw new NotSupportedException("sharding order only support property expression");
                    properties.Reverse();
                    var propertyExpression = string.Join(".", properties);
                    _orderByContext.PropertyOrders.AddFirst(new PropertyOrder(propertyExpression, method.Name == nameof(Queryable.OrderBy) || method.Name == nameof(Queryable.ThenBy), expression.Member.DeclaringType));

                }
            }
            else if (node.Method.Name == nameof(Queryable.GroupBy))
            {
                if (_groupByContext.GroupExpression == null)
                {
                    var expression = (node.Arguments[1] as UnaryExpression).Operand as LambdaExpression;
                    if (expression == null)
                        throw new NotSupportedException("sharding group not support ");
                    _groupByContext.GroupExpression = expression;
                }
            }
            else if (node.Method.Name == nameof(Queryable.Select))
            {
                if (_selectContext.SelectProperties.IsEmpty())
                {
                    var expression = ((node.Arguments[1] as UnaryExpression).Operand as LambdaExpression).Body;
                    if (expression is NewExpression newExpression)
                    {
                        var aggregateDiscoverVisitor = new QuerySelectDiscoverVisitor(_selectContext);
                        aggregateDiscoverVisitor.Visit(newExpression);
                    } else if (expression is MemberExpression memberExpression)
                    {
                        
                        var declaringType = memberExpression.Member.DeclaringType;
                        var memberName = memberExpression.Member.Name;
                        var propertyInfo = declaringType.GetProperty(memberName);
                        _selectContext.SelectProperties.Add(new SelectOwnerProperty(declaringType, propertyInfo));
                        //memberExpression.Acc
                    }
                    //if (expression != null)
                    //{
                    //    var aggregateDiscoverVisitor = new QuerySelectDiscoverVisitor(_selectContext);
                    //    aggregateDiscoverVisitor.Visit(expression);
                    //}
                }
            }

            return base.VisitMethodCall(node);
        }
        private void GetProperty(List<string> properties, MemberExpression memberExpression)
        {
            properties.Add(memberExpression.Member.Name);
            if (memberExpression.Expression is MemberExpression member)
            {
                GetProperty(properties, member);
            }
        }



    }
}