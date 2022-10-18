using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models
{
    public partial class ObsoleteContactUseEmail
    {
        public ObsoleteContactUseEmail()
        {
            ObsoleteContactHistoryUseEhists = new HashSet<ObsoleteContactHistoryUseEhist>();
            ObsoleteOpportunities = new HashSet<ObsoleteOpportunity>();
        }

        public int Id { get; set; }
        public DateTime AddedAt { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Email { get; set; }
        public string? Company { get; set; }
        public string? Notes { get; set; }
        public string? Phone { get; set; }
        public string? PermBanReason { get; set; }

        public virtual ICollection<ObsoleteContactHistoryUseEhist> ObsoleteContactHistoryUseEhists { get; set; }
        public virtual ICollection<ObsoleteOpportunity> ObsoleteOpportunities { get; set; }
    }
}
