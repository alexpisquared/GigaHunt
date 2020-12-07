namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EMail")]
    public partial class EMail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EMail()
        {
            EHists = new HashSet<EHist>();
            Leads = new HashSet<Lead>();
            LeadEmails = new HashSet<LeadEmail>();
        }

        [StringLength(256)]
        public string ID { get; set; }

        [StringLength(128)]
        public string FName { get; set; }

        [StringLength(128)]
        public string LName { get; set; }

        [StringLength(256)]
        public string Company { get; set; }

        [StringLength(100)]
        public string Phone { get; set; }

        public string PermBanReason { get; set; }

        public int? DoNotNotifyOnAvailableForCampaignID { get; set; }

        public int? DoNotNotifyOnOffMarketForCampaignID { get; set; }

        public string Notes { get; set; }

        public int NotifyPriority { get; set; }

        public DateTime? ReSendAfter { get; set; }

        public DateTime AddedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public virtual Agency Agency { get; set; }

        public virtual Campaign Campaign { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EHist> EHists { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead> Leads { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LeadEmail> LeadEmails { get; set; }
    }
}
