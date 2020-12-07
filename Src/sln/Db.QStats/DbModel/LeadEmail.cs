namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LeadEmail")]
    public partial class LeadEmail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int LeadId { get; set; }

        [Required]
        [StringLength(256)]
        public string EmailId { get; set; }

        [Column(TypeName = "money")]
        public decimal? HourlyRate { get; set; }

        public virtual EMail EMail { get; set; }

        public virtual Lead Lead { get; set; }
    }
}
