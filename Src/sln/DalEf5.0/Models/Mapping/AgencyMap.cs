using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class AgencyMap : EntityTypeConfiguration<Agency>
    {
        public AgencyMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Address)
                .HasMaxLength(256);

            // Table & Column Mappings
            this.ToTable("Agency");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.TtlAgents).HasColumnName("TtlAgents");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");
            this.Property(t => t.ModifiedAt).HasColumnName("ModifiedAt");
        }
    }
}
