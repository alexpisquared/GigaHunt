using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class LeadEmail
    {
        public int ID { get; set; }
        public int LeadId { get; set; }
        public string EmailId { get; set; }
        public Nullable<decimal> HourlyRate { get; set; }
        public virtual EMail EMail { get; set; }
        public virtual Lead Lead { get; set; }
    }
}
