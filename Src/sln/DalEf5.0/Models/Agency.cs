using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class Agency
    {
        public Agency()
        {
            this.EMails = new List<EMail>();
        }

        public string ID { get; set; }
        public Nullable<int> TtlAgents { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public System.DateTime AddedAt { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
        public virtual ICollection<EMail> EMails { get; set; }
    }
}
