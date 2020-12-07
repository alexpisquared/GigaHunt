using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class OBSOLETE_InterviewMap : EntityTypeConfiguration<OBSOLETE_Interview>
    {
        public OBSOLETE_InterviewMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Notes)
                .IsRequired()
                .HasMaxLength(5000);

            // Table & Column Mappings
            this.ToTable("OBSOLETE Interview");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.HappenedAt).HasColumnName("HappenedAt");
            this.Property(t => t.OpportunityID).HasColumnName("OpportunityID");
            this.Property(t => t.ContactID).HasColumnName("ContactID");
            this.Property(t => t.Notes).HasColumnName("Notes");
        }
    }
}
