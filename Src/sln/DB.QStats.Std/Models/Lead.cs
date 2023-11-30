using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class Lead
{
    public int Id { get; set; }

    public int CampaignId { get; set; }

    public string AgentEmailId { get; set; }

    public DateTime AddedAt { get; set; }

    public string OppCompany { get; set; }

    public string OppAddress { get; set; }

    public string RoleTitle { get; set; }

    public string RoleDescription { get; set; }

    public DateTime? OfficiallySubmittedAt { get; set; }

    public int? HourlyRate { get; set; }

    public double HourPerDay { get; set; } // 2023

    public DateTime? InterviewedAt { get; set; }

    public string Agency { get; set; }

    public string AgentName { get; set; }

    public string MarketVenue { get; set; }

    public string Note { get; set; }

    public string NoteAlso { get; set; }

    public int? Priority { get; set; }

    public string Status { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Email AgentEmail { get; set; }

    public virtual Campaign Campaign { get; set; }

    public virtual ICollection<LeadEmail> LeadEmails { get; set; } = new List<LeadEmail>();

    public virtual LkuLeadStatus StatusNavigation { get; set; }
}
