using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class CVWatchLogMap : EntityTypeConfiguration<CVWatchLog>
    {
        public CVWatchLogMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.Notes)
                .HasMaxLength(400);

            this.Property(t => t.Comment)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("CVWatchLog");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.TimeObserved).HasColumnName("TimeObserved");
            this.Property(t => t.WrkPlsExp).HasColumnName("WrkPlsExp");
            this.Property(t => t.WrkPlsDoc).HasColumnName("WrkPlsDoc");
            this.Property(t => t.WrkPlsTxt).HasColumnName("WrkPlsTxt");
            this.Property(t => t.MsnMonster).HasColumnName("MsnMonster");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.ObservedTime).HasColumnName("ObservedTime");
            this.Property(t => t.ObservedDate).HasColumnName("ObservedDate");
            this.Property(t => t.RealMonster).HasColumnName("RealMonster");
            this.Property(t => t.Comment).HasColumnName("Comment");
        }
    }
}
