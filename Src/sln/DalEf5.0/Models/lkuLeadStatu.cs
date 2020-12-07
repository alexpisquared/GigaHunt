using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class lkuLeadStatu
    {
        public lkuLeadStatu()
        {
            this.Leads = new List<Lead>();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Lead> Leads { get; set; }
    }
}
