namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("[CVWatchLog.old]")]
    public partial class CVWatchLog_old
    {
        public int id { get; set; }

        public DateTime TimeObserved { get; set; }

        public int? WrkPlsExp { get; set; }

        public int? WrkPlsDoc { get; set; }

        public int? WrkPlsTxt { get; set; }

        public int? MsnMonster { get; set; }

        [StringLength(400)]
        public string Notes { get; set; }

        public DateTime? ObservedTime { get; set; }

        public DateTime? ObservedDate { get; set; }

        public int? RealMonster { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }
    }
}
