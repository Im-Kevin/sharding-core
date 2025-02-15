﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ShardingCore.Core.VirtualRoutes.DataSourceRoutes.RouteRuleEngine;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RoutingRuleEngine;
using ShardingCore.Sharding.ShardingExecutors.QueryableCombines;

namespace ShardingCore.Sharding.ShardingExecutors.Abstractions
{
    public interface IMergeQueryCompilerContext : IQueryCompilerContext
    {
        QueryCombineResult GetQueryCombineResult();
        TableRouteResult[] GetTableRouteResults();
        DataSourceRouteResult GetDataSourceRouteResult();

        bool IsCrossTable();
        bool IsCrossDataSource();
        //bool IsEnumerableQuery();
    }
}
