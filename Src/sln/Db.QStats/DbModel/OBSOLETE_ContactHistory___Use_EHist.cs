namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OBSOLETE ContactHistory - Use EHist")]
    public partial class OBSOLETE_ContactHistory___Use_EHist
    {
        public int ID { get; set; }

        public int ContactId { get; set; }

        public DateTime AddedAt { get; set; }

        public DateTime? ISent { get; set; }

        public DateTime? IReceived { get; set; }

        public string LetterSubject { get; set; }

        public string LetterBody { get; set; }

        public string Notes { get; set; }

        public virtual OBSOLETE_Contact___Use_EMail OBSOLETE_Contact___Use_EMail { get; set; }
    }
}
