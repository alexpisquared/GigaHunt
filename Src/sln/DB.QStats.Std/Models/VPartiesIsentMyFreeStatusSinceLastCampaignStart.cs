using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models
{
    public partial class VPartiesIsentMyFreeStatusSinceLastCampaignStart
    {
        public string Id { get; set; } = null!;
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Company { get; set; }
        public string? Phone { get; set; }
        public string? PermBanReason { get; set; }
        public string? Notes { get; set; }
        public DateTime AddedAt { get; set; }
        public int? DoNotNotifyOnOffMarketForCampaignId { get; set; }
        public int? LastCampaignId { get; set; }
    }
}
