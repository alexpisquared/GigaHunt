using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class CVWatchNormalMap : EntityTypeConfiguration<CVWatchNormal>
    {
        public CVWatchNormalMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.Notes)
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("CVWatchNormal");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.NewValueAt).HasColumnName("NewValueAt");
            this.Property(t => t.LastSeenAt).HasColumnName("LastSeenAt");
            this.Property(t => t.WrkPlsExp).HasColumnName("WrkPlsExp");
            this.Property(t => t.WrkPlsDoc).HasColumnName("WrkPlsDoc");
            this.Property(t => t.WrkPlsTxt).HasColumnName("WrkPlsTxt");
            this.Property(t => t.MsnMonster).HasColumnName("MsnMonster");
            this.Property(t => t.Notes).HasColumnName("Notes");
        }
    }
}
