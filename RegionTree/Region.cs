﻿using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.GeneralTree;
using Newtonsoft.Json;

namespace RegionTree
{
    public class Region : FullAuditedEntity, IGeneralTree<Region, int>
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

    public class RegionDto : EntityDto, IGeneralTreeDto<RegionDto, int>
    {
        public string RegionCode { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }

        public int Level { get; set; }

        public int? ParentId { get; set; }

        public ICollection<RegionDto> Children { get; set; }
    }
}