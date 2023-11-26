using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DB.QStats.Std.Models;

public partial class QstatsRlsContext : DbContext
{
    public QstatsRlsContext()
    {
    }

    public QstatsRlsContext(DbContextOptions<QstatsRlsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agency> Agencies { get; set; }

    public virtual DbSet<AgencyOrg> AgencyOrgs { get; set; }

    public virtual DbSet<AgentOfInterestBySentQntView> AgentOfInterestBySentQntViews { get; set; }

    public virtual DbSet<Campaign> Campaigns { get; set; }

    public virtual DbSet<ContactForBroadcastView> ContactForBroadcastViews { get; set; }

    public virtual DbSet<ContactRecentActivityView> ContactRecentActivityViews { get; set; }

    public virtual DbSet<CountryOfOrigin> CountryOfOrigins { get; set; }

    public virtual DbSet<CvwatchLogOld> CvwatchLogOlds { get; set; }

    public virtual DbSet<CvwatchLogView> CvwatchLogViews { get; set; }

    public virtual DbSet<CvwatchNormal> CvwatchNormals { get; set; }

    public virtual DbSet<Ehist> Ehists { get; set; }

    public virtual DbSet<Email> Emails { get; set; }

    public virtual DbSet<FirstnameCountryXref> FirstnameCountryXrefs { get; set; }

    public virtual DbSet<FirstnameRootObject> FirstnameRootObjects { get; set; }

    public virtual DbSet<Lead> Leads { get; set; }

    public virtual DbSet<LeadEmail> LeadEmails { get; set; }

    public virtual DbSet<LkuLeadStatus> LkuLeadStatuses { get; set; }

    public virtual DbSet<ObsoleteContactHistoryUseEhist> ObsoleteContactHistoryUseEhists { get; set; }

    public virtual DbSet<ObsoleteContactUseEmail> ObsoleteContactUseEmails { get; set; }

    public virtual DbSet<ObsoleteInterview> ObsoleteInterviews { get; set; }

    public virtual DbSet<ObsoleteOpportunity> ObsoleteOpportunities { get; set; }

    public virtual DbSet<OppContactFor> OppContactFors { get; set; }

    public virtual DbSet<OppContactView> OppContactViews { get; set; }

    public virtual DbSet<Phone> Phones { get; set; }

    public virtual DbSet<PhoneAgencyXref> PhoneAgencyXrefs { get; set; }

    public virtual DbSet<PhoneEmailXref> PhoneEmailXrefs { get; set; }

    public virtual DbSet<VEmailAvailDev> VEmailAvailDevs { get; set; }

    public virtual DbSet<VEmailAvailProd> VEmailAvailProds { get; set; }

    public virtual DbSet<VEmailIdAvailProd> VEmailIdAvailProds { get; set; }

    public virtual DbSet<VEmailUnAvlDev> VEmailUnAvlDevs { get; set; }

    public virtual DbSet<VEmailUnAvlProd> VEmailUnAvlProds { get; set; }

    public virtual DbSet<VPartiesIsentMyFreeStatusSinceLastCampaignStart> VPartiesIsentMyFreeStatusSinceLastCampaignStarts { get; set; }

    public virtual DbSet<VwAgencyRate> VwAgencyRates { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=.\\SqlExpRess;Database=QStatsRls;Trusted_Connection=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<Agency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Agency_1");

            entity.ToTable("Agency");

            entity.Property(e => e.Id)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.AddedAt).HasColumnType("datetime");
            entity.Property(e => e.Address)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.IsBroadcastee).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Note).IsUnicode(false);
        });

        modelBuilder.Entity<AgencyOrg>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Agency");

            entity.ToTable("AgencyOrg");

            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
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

        modelBuilder.Entity<AgentOfInterestBySentQntView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("AgentOfInterestBySentQntView");

            entity.Property(e => e.EmailId)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("EMailID");
            entity.Property(e => e.LastSent).HasColumnType("datetime");
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
            entity
                .HasNoKey()
                .ToView("ContactForBroadcast_View");

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
            entity
                .HasNoKey()
                .ToView("Contact_RecentActivity_View");

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

        modelBuilder.Entity<CountryOfOrigin>(entity =>
        {
            entity.HasKey(e => e.Country).HasName("PK_Country_Of_Origins");

            entity.ToTable("Country_Of_Origin");

            entity.HasIndex(e => e.FirstnameRootObjectname, "IX_Country_Of_Origins_FirstnameRootObjectname");

            entity.Property(e => e.Country)
                .HasMaxLength(6)
                .HasColumnName("country");
            entity.Property(e => e.ContinentalRegion)
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnName("continental_region");
            entity.Property(e => e.CountryName)
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnName("country_name");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FirstnameRootObjectname).HasMaxLength(32);
            entity.Property(e => e.Probability).HasColumnName("probability");
            entity.Property(e => e.StatisticalRegion)
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnName("statistical_region");
        });

        modelBuilder.Entity<CvwatchLogOld>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CVWatchLog");

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
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<CvwatchLogView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("CVWatchLogView");

            entity.Property(e => e.From).HasColumnType("datetime");
            entity.Property(e => e.Till).HasColumnType("datetime");
        });

        modelBuilder.Entity<CvwatchNormal>(entity =>
        {
            entity.ToTable("CVWatchNormal");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastSeenAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NewValueAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
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
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailId)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("EMailID");
            entity.Property(e => e.EmailedAt).HasColumnType("datetime");
            entity.Property(e => e.LetterBody).IsUnicode(false);
            entity.Property(e => e.LetterSubject).IsUnicode(false);
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.RecivedOrSent)
                .IsRequired()
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.SentOn).HasColumnType("datetime");

            entity.HasOne(d => d.Email).WithMany(p => p.Ehists)
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
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Company)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.DoNotNotifyOnAvailableForCampaignId).HasColumnName("DoNotNotifyOnAvailableForCampaignID");
            entity.Property(e => e.DoNotNotifyOnOffMarketForCampaignId).HasColumnName("DoNotNotifyOnOffMarketForCampaignID");
            entity.Property(e => e.Fname)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("FName");
            entity.Property(e => e.LastAction)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");
            entity.Property(e => e.Lname)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("LName");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.NotifyPriority).HasDefaultValue(100);
            entity.Property(e => e.PermBanReason).IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ReSendAfter)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CompanyNavigation).WithMany(p => p.Emails)
                .HasForeignKey(d => d.Company)
                .HasConstraintName("FK_EMail_Agency");

            entity.HasOne(d => d.DoNotNotifyOnOffMarketForCampaign).WithMany(p => p.Emails)
                .HasForeignKey(d => d.DoNotNotifyOnOffMarketForCampaignId)
                .HasConstraintName("FK_EMail_Campaign");
        });

        modelBuilder.Entity<FirstnameCountryXref>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_FirstnameCountryXRefs");

            entity.ToTable("FirstnameCountryXRef");

            entity.HasIndex(e => e.Country, "IX_FirstnameCountryXRefs_Country_Of_Origincountry");

            entity.HasIndex(e => e.Name, "IX_FirstnameCountryXRefs_FirstnameRootObjectname");

            entity.Property(e => e.Country)
                .IsRequired()
                .HasMaxLength(6)
                .HasColumnName("country");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnName("name");
            entity.Property(e => e.Note)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Probability).HasColumnName("probability");

            entity.HasOne(d => d.CountryNavigation).WithMany(p => p.FirstnameCountryXrefs)
                .HasForeignKey(d => d.Country)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FirstnameCountryXRefs_Country_Of_Origins_Country_Of_Origincountry");

            entity.HasOne(d => d.NameNavigation).WithMany(p => p.FirstnameCountryXrefs)
                .HasForeignKey(d => d.Name)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FirstnameCountryXRefs_FirstnameRootObjects_FirstnameRootObjectname");
        });

        modelBuilder.Entity<FirstnameRootObject>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK_FirstnameRootObjects");

            entity.ToTable("FirstnameRootObject");

            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("name");
            entity.Property(e => e.Accuracy).HasColumnName("accuracy");
            entity.Property(e => e.CountryOfOriginMapUrl)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("country_of_origin_map_url");
            entity.Property(e => e.CreditsUsed).HasColumnName("credits_used");
            entity.Property(e => e.Duration)
                .IsRequired()
                .HasMaxLength(12)
                .HasColumnName("duration");
            entity.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(12)
                .HasColumnName("gender");
            entity.Property(e => e.NameSanitized)
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnName("name_sanitized");
            entity.Property(e => e.Samples).HasColumnName("samples");
        });

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.ToTable("Lead");

            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Agency)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AgentEmailId)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.AgentName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CampaignId).HasDefaultValue(3);
            entity.Property(e => e.InterviewedAt).HasColumnType("datetime");
            entity.Property(e => e.MarketVenue)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");
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

            entity.HasOne(d => d.AgentEmail).WithMany(p => p.Leads)
                .HasForeignKey(d => d.AgentEmailId)
                .HasConstraintName("FK_Lead_EMail");

            entity.HasOne(d => d.Campaign).WithMany(p => p.Leads)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lead_Campaign");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Leads)
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
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.HourlyRate).HasColumnType("money");

            entity.HasOne(d => d.Email).WithMany(p => p.LeadEmails)
                .HasForeignKey(d => d.EmailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeadEmail_EMail");

            entity.HasOne(d => d.Lead).WithMany(p => p.LeadEmails)
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
            entity.HasKey(e => e.Id).HasName("PK_ContactHistory");

            entity.ToTable("OBSOLETE ContactHistory - Use EHist");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Ireceived)
                .HasColumnType("datetime")
                .HasColumnName("IReceived");
            entity.Property(e => e.Isent)
                .HasColumnType("datetime")
                .HasColumnName("ISent");
            entity.Property(e => e.LetterBody).IsUnicode(false);
            entity.Property(e => e.LetterSubject).IsUnicode(false);
            entity.Property(e => e.Notes).IsUnicode(false);

            entity.HasOne(d => d.Contact).WithMany(p => p.ObsoleteContactHistoryUseEhists)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContactHistory_Contact");
        });

        modelBuilder.Entity<ObsoleteContactUseEmail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Contact");

            entity.ToTable("OBSOLETE Contact - Use EMail");

            entity.HasIndex(e => e.Email, "IX_Contact").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
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
            entity.HasKey(e => e.Id).HasName("PK_Interview");

            entity.ToTable("OBSOLETE Interview");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.HappenedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes)
                .IsRequired()
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.OpportunityId).HasColumnName("OpportunityID");
        });

        modelBuilder.Entity<ObsoleteOpportunity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Opportunity");

            entity.ToTable("OBSOLETE Opportunity ");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Company)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(4000)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.LastActivityAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Start)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Term)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValue("");

            entity.HasOne(d => d.Contact).WithMany(p => p.ObsoleteOpportunities)
                .HasForeignKey(d => d.ContactId)
                .HasConstraintName("FK_Opportunity_Contact");
        });

        modelBuilder.Entity<OppContactFor>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("OppContactFor");

            entity.Property(e => e.Expr1)
                .HasMaxLength(388)
                .IsUnicode(false);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<OppContactView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("OppContactView");

            entity.Property(e => e.AddedAt).HasColumnType("datetime");
            entity.Property(e => e.AgentCompany)
                .HasMaxLength(388)
                .IsUnicode(false);
            entity.Property(e => e.Company)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.LastActivityAt).HasColumnType("datetime");
            entity.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.Notes)
                .IsRequired()
                .HasMaxLength(4000)
                .IsUnicode(false);
            entity.Property(e => e.Start)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Term)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.ToTable("Phone");

            entity.HasIndex(e => e.PhoneNumber, "IX_Phone").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes)
                .HasMaxLength(800)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.SeenFirst).HasColumnType("datetime");
            entity.Property(e => e.SeenLast).HasColumnType("datetime");
        });

        modelBuilder.Entity<PhoneAgencyXref>(entity =>
        {
            entity.ToTable("PhoneAgencyXRef");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.AgencyId)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("AgencyID");
            entity.Property(e => e.Note)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.PhoneId).HasColumnName("PhoneID");

            entity.HasOne(d => d.Agency).WithMany(p => p.PhoneAgencyXrefs)
                .HasForeignKey(d => d.AgencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhoneAgencyXRef_Agency");

            entity.HasOne(d => d.Phone).WithMany(p => p.PhoneAgencyXrefs)
                .HasForeignKey(d => d.PhoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhoneAgencyXRef_Phone");
        });

        modelBuilder.Entity<PhoneEmailXref>(entity =>
        {
            entity.ToTable("PhoneEmailXRef");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailId)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.Note)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.PhoneId).HasColumnName("PhoneID");

            entity.HasOne(d => d.Email).WithMany(p => p.PhoneEmailXrefs)
                .HasForeignKey(d => d.EmailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhoneEmailXRef_EMail");

            entity.HasOne(d => d.Phone).WithMany(p => p.PhoneEmailXrefs)
                .HasForeignKey(d => d.PhoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhoneEmailXRef_Phone");
        });

        modelBuilder.Entity<VEmailAvailDev>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vEMail_Avail_Dev");

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
                .IsRequired()
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
            entity
                //.HasNoKey() ▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄
                .ToView("vEMail_Avail_Prod");

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
                .IsRequired()
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

        modelBuilder.Entity<VEmailIdAvailProd>(entity =>
        {
            entity
                //.HasNoKey() ▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄
                .ToView("vEMailId_Avail_Prod");

            entity.Property(e => e.Id)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("ID");
        });

        modelBuilder.Entity<VEmailUnAvlDev>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vEMail_UnAvl_Dev");

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
                .IsRequired()
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
                .IsRequired()
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
            entity
                .HasNoKey()
                .ToView("vEMail_UnAvl_Prod");

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
                .IsRequired()
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
                .IsRequired()
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
            entity
                .HasNoKey()
                .ToView("vPartiesISentMyFreeStatusSinceLastCampaignStart");

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
                .IsRequired()
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
            entity
                .HasNoKey()
                .ToView("vwAgencyRate");

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
