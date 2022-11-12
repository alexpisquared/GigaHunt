using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class OppContactView
{
    public int OppId { get; set; }

    public DateTime AddedAt { get; set; }

    public DateTime LastActivityAt { get; set; }

    public string Company { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int RateAsked { get; set; }

    public int ContactId { get; set; }

    public string? AgentCompany { get; set; }

    public string Notes { get; set; } = null!;

    public string Start { get; set; } = null!;

    public string Term { get; set; } = null!;
}
