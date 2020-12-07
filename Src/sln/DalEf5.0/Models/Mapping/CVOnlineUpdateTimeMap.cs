using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class CVOnlineUpdateTimeMap : EntityTypeConfiguration<CVOnlineUpdateTime>
    {
        public CVOnlineUpdateTimeMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id, t.UpdatedAt });

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            this.ToTable("CVOnlineUpdateTime");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UpdatedAt).HasColumnName("UpdatedAt");
            this.Property(t => t.Notes).HasColumnName("Notes");
        }
    }
}
