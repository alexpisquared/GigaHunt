using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class OBSOLETE_ContactHistory___Use_EHistMap : EntityTypeConfiguration<OBSOLETE_ContactHistory___Use_EHist>
    {
        public OBSOLETE_ContactHistory___Use_EHistMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("OBSOLETE ContactHistory - Use EHist");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ContactId).HasColumnName("ContactId");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");
            this.Property(t => t.ISent).HasColumnName("ISent");
            this.Property(t => t.IReceived).HasColumnName("IReceived");
            this.Property(t => t.LetterSubject).HasColumnName("LetterSubject");
            this.Property(t => t.LetterBody).HasColumnName("LetterBody");
            this.Property(t => t.Notes).HasColumnName("Notes");

            // Relationships
            this.HasRequired(t => t.OBSOLETE_Contact___Use_EMail)
                .WithMany(t => t.OBSOLETE_ContactHistory___Use_EHist)
                .HasForeignKey(d => d.ContactId);

        }
    }
}
