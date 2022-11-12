using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class ContactRecentActivityView
{
    public DateTime AddedAt { get; set; }

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public string? Email { get; set; }

    public DateTime? LastSent { get; set; }

    public DateTime? LastRcvd { get; set; }

    public string? LastSubject { get; set; }

    public string? Notes { get; set; }
}
