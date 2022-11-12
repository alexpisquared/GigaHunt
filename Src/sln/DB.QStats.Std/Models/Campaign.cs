using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class Campaign
{
    public int Id { get; set; }

    public DateTime CampaignStart { get; set; }

    public DateTime? CampaignEnd { get; set; }

    public string? Result { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<Email> Emails { get; } = new List<Email>();

    public virtual ICollection<Lead> Leads { get; } = new List<Lead>();
}
