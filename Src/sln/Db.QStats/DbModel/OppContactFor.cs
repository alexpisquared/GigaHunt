namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OppContactFor")]
    public partial class OppContactFor
    {
        public int ID { get; set; }

        [StringLength(388)]
        public string Expr1 { get; set; }
    }
}
