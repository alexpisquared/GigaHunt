namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vEMail_Avail_Prod
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

        public int? DoNotNotifyForCampaignID { get; set; }

        public DateTime? CurrentCampaignStart { get; set; }

        public int? LastCampaignID { get; set; }

        public int? MyReplies { get; set; }

        public DateTime? LastSentAt { get; set; }

        public DateTime? LastRepliedAt { get; set; }

        public int TtlSends { get; set; }
        public int TtlRcvds { get; set; }
  }
}
