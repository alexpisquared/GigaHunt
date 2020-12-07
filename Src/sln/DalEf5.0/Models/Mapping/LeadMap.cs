using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class LeadMap : EntityTypeConfiguration<Lead>
    {
        public LeadMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.AgentEmailId)
                .HasMaxLength(128);

            this.Property(t => t.OppCompany)
                .HasMaxLength(50);

            this.Property(t => t.OppAddress)
                .HasMaxLength(50);

            this.Property(t => t.RoleTitle)
                .HasMaxLength(50);

            this.Property(t => t.RoleDescription)
                .HasMaxLength(256);

            this.Property(t => t.Agency)
                .HasMaxLength(50);

            this.Property(t => t.AgentName)
                .HasMaxLength(50);

            this.Property(t => t.MarketVenue)
                .HasMaxLength(50);

            this.Property(t => t.Status)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("Lead");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CampaignId).HasColumnName("CampaignId");
            this.Property(t => t.AgentEmailId).HasColumnName("AgentEmailId");
            this.Property(t => t.AddedAt).HasColumnName("AddedAt");
            this.Property(t => t.OppCompany).HasColumnName("OppCompany");
            this.Property(t => t.OppAddress).HasColumnName("OppAddress");
            this.Property(t => t.RoleTitle).HasColumnName("RoleTitle");
            this.Property(t => t.RoleDescription).HasColumnName("RoleDescription");
            this.Property(t => t.OfficiallySubmittedAt).HasColumnName("OfficiallySubmittedAt");
            this.Property(t => t.HourlyRate).HasColumnName("HourlyRate");
            this.Property(t => t.InterviewedAt).HasColumnName("InterviewedAt");
            this.Property(t => t.Agency).HasColumnName("Agency");
            this.Property(t => t.AgentName).HasColumnName("AgentName");
            this.Property(t => t.MarketVenue).HasColumnName("MarketVenue");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.NoteAlso).HasColumnName("NoteAlso");
            this.Property(t => t.Priority).HasColumnName("Priority");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.ModifiedAt).HasColumnName("ModifiedAt");

            // Relationships
            this.HasRequired(t => t.Campaign)
                .WithMany(t => t.Leads)
                .HasForeignKey(d => d.CampaignId);
            this.HasOptional(t => t.EMail)
                .WithMany(t => t.Leads)
                .HasForeignKey(d => d.AgentEmailId);
            this.HasOptional(t => t.lkuLeadStatu)
                .WithMany(t => t.Leads)
                .HasForeignKey(d => d.Status);

        }
    }
}
