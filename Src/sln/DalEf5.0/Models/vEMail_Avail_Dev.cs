using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class vEMail_Avail_Dev
    {
        public string ID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string PermBanReason { get; set; }
        public string Notes { get; set; }
        public System.DateTime AddedAt { get; set; }
        public Nullable<int> DoNotNotifyForCampaignID { get; set; }
        public Nullable<System.DateTime> LastCampaignStart { get; set; }
        public Nullable<int> LastCampaignID { get; set; }
        public Nullable<int> MyReplies { get; set; }
        public Nullable<System.DateTime> LastRepliedAt { get; set; }
    }
}
