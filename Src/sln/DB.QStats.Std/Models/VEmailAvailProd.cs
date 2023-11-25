using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class VEmailAvailProd
{
    public long RowNumberForEfId { get; set; }

    public string Id { get; set; } = null!;

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public string? Company { get; set; }

    public string? Phone { get; set; }

    public string? PermBanReason { get; set; }

    public string? Notes { get; set; }

    public DateTime AddedAt { get; set; }

    public int? DoNotNotifyForCampaignId { get; set; }

    public DateTime? CurrentCampaignStart { get; set; }

    public int? LastCampaignId { get; set; }

    public int? MyReplies { get; set; }

    public DateTime? LastSentAt { get; set; }

    public DateTime? LastRepliedAt { get; set; }

    public int? TtlSends { get; set; }

    public int? TtlRcvds { get; set; }
  public int NotifyPriority { get; set; } // :manual addition to the view on 2023-11-23.
}
