using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class vEMail_Avail_DevMap : EntityTypeConfiguration<vEMail_Avail_Dev>
    {
        public vEMail_Avail_DevMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.AddedAt });

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
            this.ToTable("vEMail_Avail_Dev");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FName).HasColumnName("FName");
            this.Property(t => t.LName).HasColumnName("LName");
            this.Property(t => t.Company).HasColumnName("Company");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.PermBanReason).HasColumnName("PermBanReason");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");
            this.Property(t => t.DoNotNotifyForCampaignID).HasColumnName("DoNotNotifyForCampaignID");
            this.Property(t => t.LastCampaignStart).HasColumnName("LastCampaignStart");
            this.Property(t => t.LastCampaignID).HasColumnName("LastCampaignID");
            this.Property(t => t.MyReplies).HasColumnName("MyReplies");
            this.Property(t => t.LastRepliedAt).HasColumnName("LastRepliedAt");
        }
    }
}
