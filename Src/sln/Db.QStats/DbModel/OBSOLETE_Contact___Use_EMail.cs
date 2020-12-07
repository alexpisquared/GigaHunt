namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OBSOLETE Contact - Use EMail")]
    public partial class OBSOLETE_Contact___Use_EMail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OBSOLETE_Contact___Use_EMail()
        {
            OBSOLETE_ContactHistory___Use_EHist = new HashSet<OBSOLETE_ContactHistory___Use_EHist>();
            OBSOLETE_Opportunity_ = new HashSet<OBSOLETE_Opportunity_>();
        }

        public int ID { get; set; }

        public DateTime AddedAt { get; set; }

        [StringLength(128)]
        public string FName { get; set; }

        [StringLength(128)]
        public string LName { get; set; }

        [StringLength(128)]
        public string EMail { get; set; }

        [StringLength(128)]
        public string Company { get; set; }

        [StringLength(4000)]
        public string Notes { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        public string PermBanReason { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OBSOLETE_ContactHistory___Use_EHist> OBSOLETE_ContactHistory___Use_EHist { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OBSOLETE_Opportunity_> OBSOLETE_Opportunity_ { get; set; }
    }
}
