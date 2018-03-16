using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace RegionTree
{
    public static class StringExtensions
    {
        public static bool Is2Root(this string str)
        {
            return str.Contains("0000");
        }

        public static bool Is3Root(this string str)
        {
            return str.Contains("00") && !str.Is2Root();
        }
    }


    public class Region
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string RegionCode { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }

        public int Level { get; set; }

        [JsonIgnore]
        public Region Parent { get; set; }

        public int? ParentId { get; set; }

        public ICollection<Region> Children { get; set; }
    }

    internal class Program
    {
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

            Region lastRoot = root;
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
        }
    }
}