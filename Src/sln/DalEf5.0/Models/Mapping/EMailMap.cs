using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class EMailMap : EntityTypeConfiguration<EMail>
    {
        public EMailMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.FName)
                .HasMaxLength(128);

            this.Property(t => t.LName)
                .HasMaxLength(128);

            this.Property(t => t.Company)
                .HasMaxLength(128);

            this.Property(t => t.Phone)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("EMail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FName).HasColumnName("FName");
            this.Property(t => t.LName).HasColumnName("LName");
            this.Property(t => t.Company).HasColumnName("Company");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.PermBanReason).HasColumnName("PermBanReason");
            this.Property(t => t.DoNotNotifyOnAvailableForCampaignID).HasColumnName("DoNotNotifyOnAvailableForCampaignID");
            this.Property(t => t.DoNotNotifyOnOffMarketForCampaignID).HasColumnName("DoNotNotifyOnOffMarketForCampaignID");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.NotifyPriority).HasColumnName("NotifyPriority");
            this.Property(t => t.ReSendAfter).HasColumnName("ReSendAfter");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");
            this.Property(t => t.ModifiedAt).HasColumnName("ModifiedAt");

            // Relationships
            this.HasOptional(t => t.Agency)
                .WithMany(t => t.EMails)
                .HasForeignKey(d => d.Company);
            this.HasOptional(t => t.Campaign)
                .WithMany(t => t.EMails)
                .HasForeignKey(d => d.DoNotNotifyOnOffMarketForCampaignID);

        }
    }
}
