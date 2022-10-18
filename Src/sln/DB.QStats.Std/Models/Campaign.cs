using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models
{
    public partial class Campaign
    {
        public Campaign()
        {
            Emails = new HashSet<Email>();
            Leads = new HashSet<Lead>();
        }

        public int Id { get; set; }
        public DateTime CampaignStart { get; set; }
        public DateTime? CampaignEnd { get; set; }
        public string? Result { get; set; }
        public string? Notes { get; set; }

        public virtual ICollection<Email> Emails { get; set; }
        public virtual ICollection<Lead> Leads { get; set; }
    }
}
