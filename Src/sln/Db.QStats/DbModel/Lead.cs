namespace Db.QStats.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Lead")]
    public partial class Lead
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lead()
        {
            LeadEmails = new HashSet<LeadEmail>();
        }

        public int Id { get; set; }

        public int CampaignId { get; set; }

        [StringLength(256)]
        public string AgentEmailId { get; set; }

        public DateTime AddedAt { get; set; }

        [StringLength(50)]
        public string OppCompany { get; set; }

        [StringLength(50)]
        public string OppAddress { get; set; }

        [StringLength(50)]
        public string RoleTitle { get; set; }

        [StringLength(256)]
        public string RoleDescription { get; set; }

        public DateTime? OfficiallySubmittedAt { get; set; }

        public int? HourlyRate { get; set; }

        public DateTime? InterviewedAt { get; set; }

        [StringLength(50)]
        public string Agency { get; set; }

        [StringLength(50)]
        public string AgentName { get; set; }

        [StringLength(50)]
        public string MarketVenue { get; set; }

        public string Note { get; set; }

        public string NoteAlso { get; set; }

        public int? Priority { get; set; }

        [StringLength(10)]
        public string Status { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public virtual Campaign Campaign { get; set; }

        public virtual EMail EMail { get; set; }

        public virtual lkuLeadStatu lkuLeadStatu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LeadEmail> LeadEmails { get; set; }
    }
}
