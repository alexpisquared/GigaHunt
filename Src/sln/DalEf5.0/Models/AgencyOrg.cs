using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class AgencyOrg
    {
        public int Id { get; set; }
        public System.DateTime AddedAt { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> InterviewedAt { get; set; }
        public Nullable<int> CurAgentEmailId { get; set; }
        public string Note { get; set; }
        public string TextMax { get; set; }
    }
}
