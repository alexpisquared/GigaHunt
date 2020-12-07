using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class Lead
    {
        public Lead()
        {
            this.LeadEmails = new List<LeadEmail>();
        }

        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string AgentEmailId { get; set; }
        public System.DateTime AddedAt { get; set; }
        public string OppCompany { get; set; }
        public string OppAddress { get; set; }
        public string RoleTitle { get; set; }
        public string RoleDescription { get; set; }
        public Nullable<System.DateTime> OfficiallySubmittedAt { get; set; }
        public Nullable<int> HourlyRate { get; set; }
        public Nullable<System.DateTime> InterviewedAt { get; set; }
        public string Agency { get; set; }
        public string AgentName { get; set; }
        public string MarketVenue { get; set; }
        public string Note { get; set; }
        public string NoteAlso { get; set; }
        public Nullable<int> Priority { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
        public virtual Campaign Campaign { get; set; }
        public virtual EMail EMail { get; set; }
        public virtual lkuLeadStatu lkuLeadStatu { get; set; }
        public virtual ICollection<LeadEmail> LeadEmails { get; set; }
    }
}
