using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class Contact_RecentActivity_ViewMap : EntityTypeConfiguration<Contact_RecentActivity_View>
    {
        public Contact_RecentActivity_ViewMap()
        {
            // Primary Key
            this.HasKey(t => t.AddedAt);

            // Properties
            this.Property(t => t.FName)
                .HasMaxLength(128);

            this.Property(t => t.LName)
                .HasMaxLength(128);

            this.Property(t => t.EMail)
                .HasMaxLength(128);

            this.Property(t => t.LastSubject)
                .HasMaxLength(256);

            this.Property(t => t.Notes)
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("Contact_RecentActivity_View");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");
            this.Property(t => t.FName).HasColumnName("FName");
            this.Property(t => t.LName).HasColumnName("LName");
            this.Property(t => t.EMail).HasColumnName("EMail");
            this.Property(t => t.LastSent).HasColumnName("LastSent");
            this.Property(t => t.LastRcvd).HasColumnName("LastRcvd");
            this.Property(t => t.LastSubject).HasColumnName("LastSubject");
            this.Property(t => t.Notes).HasColumnName("Notes");
        }
    }
}
