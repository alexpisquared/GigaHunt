﻿using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models
{
    public partial class LkuLeadStatus
    {
        public LkuLeadStatus()
        {
            Leads = new HashSet<Lead>();
        }

        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Lead> Leads { get; set; }
    }
}
