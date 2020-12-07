namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EHist")]
    public partial class EHist
    {
        public int ID { get; set; }

        [Required]
        [StringLength(256)]
        public string EMailID { get; set; }

        [Required]
        [StringLength(1)]
        public string RecivedOrSent { get; set; }

        public DateTime EmailedAt { get; set; }

        public string LetterSubject { get; set; }

        public string LetterBody { get; set; }

        public string Notes { get; set; }

        public DateTime AddedAt { get; set; }

        public virtual EMail EMail { get; set; }
    }
}
