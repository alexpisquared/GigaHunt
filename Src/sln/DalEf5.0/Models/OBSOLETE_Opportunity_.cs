using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class OBSOLETE_Opportunity_
    {
        public int ID { get; set; }
        public System.DateTime AddedAt { get; set; }
        public System.DateTime LastActivityAt { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public Nullable<int> ContactId { get; set; }
        public string Description { get; set; }
        public int RateAsked { get; set; }
        public string Start { get; set; }
        public string Term { get; set; }
        public virtual OBSOLETE_Contact___Use_EMail OBSOLETE_Contact___Use_EMail { get; set; }
    }
}
