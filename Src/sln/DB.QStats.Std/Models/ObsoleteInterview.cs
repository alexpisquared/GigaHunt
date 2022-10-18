using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models
{
    public partial class ObsoleteInterview
    {
        public int Id { get; set; }
        public DateTime HappenedAt { get; set; }
        public int? OpportunityId { get; set; }
        public int? ContactId { get; set; }
        public string Notes { get; set; } = null!;
    }
}
