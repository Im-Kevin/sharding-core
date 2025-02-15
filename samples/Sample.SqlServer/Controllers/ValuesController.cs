﻿using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.SqlServer.DbContexts;
using Sample.SqlServer.Domain.Entities;
using ShardingCore.Core.QueryRouteManagers.Abstractions;
using ShardingCore.Extensions;
using ShardingCore.Extensions.ShardingPageExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShardingCore;
using ShardingCore.Core.VirtualDatabase.VirtualTables;
using ShardingCore.Core.VirtualRoutes.TableRoutes;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RoutingRuleEngine;
using ShardingCore.Extensions.ShardingQueryableExtensions;
using ShardingCore.Sharding.Abstractions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ShardingCore.EFCores;
using ShardingCore.Sharding.ReadWriteConfigurations.Abstractions;

namespace Sample.SqlServer.Controllers
{
    public class STU
    {
        public string Id { get; set; }
    }
    [ApiController]
    [Route("[controller]/[action]")]
    public class ValuesController : ControllerBase
    {

        private readonly DefaultShardingDbContext _defaultTableDbContext;
        private readonly IShardingRouteManager _shardingRouteManager;
        private readonly IShardingReadWriteManager _readWriteManager;

        public ValuesController(DefaultShardingDbContext defaultTableDbContext, IShardingRouteManager shardingRouteManager,IShardingReadWriteManager readWriteManager)
        {
            _defaultTableDbContext = defaultTableDbContext;
            _ = defaultTableDbContext.Model;
            _shardingRouteManager = shardingRouteManager;
            _readWriteManager = readWriteManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get2x()
        {
            _defaultTableDbContext.ChangeTracker.HasChanges();
            //var queryable = _defaultTableDbContext.Set<SysUserMod>().Where(o=>true);

            //var tableRouteRuleEngineFactory = ShardingContainer.GetService<ITableRouteRuleEngineFactory<DefaultShardingDbContext>>();
            //var tableRouteResults = tableRouteRuleEngineFactory.Route(queryable);
            var virtualTableManager = ShardingContainer.GetService<IVirtualTableManager<DefaultShardingDbContext>>();
            var virtualTable = virtualTableManager.GetVirtualTable<SysUserMod>();

            var physicTable1s = virtualTable.RouteTo(new ShardingTableRouteConfig(shardingKeyValue: "123"));//获取值为123的所有分片

            Expression<Func<SysUserMod, bool>> where = o => o.Id == "123";
            var physicTable2s = virtualTable.RouteTo(new ShardingTableRouteConfig(predicate: where));//获取表达式o.Id == "123"的所有路由

            var allPhysicTables = virtualTable.GetAllPhysicTables();

            var virtualTableRoute = virtualTable.GetVirtualRoute();
            var allTails = virtualTableRoute.GetAllTails();
            Console.WriteLine("------------------Get2x------------------------");
            using (var dbContext =
                   DbContextHelper.CreateDbContextByString(
                       "Data Source=localhost;Initial Catalog=ShardingCoreDBXA;Integrated Security=True;"))
            {
                await dbContext.AddAsync(new SysUserMod()
                {
                    Id = Guid.NewGuid().ToString("n"),
                    Age = 9,
                    AgeGroup = 10,
                    Name = "SysUserModTest"
                });
                await dbContext.SaveChangesAsync();
            }
            Console.WriteLine("------------------Get2x------------------------");
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //var sql = from ut in _defaultTableDbContext.Set<SysTest>()
            //          join u in _defaultTableDbContext.Set<SysUserMod>()
            //              on ut.UserId equals u.Id
            //          select new
            //          {
            //              ut.Id,
            //              userId = u.Id
            //          };
            //var listAsync = await sql.ToListAsync();
            //var resultx112331tt = await _defaultTableDbContext.Set<SysTest>().AsNoTracking().CountAsync();
           
            var resultx112331tt2 = await _defaultTableDbContext.Set<SysTest>().FirstOrDefaultAsync(o => o.Id == "2");
            var resultx112331ttaa2 = await _defaultTableDbContext.Set<SysTest>().FirstOrDefaultAsync(o => o.Id == "2");
            resultx112331ttaa2.UserId = "zzzz";
            var resultx112331tt2xx = await _defaultTableDbContext.Set<SysTest>().Where(o => o.Id == "2").FirstOrDefaultAsync();
            resultx112331tt2xx.UserId = "xxxxx";
            var resultx112331 = await _defaultTableDbContext.Set<SysUserMod>().CountAsync();
            var resultx11233411 = _defaultTableDbContext.Set<SysUserMod>().Count();
            var resultx11231xa = await _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Age == 198).Select(o => o.Id).ContainsAsync("198");
            var resultx11231 = await _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Age == 198198).Select(o => o.Id).ContainsAsync("1981");
            var resultx1121 = await _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Id == "198").SumAsync(o => o.Age);
            var resultx111 = await _defaultTableDbContext.Set<SysUserMod>().FirstOrDefaultAsync(o => o.Id == "198");
            var resultx2 = await _defaultTableDbContext.Set<SysUserMod>().CountAsync(o => o.Age <= 10);
            var resultx = await _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Id == "198").FirstOrDefaultAsync();
            var resultx33 = await _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Id == "198").Select(o => o.Id).FirstOrDefaultAsync();
            var resulxxt = await _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Id == "198").ToListAsync();
            var result = await _defaultTableDbContext.Set<SysUserMod>().ToListAsync();




            var sresultx11231 = _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Age == 198198).Select(o => o.Id).Contains("1981");
            var sresultx1121 = _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Id == "198").Sum(o => o.Age);
            var sresultx111 = _defaultTableDbContext.Set<SysUserMod>().FirstOrDefault(o => o.Id == "198");
            var sresultx2 = _defaultTableDbContext.Set<SysUserMod>().Count(o => o.Age <= 10);
            var sresultx = _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Id == "198").FirstOrDefault();
            var sresultx33 = _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Id == "198").Select(o => o.Id).FirstOrDefault();
            var sresultxc = _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Id == "198").Select(o => o.Id).ToList();
            var sresultxasdc = _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Id == "198").ToList();
            var sresult = _defaultTableDbContext.Set<SysUserMod>().ToList();
            var sysUserMod98 = result.FirstOrDefault(o => o.Id == "98");
            sysUserMod98.Name = "name_update" + new Random().Next(1, 99) + "_98";
            using (var tran = _defaultTableDbContext.Database.BeginTransaction())
            {
                await _defaultTableDbContext.SaveChangesAsync();
                tran.Commit();
            }

            var stu = new STU() { Id = "198" };
            var sresultx111x = _defaultTableDbContext.Set<SysUserMod>().FirstOrDefault(o => o.Id == stu.Id);

            var pageResult = await _defaultTableDbContext.Set<SysUserMod>().Skip(10).Take(10).OrderBy(o => o.Age).ToListAsync();

            var resuxaasa = await _defaultTableDbContext.Set<SysTest>().ToListAsync();

            using (_shardingRouteManager.CreateScope())
            {
                _shardingRouteManager.Current.TryCreateOrAddMustTail<SysUserMod>("00", "01");
                //_shardingRouteManager.Current.TryCreateOrAddHintTail<SysUserMod>("00", "01");
                //_shardingRouteManager.Current.TryCreateOrAddAssertTail<SysUserMod>(new TestRouteAssert());

                var mod00s = await _defaultTableDbContext.Set<SysUserMod>().Skip(10).Take(11).ToListAsync();
            }
            //_defaultTableDbContext.RemoveRange(_defaultTableDbContext.Set<SysUserMod>());
            //await _defaultTableDbContext.SaveChangesAsync();

            var sresultx1121222 = await _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Id == "198").MaxAsync(o => o.Age);




            var unionUserIds = await _defaultTableDbContext.Set<SysUserMod>().Select(o => new UnionUserId() { UserId = o.Id })
                .Union(_defaultTableDbContext.Set<SysUserSalary>().Select(o => new UnionUserId() { UserId = o.UserId })).UseUnionAllMerge().ToListAsync();







            var unionUserIdCounts = await _defaultTableDbContext.Set<SysUserMod>().Select(o => new UnionUserId() { UserId = o.Id })
                .Union(_defaultTableDbContext.Set<SysUserSalary>().Select(o => new UnionUserId() { UserId = o.UserId })).UseUnionAllMerge().CountAsync();
            var hashSet = unionUserIds.Select(o => o.UserId).ToHashSet();
            var hashSetCount = hashSet.Count;
            var averageAsync = await _defaultTableDbContext.Set<SysUserMod>().Where(o => o.Age != 19).AverageAsync(o => o.Age);


            return Ok();
        }

        public class UnionUserId
        {
            public string UserId { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> Get1([FromQuery] int p, [FromQuery] int s)

        {
           //var queryable= _defaultTableDbContext.Set<SysUserMod>().AsNoSequence().Where(o => true);
           // var provider = queryable.Provider as EntityQueryProvider;
           // var compiler = provider.GetFieldValue("_queryCompiler") as ShardingQueryCompiler;
           // var shardingDbContext = compiler.GetFieldValue("_shardingDbContext") as IShardingDbContext;


            Stopwatch sp = new Stopwatch();
            sp.Start();
            var shardingPageResultAsync = await _defaultTableDbContext.Set<SysUserMod>()
                .AsRoute(c =>
                {
                    c.TryCreateOrAddMustTail<SysUserMod>("01");
                })
                .OrderBy(o => o.Age).ToShardingPageAsync(p, s);
            sp.Stop();
            return Ok(new
            {
                sp.ElapsedMilliseconds,
                shardingPageResultAsync
            });
        }
        [HttpGet]
        public async Task<IActionResult> Get1a([FromQuery] int p, [FromQuery] int s)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            var shardingPageResultAsync = await _defaultTableDbContext.Set<SysUserMod>().UseUnionAllMerge().OrderBy(o => o.Age).ToShardingPageAsync(p, s);
            sp.Stop();
            return Ok(new
            {
                sp.ElapsedMilliseconds,
                shardingPageResultAsync
            });
        }
        [HttpGet]
        public async Task<IActionResult> Get2a([FromQuery] int p, [FromQuery] int s)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            var shardingPageResultAsync = await _defaultTableDbContext.Set<SysUserMod>().UseConnectionMode(1).OrderBy(o => o.Age).ToShardingPageAsync(p, s);
            sp.Stop();
            return Ok(new
            {
                sp.ElapsedMilliseconds,
                shardingPageResultAsync
            });
        }
        [HttpGet]
        public async Task<IActionResult> Get2a1([FromQuery] int p, [FromQuery] int s)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            var shardingPageResultAsync = await _defaultTableDbContext.Set<SysUserMod>().UseConnectionMode(1).OrderBy(o => o.Age).ToShardingPageAsync(p, s);
            sp.Stop();
            return Ok(new
            {
                sp.ElapsedMilliseconds,
                shardingPageResultAsync
            });
        }
        [HttpGet]
        public async Task<IActionResult> Get2a2([FromQuery] int p, [FromQuery] int s)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            var shardingPageResultAsync = await _defaultTableDbContext.Set<SysUserMod>().UseConnectionMode(1).OrderBy(o => o.Age).ToShardingPageAsync(p, s);
            sp.Stop();
            return Ok(new
            {
                sp.ElapsedMilliseconds,
                shardingPageResultAsync
            });
        }
        [HttpGet]
        public async Task<IActionResult> Get2a3()
        {
            var sql = from o in _defaultTableDbContext.Set<SysUserSalary>()
                      orderby o.Id descending
                      select new
                      {
                          o.Id,
                          o.UserId,
                          o.DateOfMonth,
                          o.SalaryDecimal,
                          o.SalaryFloat
                      };
            var xx = await sql.ToListAsync();
            Console.WriteLine("Get2a3-------------");
            var sysUserMods = await _defaultTableDbContext.Set<SysUserSalary>().UseConnectionMode(1).Take(2).Select(o => new
            {
                o.Id,
                o.UserId,
                o.DateOfMonth,
                o.SalaryDecimal,
                o.SalaryFloat
            }).OrderByDescending(o => o.Id).ToListAsync();
            return Ok(sysUserMods);
        }
        [HttpGet]
        public async Task<IActionResult> Get2a4()
        {
            Console.WriteLine("Get2a4-------------");
            var sysUserMods = await _defaultTableDbContext.Set<SysUserSalary>().UseConnectionMode(1).Skip(2).Take(2).OrderByDescending(o => o.DateOfMonth).ToListAsync();
            return Ok(sysUserMods);
        }
        [HttpGet]
        public IActionResult Get2([FromQuery] int p, [FromQuery] int s)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            var shardingPageResultAsync = _defaultTableDbContext.Set<SysUserMod>().OrderBy(o => o.Age).ToShardingPage(p, s);
            sp.Stop();
            return Ok(new
            {
                sp.ElapsedMilliseconds,
                shardingPageResultAsync
            });
        }
        [HttpGet]
        public IActionResult Get3()
        {

            //var dbContext2s = _defaultTableDbContext.BulkShardingExpression<SysUserMod>(o => o.Age > 100);
            //using (var tran = _defaultTableDbContext.Database.BeginTransaction())
            //{
            //    dbContext2s.ForEach(dbContext =>
            //    {
            //        dbContext.Set<SysUserMod>().Where(o => o.Age > 100).Update(o => new SysUserMod()
            //        {
            //            AgeGroup = 1000
            //        });
            //    });
            //    _defaultTableDbContext.SaveChanges();
            //    tran.Commit();
            //}
            var list = new List<SysUserMod>();
            for (int i = 0; i < 100; i++)
            {
                list.Add(new SysUserMod()
                {
                    Id = i.ToString(),
                    Name = i.ToString(),
                    Age = i,
                    AgeGroup = i
                });
            }

            using (var tran = _defaultTableDbContext.Database.BeginTransaction())
            {
                var dbContexts = _defaultTableDbContext.BulkShardingTableEnumerable(list);
                foreach (var kv in dbContexts)
                {
                    kv.Key.BulkInsert(kv.Value.ToList());
                }
                var a = 0;
                var b = 1 / a;
                tran.Commit();
            }



            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get4()
        {
            var xxxaaa = await _defaultTableDbContext.Set<SysUserSalary>().FirstOrDefaultAsync();

            Console.WriteLine("----0----");
            var xxx = await _defaultTableDbContext.Set<SysUserSalary>().IgnoreQueryFilters().OrderByDescending(o => o.DateOfMonth).FirstOrDefaultAsync();
            Console.WriteLine("----1----");
            var xxx1 = await _defaultTableDbContext.Set<SysUserSalary>().OrderByDescending(o => o.DateOfMonth).LastOrDefaultAsync();
            Console.WriteLine("----2----");
            var xxx11 = await _defaultTableDbContext.Set<SysUserSalary>().OrderByDescending(o => o.DateOfMonth).FirstAsync();
            Console.WriteLine("----3----");
            var xxx21 = await _defaultTableDbContext.Set<SysUserSalary>().OrderByDescending(o => o.DateOfMonth).LastAsync();
            Console.WriteLine("----4----");

            await _defaultTableDbContext.Set<SysUserSalary>().MaxAsync(o => o.DateOfMonth);
            await _defaultTableDbContext.Set<SysUserSalary>().MinAsync(o => o.DateOfMonth);
            return Ok(new { xxx, xxx1 });
        }

        [HttpGet]
        public async Task<IActionResult> Get5(string readNodeName)
        {
            using (_readWriteManager.CreateScope<DefaultShardingDbContext>())
            {
                _readWriteManager.GetCurrent<DefaultShardingDbContext>().SetReadWriteSeparation(100,true);

                _readWriteManager.GetCurrent<DefaultShardingDbContext>().AddDataSourceReadNode("A", readNodeName);
                var xxxaaa = await _defaultTableDbContext.Set<SysUserSalary>().FirstOrDefaultAsync();

            }
            return Ok();
        }

    }
}
