namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CVWatchNormal")]
    public partial class CVWatchNormal
    {
        public int id { get; set; }

        public DateTime NewValueAt { get; set; }

        public DateTime LastSeenAt { get; set; }

        public int? WrkPlsExp { get; set; }

        public int? WrkPlsDoc { get; set; }

        public int? WrkPlsTxt { get; set; }

        public int? MsnMonster { get; set; }

        [StringLength(4000)]
        public string Notes { get; set; }
    }
}
