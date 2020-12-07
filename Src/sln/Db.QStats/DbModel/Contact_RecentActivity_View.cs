namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Contact_RecentActivity_View
    {
        [Key]
        public DateTime AddedAt { get; set; }

        [StringLength(128)]
        public string FName { get; set; }

        [StringLength(128)]
        public string LName { get; set; }

        [StringLength(128)]
        public string EMail { get; set; }

        public DateTime? LastSent { get; set; }

        public DateTime? LastRcvd { get; set; }

        [StringLength(256)]
        public string LastSubject { get; set; }

        [StringLength(4000)]
        public string Notes { get; set; }
    }
}
