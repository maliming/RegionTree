using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.GeneralTree;
using Newtonsoft.Json;

namespace RegionTree
{
    public class Region : Entity, IGeneralTree<Region, int>
    {
        public string RegionCode { get; set; }
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }

        public int Level { get; set; }

        [JsonIgnore]
        public Region Parent { get; set; }

        public int? ParentId { get; set; }

        public ICollection<Region> Children { get; set; }
    }
}