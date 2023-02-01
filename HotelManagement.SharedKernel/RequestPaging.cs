﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.SharedKernel
{
    public class PaginationRequest
    {
        public string Sort { get; set; } = "-LastModifiedOnDate";
        public string Fields { get; set; }

        [Range(1, int.MaxValue)] public int? Page { get; set; } = 1;

        [Range(1, int.MaxValue)] public int? Size { get; set; } = 20;

        public string Filter { get; set; } = "{}";
        public string FullTextSearch { get; set; }
        public string Name { get; set; }
        public Guid? Id { get; set; }
        public List<Guid> ListId { get; set; }
        public Guid? ApplicationId { get; set; }
        public bool SearchAllApp { get; set; } = true;
    }
}
