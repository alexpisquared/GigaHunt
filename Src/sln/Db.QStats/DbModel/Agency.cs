namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Agency")]
    public partial class Agency
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Agency()
        {
            EMails = new HashSet<EMail>();
        }

        [StringLength(256)]
        public string ID { get; set; }

        public int? TtlAgents { get; set; }

        [StringLength(256)]
        public string Address { get; set; }

        public string Note { get; set; }

        public DateTime AddedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMail> EMails { get; set; }
    }
}
