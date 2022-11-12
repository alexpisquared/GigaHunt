using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class AgencyOrg
{
    public int Id { get; set; }

    public DateTime AddedAt { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public DateTime? InterviewedAt { get; set; }

    public int? CurAgentEmailId { get; set; }

    public string? Note { get; set; }

    public string? TextMax { get; set; }
}
