namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OppContactView")]
    public partial class OppContactView
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OppId { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime AddedAt { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime LastActivityAt { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(150)]
        public string Company { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(400)]
        public string Location { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RateAsked { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContactId { get; set; }

        [StringLength(388)]
        public string AgentCompany { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(4000)]
        public string Notes { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(150)]
        public string Start { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(150)]
        public string Term { get; set; }
    }
}
