using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class EMail
    {
        public EMail()
        {
            this.EHists = new List<EHist>();
            this.Leads = new List<Lead>();
            this.LeadEmails = new List<LeadEmail>();
        }

        public string ID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string PermBanReason { get; set; }
        public Nullable<int> DoNotNotifyOnAvailableForCampaignID { get; set; }
        public Nullable<int> DoNotNotifyOnOffMarketForCampaignID { get; set; }
        public string Notes { get; set; }
        public int NotifyPriority { get; set; }
        public Nullable<System.DateTime> ReSendAfter { get; set; }
        public System.DateTime AddedAt { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
        public virtual Agency Agency { get; set; }
        public virtual Campaign Campaign { get; set; }
        public virtual ICollection<EHist> EHists { get; set; }
        public virtual ICollection<Lead> Leads { get; set; }
        public virtual ICollection<LeadEmail> LeadEmails { get; set; }
    }
}
