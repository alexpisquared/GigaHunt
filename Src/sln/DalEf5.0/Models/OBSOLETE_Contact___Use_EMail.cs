using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class OBSOLETE_Contact___Use_EMail
    {
        public OBSOLETE_Contact___Use_EMail()
        {
            this.OBSOLETE_ContactHistory___Use_EHist = new List<OBSOLETE_ContactHistory___Use_EHist>();
            this.OBSOLETE_Opportunity_ = new List<OBSOLETE_Opportunity_>();
        }

        public int ID { get; set; }
        public System.DateTime AddedAt { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string EMail { get; set; }
        public string Company { get; set; }
        public string Notes { get; set; }
        public string Phone { get; set; }
        public string PermBanReason { get; set; }
        public virtual ICollection<OBSOLETE_ContactHistory___Use_EHist> OBSOLETE_ContactHistory___Use_EHist { get; set; }
        public virtual ICollection<OBSOLETE_Opportunity_> OBSOLETE_Opportunity_ { get; set; }
    }
}
