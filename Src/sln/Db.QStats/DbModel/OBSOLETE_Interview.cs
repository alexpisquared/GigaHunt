namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OBSOLETE Interview")]
    public partial class OBSOLETE_Interview
    {
        public int ID { get; set; }

        public DateTime HappenedAt { get; set; }

        public int? OpportunityID { get; set; }

        public int? ContactID { get; set; }

        [Required]
        [StringLength(5000)]
        public string Notes { get; set; }
    }
}
