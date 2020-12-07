namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ContactForBroadcast_View
    {
        public int ID { get; set; }

        [StringLength(128)]
        public string FName { get; set; }

        [StringLength(128)]
        public string EMail { get; set; }
    }
}
