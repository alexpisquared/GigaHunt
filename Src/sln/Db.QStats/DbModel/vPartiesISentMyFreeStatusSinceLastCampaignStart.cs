namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vPartiesISentMyFreeStatusSinceLastCampaignStart")]
    public partial class vPartiesISentMyFreeStatusSinceLastCampaignStart
    {
        [Key]
        [Column(Order = 0)]
        public string ID { get; set; }

        [StringLength(128)]
        public string FName { get; set; }

        [StringLength(128)]
        public string LName { get; set; }

        [StringLength(128)]
        public string Company { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        public string PermBanReason { get; set; }

        public string Notes { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime AddedAt { get; set; }

        public int? DoNotNotifyOnOffMarketForCampaignID { get; set; }

        public int? LastCampaignID { get; set; }
    }
}
