using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class AgentOfInterestBySentQntView
{
    public string EmailId { get; set; }

    public int? TtlSent { get; set; }

    public DateTime? LastSent { get; set; }
}
