using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class OppContactView
    {
        public int OppId { get; set; }
        public System.DateTime AddedAt { get; set; }
        public System.DateTime LastActivityAt { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public int RateAsked { get; set; }
        public int ContactId { get; set; }
        public string AgentCompany { get; set; }
        public string Notes { get; set; }
        public string Start { get; set; }
        public string Term { get; set; }
    }
}
