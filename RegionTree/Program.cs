using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.GeneralTree;
using Abp.Threading;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using Newtonsoft.Json;

namespace RegionTree
{
    internal class Program
    {
        public static IConfigurationRoot Configuration;

        private static void Main(string[] args)
        {
            var client = new HttpClient();

            var regionWebResponseMessage = client
                .GetAsync("http://www.mca.gov.cn/article/sj/tjbz/a/2018/201803131439.html")
                .Result;

            var regionResult = regionWebResponseMessage.Content.ReadAsStringAsync().Result;

            var regionCodes = Regex.Matches(regionResult, @"<td class=[0-9a-zA-Z]{1,10}>(\d{6})<");
            var regionNames = Regex.Matches(regionResult, @"<td class=[0-9a-zA-Z]{1,10}>([^0-9a-zA-Z<>]+)<");

            if (regionCodes.Count != regionNames.Count)
            {
                throw new ApplicationException("获取的到地区数据不一致!");
            }

            var region = new List<Region>();
            for (var i = 0; i < regionCodes.Count; i++)
            {
                var code = regionCodes[i].Groups[1].Value;
                var name = regionNames[i].Groups[1].Value;

                region.Add(new Region
                {
                    Name = name,
                    RegionCode = code
                });
            }

            var root = new Region
            {
                Name = "中国"
            };

            var lastRoot = root;
            region.ForEach(r =>
            {
                var curr = new Region
                {
                    Name = r.Name,
                    RegionCode = r.RegionCode
                };

                if (r.RegionCode.Is2Root())
                {
                    if (lastRoot.Parent == null)
                    {
                        if (lastRoot.Children == null)
                        {
                            lastRoot.Children = new List<Region>();
                        }

                        lastRoot.Children.Add(curr);
                        curr.Parent = lastRoot;
                        lastRoot = curr;
                    }
                    else
                    {
                        lastRoot = lastRoot.RegionCode.Is3Root() ? lastRoot.Parent.Parent : lastRoot.Parent;

                        curr.Parent = lastRoot;
                        if (curr.Parent.Children == null)
                        {
                            curr.Parent.Children = new List<Region>();
                        }

                        curr.Parent.Children.Add(curr);

                        lastRoot = curr;
                    }
                }
                else if (r.RegionCode.Is3Root())
                {
                    curr.Parent = lastRoot.RegionCode.Is3Root() ? lastRoot.Parent : lastRoot;
                    if (curr.Parent.Children == null)
                    {
                        curr.Parent.Children = new List<Region>();
                    }

                    curr.Parent.Children.Add(curr);

                    lastRoot = curr;
                }
                else
                {
                    curr.Parent = lastRoot;
                    if (curr.Parent.Children == null)
                    {
                        curr.Parent.Children = new List<Region>();
                    }

                    curr.Parent.Children.Add(curr);
                }
            });

            File.WriteAllText("region.json", JsonConvert.SerializeObject(root, Formatting.Indented));

            using (var abpBootstrapper = AbpBootstrapper.Create<RegionModule>())
            {
                abpBootstrapper.Initialize();

                var unitOfWorkManager = abpBootstrapper.IocManager.Resolve<IUnitOfWorkManager>();

                var dbContext = abpBootstrapper.IocManager.Resolve<RegionDbContext>();

                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                using (var uow = unitOfWorkManager.Begin())
                {
                    var regionRepository = abpBootstrapper.IocManager.Resolve<IRepository<Region, int>>();
                    var regionTreeManager = abpBootstrapper.IocManager.Resolve<GeneralTreeManager<Region, int>>();

                    root.Children.ForEach(x =>
                    {
                        x.Parent = null;
                        AsyncHelper.RunSync(() => regionTreeManager.BulkCreateAsync(x));
                        unitOfWorkManager.Current.SaveChanges();
                    });

                    unitOfWorkManager.Current.SaveChanges();

                    var regionTree = regionRepository.GetAll().ToList().ToTree<Region, int>().ToList();

                    uow.Complete();
                }
            }
        }
    }
}