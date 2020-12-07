using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class OBSOLETE_Interview
    {
        public int ID { get; set; }
        public System.DateTime HappenedAt { get; set; }
        public Nullable<int> OpportunityID { get; set; }
        public Nullable<int> ContactID { get; set; }
        public string Notes { get; set; }
    }
}
