using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class vPartiesISentMyFreeStatusSinceLastCampaignStart
    {
        public string ID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string PermBanReason { get; set; }
        public string Notes { get; set; }
        public System.DateTime AddedAt { get; set; }
        public Nullable<int> DoNotNotifyOnOffMarketForCampaignID { get; set; }
        public Nullable<int> LastCampaignID { get; set; }
    }
}
