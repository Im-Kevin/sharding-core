﻿using System;
using ShardingCore.Core.EntityMetadatas;
using ShardingCore.Sharding.PaginationConfigurations;
using ShardingCore.Test3x.Domain.Entities;
using ShardingCore.Test3x.Shardings.PaginationConfigs;
using ShardingCore.VirtualRoutes.Days;

namespace ShardingCore.Test3x.Shardings
{
    public class LogDayVirtualTableRoute:AbstractSimpleShardingDayKeyDateTimeVirtualTableRoute<LogDay>
    {
        protected override bool EnableHintRoute => true;

        public override DateTime GetBeginTime()
        {
            return new DateTime(2021, 1, 1);
        }

        public override void Configure(EntityMetadataTableBuilder<LogDay> builder)
        {
            builder.ShardingProperty(o => o.LogTime);
            builder.TableSeparator(string.Empty);
        }

        public override IPaginationConfiguration<LogDay> CreatePaginationConfiguration()
        {
            return new LogDayPaginationConfiguration();
        }

        public override bool AutoCreateTableByTime()
        {
            return true;
        }
    }
}
