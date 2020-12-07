using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class vEMail_UnAvl_ProdMap : EntityTypeConfiguration<vEMail_UnAvl_Prod>
    {
        public vEMail_UnAvl_ProdMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.AddedAt, t.No_sends_after_cmapaign_end });

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

            this.Property(t => t.No_sends_after_cmapaign_end)
                .IsRequired()
                .HasMaxLength(8);

            // Table & Column Mappings
            this.ToTable("vEMail_UnAvl_Prod");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FName).HasColumnName("FName");
            this.Property(t => t.LName).HasColumnName("LName");
            this.Property(t => t.Company).HasColumnName("Company");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.PermBanReason).HasColumnName("PermBanReason");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");
            this.Property(t => t.DoNotNotifyForCampaignID).HasColumnName("DoNotNotifyForCampaignID");
            this.Property(t => t.LastCampaignID).HasColumnName("LastCampaignID");
            this.Property(t => t.MyReplies).HasColumnName("MyReplies");
            this.Property(t => t.LastRepliedAt).HasColumnName("LastRepliedAt");
            this.Property(t => t.No_sends_after_cmapaign_end).HasColumnName("No sends after cmapaign end");
        }
    }
}
