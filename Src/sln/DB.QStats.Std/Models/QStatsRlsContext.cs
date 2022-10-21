using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DB.QStats.Std.Models
{
  public partial class QStatsRlsContext : DbContext
  {
    public QStatsRlsContext()
    {
    }

    public QStatsRlsContext(DbContextOptions<QStatsRlsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agency> Agencies { get; set; } = null!;
    public virtual DbSet<AgencyOrg> AgencyOrgs { get; set; } = null!;
    public virtual DbSet<Campaign> Campaigns { get; set; } = null!;
    public virtual DbSet<ContactForBroadcastView> ContactForBroadcastViews { get; set; } = null!;
    public virtual DbSet<ContactRecentActivityView> ContactRecentActivityViews { get; set; } = null!;
    public virtual DbSet<CvwatchLogOld> CvwatchLogOlds { get; set; } = null!;
    public virtual DbSet<CvwatchLogView> CvwatchLogViews { get; set; } = null!;
    public virtual DbSet<CvwatchNormal> CvwatchNormals { get; set; } = null!;
    public virtual DbSet<Ehist> Ehists { get; set; } = null!;
    public virtual DbSet<Email> Emails { get; set; } = null!;
    public virtual DbSet<Lead> Leads { get; set; } = null!;
    public virtual DbSet<LeadEmail> LeadEmails { get; set; } = null!;
    public virtual DbSet<LkuLeadStatus> LkuLeadStatuses { get; set; } = null!;
    public virtual DbSet<ObsoleteContactHistoryUseEhist> ObsoleteContactHistoryUseEhists { get; set; } = null!;
    public virtual DbSet<ObsoleteContactUseEmail> ObsoleteContactUseEmails { get; set; } = null!;
    public virtual DbSet<ObsoleteInterview> ObsoleteInterviews { get; set; } = null!;
    public virtual DbSet<ObsoleteOpportunity> ObsoleteOpportunities { get; set; } = null!;
    public virtual DbSet<OppContactFor> OppContactFors { get; set; } = null!;
    public virtual DbSet<OppContactView> OppContactViews { get; set; } = null!;
    public virtual DbSet<VEmailAvailDev> VEmailAvailDevs { get; set; } = null!;
    public virtual DbSet<VEmailAvailProd> VEmailAvailProds { get; set; } = null!;
    public virtual DbSet<VEmailUnAvlDev> VEmailUnAvlDevs { get; set; } = null!;
    public virtual DbSet<VEmailUnAvlProd> VEmailUnAvlProds { get; set; } = null!;
    public virtual DbSet<VPartiesIsentMyFreeStatusSinceLastCampaignStart> VPartiesIsentMyFreeStatusSinceLastCampaignStarts { get; set; } = null!;
    public virtual DbSet<VwAgencyRate> VwAgencyRates { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
#if DEBUG
        optionsBuilder.UseSqlServer("Server=.\\SqlExpRess;Database=QStatsDbg;Trusted_Connection=True;");
#else
        optionsBuilder.UseSqlServer("Server=.\\SqlExpRess;Database=QStatsRls;Trusted_Connection=True;");
#endif
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.UseCollation("Latin1_General_CI_AS");

      modelBuilder.Entity<Agency>(entity =>
      {
        entity.ToTable("Agency");

        entity.Property(e => e.Id)
                  .HasMaxLength(256)
                  .IsUnicode(false)
                  .HasColumnName("ID");

        entity.Property(e => e.AddedAt).HasColumnType("datetime");

        entity.Property(e => e.Address)
                  .HasMaxLength(256)
                  .IsUnicode(false);

        entity.Property(e => e.ModifiedAt).HasColumnType("datetime");

        entity.Property(e => e.Note).IsUnicode(false);
      });

      modelBuilder.Entity<AgencyOrg>(entity =>
      {
        entity.ToTable("AgencyOrg");

        entity.Property(e => e.AddedAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Address)
                  .HasMaxLength(50)
                  .IsUnicode(false);

        entity.Property(e => e.InterviewedAt).HasColumnType("datetime");

        entity.Property(e => e.Name)
                  .HasMaxLength(50)
                  .IsUnicode(false);

        entity.Property(e => e.Note).IsUnicode(false);

        entity.Property(e => e.TextMax).IsUnicode(false);
      });

      modelBuilder.Entity<Campaign>(entity =>
      {
        entity.ToTable("Campaign");

        entity.Property(e => e.CampaignEnd).HasColumnType("datetime");

        entity.Property(e => e.CampaignStart).HasColumnType("datetime");

        entity.Property(e => e.Notes).IsUnicode(false);

        entity.Property(e => e.Result)
                  .HasMaxLength(50)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<ContactForBroadcastView>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("ContactForBroadcast_View");

        entity.Property(e => e.Email)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("EMail");

        entity.Property(e => e.Fname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("FName");

        entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd()
                  .HasColumnName("ID");
      });

      modelBuilder.Entity<ContactRecentActivityView>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("Contact_RecentActivity_View");

        entity.Property(e => e.AddedAt).HasColumnType("datetime");

        entity.Property(e => e.Email)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("EMail");

        entity.Property(e => e.Fname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("FName");

        entity.Property(e => e.LastRcvd).HasColumnType("datetime");

        entity.Property(e => e.LastSent).HasColumnType("datetime");

        entity.Property(e => e.LastSubject)
                  .HasMaxLength(256)
                  .IsUnicode(false);

        entity.Property(e => e.Lname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("LName");

        entity.Property(e => e.Notes)
                  .HasMaxLength(4000)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<CvwatchLogOld>(entity =>
      {
        entity.ToTable("CVWatchLog.old");

        entity.Property(e => e.Id).HasColumnName("id");

        entity.Property(e => e.Comment)
                  .HasMaxLength(500)
                  .IsUnicode(false);

        entity.Property(e => e.Notes)
                  .HasMaxLength(400)
                  .IsUnicode(false);

        entity.Property(e => e.ObservedDate).HasColumnType("datetime");

        entity.Property(e => e.ObservedTime).HasColumnType("datetime");

        entity.Property(e => e.TimeObserved)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");
      });

      modelBuilder.Entity<CvwatchLogView>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("CVWatchLogView");

        entity.Property(e => e.From).HasColumnType("datetime");

        entity.Property(e => e.Till).HasColumnType("datetime");
      });

      modelBuilder.Entity<CvwatchNormal>(entity =>
      {
        entity.ToTable("CVWatchNormal");

        entity.Property(e => e.Id).HasColumnName("id");

        entity.Property(e => e.LastSeenAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.NewValueAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Notes)
                  .HasMaxLength(4000)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<Ehist>(entity =>
      {
        entity.ToTable("EHist");

        entity.HasIndex(e => new { e.EmailId, e.RecivedOrSent }, "EmailerViewAcceleratorIndex");

        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.AddedAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.EmailId)
                  .HasMaxLength(256)
                  .IsUnicode(false)
                  .HasColumnName("EMailID");

        entity.Property(e => e.EmailedAt).HasColumnType("datetime");

        entity.Property(e => e.LetterBody).IsUnicode(false);

        entity.Property(e => e.LetterSubject).IsUnicode(false);

        entity.Property(e => e.Notes).IsUnicode(false);

        entity.Property(e => e.RecivedOrSent)
                  .HasMaxLength(1)
                  .IsUnicode(false);

        entity.HasOne(d => d.Email)
                  .WithMany(p => p.Ehists)
                  .HasForeignKey(d => d.EmailId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_EHist_EMail");
      });

      modelBuilder.Entity<Email>(entity =>
      {
        entity.ToTable("EMail");

        entity.HasIndex(e => e.Fname, "IX_EMail");

        entity.HasIndex(e => e.Lname, "IX_EMail_1");

        entity.HasIndex(e => e.Company, "IX_EMail_2");

        entity.Property(e => e.Id)
                  .HasMaxLength(256)
                  .IsUnicode(false)
                  .HasColumnName("ID");

        entity.Property(e => e.AddedAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Company)
                  .HasMaxLength(256)
                  .IsUnicode(false);

        entity.Property(e => e.DoNotNotifyOnAvailableForCampaignId).HasColumnName("DoNotNotifyOnAvailableForCampaignID");

        entity.Property(e => e.DoNotNotifyOnOffMarketForCampaignId).HasColumnName("DoNotNotifyOnOffMarketForCampaignID");

        entity.Property(e => e.Fname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("FName");

        entity.Property(e => e.Lname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("LName");

        entity.Property(e => e.ModifiedAt).HasColumnType("datetime");

        entity.Property(e => e.Notes).IsUnicode(false);

        entity.Property(e => e.NotifyPriority).HasDefaultValueSql("((100))");

        entity.Property(e => e.PermBanReason).IsUnicode(false);

        entity.Property(e => e.Phone)
                  .HasMaxLength(100)
                  .IsUnicode(false);

        entity.Property(e => e.ReSendAfter).HasColumnType("datetime");

        entity.HasOne(d => d.CompanyNavigation)
                  .WithMany(p => p.Emails)
                  .HasForeignKey(d => d.Company)
                  .HasConstraintName("FK_EMail_Agency");

        entity.HasOne(d => d.DoNotNotifyOnOffMarketForCampaign)
                  .WithMany(p => p.Emails)
                  .HasForeignKey(d => d.DoNotNotifyOnOffMarketForCampaignId)
                  .HasConstraintName("FK_EMail_Campaign");
      });

      modelBuilder.Entity<Lead>(entity =>
      {
        entity.ToTable("Lead");

        entity.Property(e => e.AddedAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Agency)
                  .HasMaxLength(50)
                  .IsUnicode(false);

        entity.Property(e => e.AgentEmailId)
                  .HasMaxLength(256)
                  .IsUnicode(false);

        entity.Property(e => e.AgentName)
                  .HasMaxLength(50)
                  .IsUnicode(false);

        entity.Property(e => e.CampaignId).HasDefaultValueSql("((3))");

        entity.Property(e => e.InterviewedAt).HasColumnType("datetime");

        entity.Property(e => e.MarketVenue)
                  .HasMaxLength(50)
                  .IsUnicode(false);

        entity.Property(e => e.ModifiedAt).HasColumnType("datetime");

        entity.Property(e => e.Note).IsUnicode(false);

        entity.Property(e => e.NoteAlso).IsUnicode(false);

        entity.Property(e => e.OfficiallySubmittedAt).HasColumnType("datetime");

        entity.Property(e => e.OppAddress)
                  .HasMaxLength(50)
                  .IsUnicode(false);

        entity.Property(e => e.OppCompany)
                  .HasMaxLength(50)
                  .IsUnicode(false);

        entity.Property(e => e.RoleDescription)
                  .HasMaxLength(256)
                  .IsUnicode(false);

        entity.Property(e => e.RoleTitle)
                  .HasMaxLength(50)
                  .IsUnicode(false);

        entity.Property(e => e.Status)
                  .HasMaxLength(10)
                  .IsUnicode(false);

        entity.HasOne(d => d.AgentEmail)
                  .WithMany(p => p.Leads)
                  .HasForeignKey(d => d.AgentEmailId)
                  .HasConstraintName("FK_Lead_EMail");

        entity.HasOne(d => d.Campaign)
                  .WithMany(p => p.Leads)
                  .HasForeignKey(d => d.CampaignId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Lead_Campaign");

        entity.HasOne(d => d.StatusNavigation)
                  .WithMany(p => p.Leads)
                  .HasForeignKey(d => d.Status)
                  .HasConstraintName("FK_Lead_lkuLeadStatus");
      });

      modelBuilder.Entity<LeadEmail>(entity =>
      {
        entity.ToTable("LeadEmail");

        entity.Property(e => e.Id)
                  .ValueGeneratedNever()
                  .HasColumnName("ID");

        entity.Property(e => e.EmailId)
                  .HasMaxLength(256)
                  .IsUnicode(false);

        entity.Property(e => e.HourlyRate).HasColumnType("money");

        entity.HasOne(d => d.Email)
                  .WithMany(p => p.LeadEmails)
                  .HasForeignKey(d => d.EmailId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_LeadEmail_EMail");

        entity.HasOne(d => d.Lead)
                  .WithMany(p => p.LeadEmails)
                  .HasForeignKey(d => d.LeadId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_LeadEmail_Lead");
      });

      modelBuilder.Entity<LkuLeadStatus>(entity =>
      {
        entity.ToTable("lkuLeadStatus");

        entity.Property(e => e.Id)
                  .HasMaxLength(10)
                  .IsUnicode(false)
                  .HasColumnName("ID");

        entity.Property(e => e.Description)
                  .HasMaxLength(500)
                  .IsUnicode(false);

        entity.Property(e => e.Name)
                  .HasMaxLength(50)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<ObsoleteContactHistoryUseEhist>(entity =>
      {
        entity.ToTable("OBSOLETE ContactHistory - Use EHist");

        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.AddedAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Ireceived)
                  .HasColumnType("datetime")
                  .HasColumnName("IReceived");

        entity.Property(e => e.Isent)
                  .HasColumnType("datetime")
                  .HasColumnName("ISent");

        entity.Property(e => e.LetterBody).IsUnicode(false);

        entity.Property(e => e.LetterSubject).IsUnicode(false);

        entity.Property(e => e.Notes).IsUnicode(false);

        entity.HasOne(d => d.Contact)
                  .WithMany(p => p.ObsoleteContactHistoryUseEhists)
                  .HasForeignKey(d => d.ContactId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ContactHistory_Contact");
      });

      modelBuilder.Entity<ObsoleteContactUseEmail>(entity =>
      {
        entity.ToTable("OBSOLETE Contact - Use EMail");

        entity.HasIndex(e => e.Email, "IX_Contact")
                  .IsUnique();

        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.AddedAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Company)
                  .HasMaxLength(128)
                  .IsUnicode(false);

        entity.Property(e => e.Email)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("EMail");

        entity.Property(e => e.Fname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("FName");

        entity.Property(e => e.Lname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("LName");

        entity.Property(e => e.Notes)
                  .HasMaxLength(4000)
                  .IsUnicode(false);

        entity.Property(e => e.PermBanReason).IsUnicode(false);

        entity.Property(e => e.Phone)
                  .HasMaxLength(50)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<ObsoleteInterview>(entity =>
      {
        entity.ToTable("OBSOLETE Interview");

        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.ContactId).HasColumnName("ContactID");

        entity.Property(e => e.HappenedAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Notes)
                  .HasMaxLength(5000)
                  .IsUnicode(false)
                  .HasDefaultValueSql("('')");

        entity.Property(e => e.OpportunityId).HasColumnName("OpportunityID");
      });

      modelBuilder.Entity<ObsoleteOpportunity>(entity =>
      {
        entity.ToTable("OBSOLETE Opportunity ");

        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.AddedAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Company)
                  .HasMaxLength(150)
                  .IsUnicode(false);

        entity.Property(e => e.Description)
                  .HasMaxLength(4000)
                  .IsUnicode(false)
                  .HasDefaultValueSql("('')");

        entity.Property(e => e.LastActivityAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Location)
                  .HasMaxLength(400)
                  .IsUnicode(false)
                  .HasDefaultValueSql("('')");

        entity.Property(e => e.Start)
                  .HasMaxLength(150)
                  .IsUnicode(false)
                  .HasDefaultValueSql("('')");

        entity.Property(e => e.Term)
                  .HasMaxLength(150)
                  .IsUnicode(false)
                  .HasDefaultValueSql("('')");

        entity.HasOne(d => d.Contact)
                  .WithMany(p => p.ObsoleteOpportunities)
                  .HasForeignKey(d => d.ContactId)
                  .HasConstraintName("FK_Opportunity_Contact");
      });

      modelBuilder.Entity<OppContactFor>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("OppContactFor");

        entity.Property(e => e.Expr1)
                  .HasMaxLength(388)
                  .IsUnicode(false);

        entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd()
                  .HasColumnName("ID");
      });

      modelBuilder.Entity<OppContactView>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("OppContactView");

        entity.Property(e => e.AddedAt).HasColumnType("datetime");

        entity.Property(e => e.AgentCompany)
                  .HasMaxLength(388)
                  .IsUnicode(false);

        entity.Property(e => e.Company)
                  .HasMaxLength(150)
                  .IsUnicode(false);

        entity.Property(e => e.LastActivityAt).HasColumnType("datetime");

        entity.Property(e => e.Location)
                  .HasMaxLength(400)
                  .IsUnicode(false);

        entity.Property(e => e.Notes)
                  .HasMaxLength(4000)
                  .IsUnicode(false);

        entity.Property(e => e.Start)
                  .HasMaxLength(150)
                  .IsUnicode(false);

        entity.Property(e => e.Term)
                  .HasMaxLength(150)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<VEmailAvailDev>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("vEMail_Avail_Dev");

        entity.Property(e => e.AddedAt).HasColumnType("datetime");

        entity.Property(e => e.Company)
                  .HasMaxLength(128)
                  .IsUnicode(false);

        entity.Property(e => e.DoNotNotifyForCampaignId).HasColumnName("DoNotNotifyForCampaignID");

        entity.Property(e => e.Fname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("FName");

        entity.Property(e => e.Id)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("ID");

        entity.Property(e => e.LastCampaignId).HasColumnName("LastCampaignID");

        entity.Property(e => e.LastCampaignStart).HasColumnType("datetime");

        entity.Property(e => e.LastRepliedAt).HasColumnType("datetime");

        entity.Property(e => e.Lname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("LName");

        entity.Property(e => e.Notes).IsUnicode(false);

        entity.Property(e => e.PermBanReason).IsUnicode(false);

        entity.Property(e => e.Phone)
                  .HasMaxLength(50)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<VEmailAvailProd>(entity =>
      {
        //entity.HasNoKey(); //tu: ?improper? fix for "The invoked method cannot be used for the entity type 'VEmailAvailProd' because it does not have a primary key."

        entity.ToView("vEMail_Avail_Prod");

        entity.Property(e => e.AddedAt).HasColumnType("datetime");

        entity.Property(e => e.Company)
                  .HasMaxLength(256)
                  .IsUnicode(false);

        entity.Property(e => e.CurrentCampaignStart).HasColumnType("datetime");

        entity.Property(e => e.DoNotNotifyForCampaignId).HasColumnName("DoNotNotifyForCampaignID");

        entity.Property(e => e.Fname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("FName");

        entity.Property(e => e.Id)
                  .HasMaxLength(256)
                  .IsUnicode(false)
                  .HasColumnName("ID");

        entity.Property(e => e.LastCampaignId).HasColumnName("LastCampaignID");

        entity.Property(e => e.LastRepliedAt).HasColumnType("datetime");

        entity.Property(e => e.LastSentAt).HasColumnType("datetime");

        entity.Property(e => e.Lname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("LName");

        entity.Property(e => e.Notes).IsUnicode(false);

        entity.Property(e => e.PermBanReason).IsUnicode(false);

        entity.Property(e => e.Phone)
                  .HasMaxLength(100)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<VEmailUnAvlDev>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("vEMail_UnAvl_Dev");

        entity.Property(e => e.AddedAt).HasColumnType("datetime");

        entity.Property(e => e.Company)
                  .HasMaxLength(128)
                  .IsUnicode(false);

        entity.Property(e => e.DoNotNotifyForCampaignId).HasColumnName("DoNotNotifyForCampaignID");

        entity.Property(e => e.Fname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("FName");

        entity.Property(e => e.Id)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("ID");

        entity.Property(e => e.LastCampaignId).HasColumnName("LastCampaignID");

        entity.Property(e => e.LastCampaignStart).HasColumnType("datetime");

        entity.Property(e => e.LastRepliedAt).HasColumnType("datetime");

        entity.Property(e => e.Lname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("LName");

        entity.Property(e => e.NoSendsAfterCmapaignEnd)
                  .HasMaxLength(8)
                  .IsUnicode(false)
                  .HasColumnName("No sends after cmapaign end");

        entity.Property(e => e.Notes).IsUnicode(false);

        entity.Property(e => e.PermBanReason).IsUnicode(false);

        entity.Property(e => e.Phone)
                  .HasMaxLength(50)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<VEmailUnAvlProd>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("vEMail_UnAvl_Prod");

        entity.Property(e => e.AddedAt).HasColumnType("datetime");

        entity.Property(e => e.Company)
                  .HasMaxLength(128)
                  .IsUnicode(false);

        entity.Property(e => e.DoNotNotifyForCampaignId).HasColumnName("DoNotNotifyForCampaignID");

        entity.Property(e => e.Fname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("FName");

        entity.Property(e => e.Id)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("ID");

        entity.Property(e => e.LastCampaignId).HasColumnName("LastCampaignID");

        entity.Property(e => e.LastCampaignStart).HasColumnType("datetime");

        entity.Property(e => e.LastRepliedAt).HasColumnType("datetime");

        entity.Property(e => e.Lname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("LName");

        entity.Property(e => e.NoSendsAfterCmapaignEnd)
                  .HasMaxLength(8)
                  .IsUnicode(false)
                  .HasColumnName("No sends after cmapaign end");

        entity.Property(e => e.Notes).IsUnicode(false);

        entity.Property(e => e.PermBanReason).IsUnicode(false);

        entity.Property(e => e.Phone)
                  .HasMaxLength(50)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<VPartiesIsentMyFreeStatusSinceLastCampaignStart>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("vPartiesISentMyFreeStatusSinceLastCampaignStart");

        entity.Property(e => e.AddedAt).HasColumnType("datetime");

        entity.Property(e => e.Company)
                  .HasMaxLength(128)
                  .IsUnicode(false);

        entity.Property(e => e.DoNotNotifyOnOffMarketForCampaignId).HasColumnName("DoNotNotifyOnOffMarketForCampaignID");

        entity.Property(e => e.Fname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("FName");

        entity.Property(e => e.Id)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("ID");

        entity.Property(e => e.LastCampaignId).HasColumnName("LastCampaignID");

        entity.Property(e => e.Lname)
                  .HasMaxLength(128)
                  .IsUnicode(false)
                  .HasColumnName("LName");

        entity.Property(e => e.Notes).IsUnicode(false);

        entity.Property(e => e.PermBanReason).IsUnicode(false);

        entity.Property(e => e.Phone)
                  .HasMaxLength(50)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<VwAgencyRate>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("vwAgencyRate");

        entity.Property(e => e.Agency)
                  .HasMaxLength(50)
                  .IsUnicode(false);

        entity.Property(e => e.Cnt).HasColumnName("cnt");

        entity.Property(e => e.LatestInstance).HasColumnType("datetime");
      });

      OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}
