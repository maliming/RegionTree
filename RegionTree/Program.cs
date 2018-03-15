using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace RegionTree
{
    class Program
    {
        static void Main(string[] args)
        {

            var client = new HttpClient();

            var regionWebResponseMessage = client.GetAsync("http://www.mca.gov.cn/article/sj/tjbz/a/2018/201803131439.html")
                .Result;

            var regionResult = regionWebResponseMessage.Content.ReadAsStringAsync().Result;

            var regionCodes =  Regex.Matches(regionResult, @"<td class=xl\d{1,10}>(\d{6})</td>");
            var regionNames = Regex.Matches(regionResult, @"<td class=xl\d{1,10}>([^0-9a-zA-Z<>]+)<");

            foreach (Match code in regionCodes)
            {
                Console.WriteLine(code.Value);
            }


            Console.WriteLine("Hello World!");
        }
    }
}
