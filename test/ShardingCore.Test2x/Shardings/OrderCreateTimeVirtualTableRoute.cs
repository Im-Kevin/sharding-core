﻿using System;
using System.Collections.Generic;
using ShardingCore.Core.EntityMetadatas;
using ShardingCore.Sharding.PaginationConfigurations;
using ShardingCore.Test2x.Domain.Entities;
using ShardingCore.VirtualRoutes.Months;

namespace ShardingCore.Test2x.Shardings
{
   public  class OrderCreateTimeVirtualTableRoute:AbstractSimpleShardingMonthKeyDateTimeVirtualTableRoute<Order>
    {
        public override bool? EnableRouteParseCompileCache => true;
        public override DateTime GetBeginTime()
        {
            return new DateTime(2021, 1, 1);
        }

        public override List<string> GetAllTails()
        {
            var allTails = base.GetAllTails();
            allTails.Add("202112");
            return allTails;
        }

        public override void Configure(EntityMetadataTableBuilder<Order> builder)
        {
            
        }

        public override IPaginationConfiguration<Order> CreatePaginationConfiguration()
        {
            return new OrderCreateTimePaginationConfiguration();
        }
        public override bool AutoCreateTableByTime()
        {
            return true;
        }
    }

   public class OrderCreateTimePaginationConfiguration : IPaginationConfiguration<Order>
   {
       public void Configure(PaginationBuilder<Order> builder)
       {
           builder.PaginationSequence(o => o.CreateTime)
               .UseQueryMatch(PaginationMatchEnum.Owner | PaginationMatchEnum.Named | PaginationMatchEnum.PrimaryMatch)
               .UseAppendIfOrderNone().UseRouteComparer(Comparer<string>.Default);
       }
   }
}
