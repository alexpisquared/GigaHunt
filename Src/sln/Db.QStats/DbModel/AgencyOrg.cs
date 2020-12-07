namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AgencyOrg")]
    public partial class AgencyOrg
    {
        public int Id { get; set; }

        public DateTime AddedAt { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        public DateTime? InterviewedAt { get; set; }

        public int? CurAgentEmailId { get; set; }

        public string Note { get; set; }

        public string TextMax { get; set; }
    }
}
