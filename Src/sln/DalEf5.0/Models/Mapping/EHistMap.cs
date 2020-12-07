using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class EHistMap : EntityTypeConfiguration<EHist>
    {
        public EHistMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.EMailID)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.RecivedOrSent)
                .IsRequired()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("EHist");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.EMailID).HasColumnName("EMailID");
            this.Property(t => t.RecivedOrSent).HasColumnName("RecivedOrSent");
            this.Property(t => t.EmailedAt).HasColumnName("EmailedAt");
            this.Property(t => t.LetterSubject).HasColumnName("LetterSubject");
            this.Property(t => t.LetterBody).HasColumnName("LetterBody");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");

            // Relationships
            this.HasRequired(t => t.EMail)
                .WithMany(t => t.EHists)
                .HasForeignKey(d => d.EMailID);

        }
    }
}
