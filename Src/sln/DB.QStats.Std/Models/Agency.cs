using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models
{
    public partial class Agency
    {
        public Agency()
        {
            Emails = new HashSet<Email>();
        }

        public string Id { get; set; } = null!;
        public int? TtlAgents { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public virtual ICollection<Email> Emails { get; set; }
    }
}
