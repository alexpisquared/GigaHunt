using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class Campaign
    {
        public Campaign()
        {
            this.EMails = new List<EMail>();
            this.Leads = new List<Lead>();
        }

        public int Id { get; set; }
        public System.DateTime CampaignStart { get; set; }
        public Nullable<System.DateTime> CampaignEnd { get; set; }
        public string Result { get; set; }
        public string Notes { get; set; }
        public virtual ICollection<EMail> EMails { get; set; }
        public virtual ICollection<Lead> Leads { get; set; }
    }
}
