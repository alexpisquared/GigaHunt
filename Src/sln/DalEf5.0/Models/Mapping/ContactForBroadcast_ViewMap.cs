using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class ContactForBroadcast_ViewMap : EntityTypeConfiguration<ContactForBroadcast_View>
    {
        public ContactForBroadcast_ViewMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.FName)
                .HasMaxLength(128);

            this.Property(t => t.EMail)
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("ContactForBroadcast_View");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FName).HasColumnName("FName");
            this.Property(t => t.EMail).HasColumnName("EMail");
        }
    }
}
