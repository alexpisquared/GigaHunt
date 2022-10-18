using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models
{
    public partial class Email
    {
        public Email()
        {
            Ehists = new HashSet<Ehist>();
            LeadEmails = new HashSet<LeadEmail>();
            Leads = new HashSet<Lead>();
        }

        public string Id { get; set; } = null!;
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Company { get; set; }
        public string? Phone { get; set; }
        public string? PermBanReason { get; set; }
        public int? DoNotNotifyOnAvailableForCampaignId { get; set; }
        public int? DoNotNotifyOnOffMarketForCampaignId { get; set; }
        public string? Notes { get; set; }
        public int NotifyPriority { get; set; }
        public DateTime? ReSendAfter { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public virtual Agency? CompanyNavigation { get; set; }
        public virtual Campaign? DoNotNotifyOnOffMarketForCampaign { get; set; }
        public virtual ICollection<Ehist> Ehists { get; set; }
        public virtual ICollection<LeadEmail> LeadEmails { get; set; }
        public virtual ICollection<Lead> Leads { get; set; }
    }
}
