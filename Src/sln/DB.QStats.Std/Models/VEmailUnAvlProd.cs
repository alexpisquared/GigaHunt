using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class VEmailUnAvlProd
{
    public string Id { get; set; } = null!;

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public string? Company { get; set; }

    public string? Phone { get; set; }

    public string? PermBanReason { get; set; }

    public string? Notes { get; set; }

    public DateTime AddedAt { get; set; }

    public int? DoNotNotifyForCampaignId { get; set; }

    public DateTime? LastCampaignStart { get; set; }

    public int? LastCampaignId { get; set; }

    public int? MyReplies { get; set; }

    public DateTime? LastRepliedAt { get; set; }

    public string NoSendsAfterCmapaignEnd { get; set; } = null!;
}
