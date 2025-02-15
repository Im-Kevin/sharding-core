﻿using ShardingCore.Core.ShardingPage.Abstractions;
using ShardingCore.Sharding.MergeEngines.Abstractions.InMemoryMerge;
using ShardingCore.Sharding.MergeEngines.Executors.Abstractions;
using ShardingCore.Sharding.MergeEngines.Executors.Methods;
using System.Collections.Generic;
using System.Linq;

namespace ShardingCore.Sharding.StreamMergeEngines
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/8/17 22:36:14
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    internal class CountAsyncInMemoryMergeEngine<TEntity> : AbstractEnsureMethodCallInMemoryAsyncMergeEngine<TEntity,int>
    {
        private readonly IShardingPageManager _shardingPageManager;
        public CountAsyncInMemoryMergeEngine(StreamMergeContext streamMergeContext) : base(streamMergeContext)
        {
            _shardingPageManager = ShardingContainer.GetService<IShardingPageManager>();
        }

        protected override int DoMergeResult(List<RouteQueryResult<int>> resultList)
        {

            if (_shardingPageManager.Current != null)
            {
                int r = 0;
                foreach (var routeQueryResult in resultList)
                {
                    _shardingPageManager.Current.RouteQueryResults.Add(new RouteQueryResult<long>(routeQueryResult.DataSourceName, routeQueryResult.TableRouteResult, routeQueryResult.QueryResult));
                    r += routeQueryResult.QueryResult;
                }

                return r;
            }
            return resultList.Sum(o => o.QueryResult);
        }

        protected override IExecutor<RouteQueryResult<int>> CreateExecutor0(bool async)
        {
            return new CountMethodExecutor<TEntity>(GetStreamMergeContext());
        }
    }
}