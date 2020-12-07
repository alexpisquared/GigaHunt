using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class AgencyOrgMap : EntityTypeConfiguration<AgencyOrg>
    {
        public AgencyOrgMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(50);

            this.Property(t => t.Address)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AgencyOrg");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.InterviewedAt).HasColumnName("InterviewedAt");
            this.Property(t => t.CurAgentEmailId).HasColumnName("CurAgentEmailId");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.TextMax).HasColumnName("TextMax");
        }
    }
}
