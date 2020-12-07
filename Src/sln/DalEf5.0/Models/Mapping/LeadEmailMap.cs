using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class LeadEmailMap : EntityTypeConfiguration<LeadEmail>
    {
        public LeadEmailMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmailId)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("LeadEmail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.LeadId).HasColumnName("LeadId");
            this.Property(t => t.EmailId).HasColumnName("EmailId");
            this.Property(t => t.HourlyRate).HasColumnName("HourlyRate");

            // Relationships
            this.HasRequired(t => t.EMail)
                .WithMany(t => t.LeadEmails)
                .HasForeignKey(d => d.EmailId);
            this.HasRequired(t => t.Lead)
                .WithMany(t => t.LeadEmails)
                .HasForeignKey(d => d.LeadId);

        }
    }
}
