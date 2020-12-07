namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OBSOLETE Opportunity ")]
    public partial class OBSOLETE_Opportunity_
    {
        public int ID { get; set; }

        public DateTime AddedAt { get; set; }

        public DateTime LastActivityAt { get; set; }

        [Required]
        [StringLength(150)]
        public string Company { get; set; }

        [Required]
        [StringLength(400)]
        public string Location { get; set; }

        public int? ContactId { get; set; }

        [Required]
        [StringLength(4000)]
        public string Description { get; set; }

        public int RateAsked { get; set; }

        [Required]
        [StringLength(150)]
        public string Start { get; set; }

        [Required]
        [StringLength(150)]
        public string Term { get; set; }

        public virtual OBSOLETE_Contact___Use_EMail OBSOLETE_Contact___Use_EMail { get; set; }
    }
}
