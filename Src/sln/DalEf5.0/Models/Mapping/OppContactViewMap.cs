using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class OppContactViewMap : EntityTypeConfiguration<OppContactView>
    {
        public OppContactViewMap()
        {
            // Primary Key
            this.HasKey(t => new { t.OppId, t.AddedAt, t.LastActivityAt, t.Company, t.Location, t.RateAsked, t.ContactId, t.Notes, t.Start, t.Term });

            // Properties
            this.Property(t => t.OppId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Company)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Location)
                .IsRequired()
                .HasMaxLength(400);

            this.Property(t => t.RateAsked)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ContactId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AgentCompany)
                .HasMaxLength(388);

            this.Property(t => t.Notes)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Start)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Term)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("OppContactView");
            this.Property(t => t.OppId).HasColumnName("OppId");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");
            this.Property(t => t.LastActivityAt).HasColumnName("LastActivityAt");
            this.Property(t => t.Company).HasColumnName("Company");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.RateAsked).HasColumnName("RateAsked");
            this.Property(t => t.ContactId).HasColumnName("ContactId");
            this.Property(t => t.AgentCompany).HasColumnName("AgentCompany");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.Start).HasColumnName("Start");
            this.Property(t => t.Term).HasColumnName("Term");
        }
    }
}
