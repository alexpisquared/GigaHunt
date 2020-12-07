using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class OBSOLETE_Contact___Use_EMailMap : EntityTypeConfiguration<OBSOLETE_Contact___Use_EMail>
    {
        public OBSOLETE_Contact___Use_EMailMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.FName)
                .HasMaxLength(128);

            this.Property(t => t.LName)
                .HasMaxLength(128);

            this.Property(t => t.EMail)
                .HasMaxLength(128);

            this.Property(t => t.Company)
                .HasMaxLength(128);

            this.Property(t => t.Notes)
                .HasMaxLength(4000);

            this.Property(t => t.Phone)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("OBSOLETE Contact - Use EMail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");
            this.Property(t => t.FName).HasColumnName("FName");
            this.Property(t => t.LName).HasColumnName("LName");
            this.Property(t => t.EMail).HasColumnName("EMail");
            this.Property(t => t.Company).HasColumnName("Company");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.PermBanReason).HasColumnName("PermBanReason");
        }
    }
}
