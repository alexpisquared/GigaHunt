using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class OBSOLETE_Opportunity_Map : EntityTypeConfiguration<OBSOLETE_Opportunity_>
    {
        public OBSOLETE_Opportunity_Map()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Company)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Location)
                .IsRequired()
                .HasMaxLength(400);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Start)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Term)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("OBSOLETE Opportunity ");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");
            this.Property(t => t.LastActivityAt).HasColumnName("LastActivityAt");
            this.Property(t => t.Company).HasColumnName("Company");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.ContactId).HasColumnName("ContactId");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.RateAsked).HasColumnName("RateAsked");
            this.Property(t => t.Start).HasColumnName("Start");
            this.Property(t => t.Term).HasColumnName("Term");

            // Relationships
            this.HasOptional(t => t.OBSOLETE_Contact___Use_EMail)
                .WithMany(t => t.OBSOLETE_Opportunity_)
                .HasForeignKey(d => d.ContactId);

        }
    }
}
