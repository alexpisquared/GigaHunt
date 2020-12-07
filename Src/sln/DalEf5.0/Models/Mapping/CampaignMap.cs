using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class CampaignMap : EntityTypeConfiguration<Campaign>
    {
        public CampaignMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Result)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Campaign");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CampaignStart).HasColumnName("CampaignStart");
            this.Property(t => t.CampaignEnd).HasColumnName("CampaignEnd");
            this.Property(t => t.Result).HasColumnName("Result");
            this.Property(t => t.Notes).HasColumnName("Notes");
        }
    }
}
