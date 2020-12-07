namespace Db.QStats.DbModel
{
  using System;
  using System.Data.Entity;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;

  public partial class A0DbContext : DbContext
  {
    //public A0DbContext()
    //    : base("name=A0DbContext")
    //{
    //}

    public virtual DbSet<Agency> Agencies { get; set; }
    public virtual DbSet<AgencyOrg> AgencyOrgs { get; set; }
    public virtual DbSet<Campaign> Campaigns { get; set; }
    public virtual DbSet<CVWatchLog_old> CVWatchLog_old { get; set; }
    public virtual DbSet<CVWatchNormal> CVWatchNormals { get; set; }
    public virtual DbSet<EHist> EHists { get; set; }
    public virtual DbSet<EMail> EMails { get; set; }
    public virtual DbSet<Lead> Leads { get; set; }
    public virtual DbSet<LeadEmail> LeadEmails { get; set; }
    public virtual DbSet<lkuLeadStatu> lkuLeadStatus { get; set; }
    public virtual DbSet<OBSOLETE_Contact___Use_EMail> OBSOLETE_Contact___Use_EMail { get; set; }
    public virtual DbSet<OBSOLETE_ContactHistory___Use_EHist> OBSOLETE_ContactHistory___Use_EHist { get; set; }
    public virtual DbSet<OBSOLETE_Interview> OBSOLETE_Interviews { get; set; }
    public virtual DbSet<OBSOLETE_Opportunity_> OBSOLETE_Opportunity_ { get; set; }
    public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
    public virtual DbSet<Contact_RecentActivity_View> Contact_RecentActivity_View { get; set; }
    public virtual DbSet<ContactForBroadcast_View> ContactForBroadcast_View { get; set; }
    public virtual DbSet<OppContactFor> OppContactFors { get; set; }
    public virtual DbSet<OppContactView> OppContactViews { get; set; }
    public virtual DbSet<vEMail_Avail_Dev> vEMail_Avail_Dev { get; set; }
    public virtual DbSet<vEMail_Avail_Prod> vEMail_Avail_Prod { get; set; }
    public virtual DbSet<vEMail_UnAvl_Dev> vEMail_UnAvl_Dev { get; set; }
    public virtual DbSet<vEMail_UnAvl_Prod> vEMail_UnAvl_Prod { get; set; }
    public virtual DbSet<vPartiesISentMyFreeStatusSinceLastCampaignStart> vPartiesISentMyFreeStatusSinceLastCampaignStarts { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Agency>()
          .Property(e => e.ID)
          .IsUnicode(false);

      modelBuilder.Entity<Agency>()
          .Property(e => e.Address)
          .IsUnicode(false);

      modelBuilder.Entity<Agency>()
          .Property(e => e.Note)
          .IsUnicode(false);

      modelBuilder.Entity<Agency>()
          .HasMany(e => e.EMails)
          .WithOptional(e => e.Agency)
          .HasForeignKey(e => e.Company);

      modelBuilder.Entity<AgencyOrg>()
          .Property(e => e.Name)
          .IsUnicode(false);

      modelBuilder.Entity<AgencyOrg>()
          .Property(e => e.Address)
          .IsUnicode(false);

      modelBuilder.Entity<AgencyOrg>()
          .Property(e => e.Note)
          .IsUnicode(false);

      modelBuilder.Entity<AgencyOrg>()
          .Property(e => e.TextMax)
          .IsUnicode(false);

      modelBuilder.Entity<Campaign>()
          .Property(e => e.Result)
          .IsUnicode(false);

      modelBuilder.Entity<Campaign>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<Campaign>()
          .HasMany(e => e.EMails)
          .WithOptional(e => e.Campaign)
          .HasForeignKey(e => e.DoNotNotifyOnOffMarketForCampaignID);

      modelBuilder.Entity<Campaign>()
          .HasMany(e => e.Leads)
          .WithRequired(e => e.Campaign)
          .WillCascadeOnDelete(false);

      modelBuilder.Entity<CVWatchLog_old>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<CVWatchLog_old>()
          .Property(e => e.Comment)
          .IsUnicode(false);

      modelBuilder.Entity<CVWatchNormal>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<EHist>()
          .Property(e => e.EMailID)
          .IsUnicode(false);

      modelBuilder.Entity<EHist>()
          .Property(e => e.RecivedOrSent)
          .IsUnicode(false);

      modelBuilder.Entity<EHist>()
          .Property(e => e.LetterSubject)
          .IsUnicode(false);

      modelBuilder.Entity<EHist>()
          .Property(e => e.LetterBody)
          .IsUnicode(false);

      modelBuilder.Entity<EHist>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<EMail>()
          .Property(e => e.ID)
          .IsUnicode(false);

      modelBuilder.Entity<EMail>()
          .Property(e => e.FName)
          .IsUnicode(false);

      modelBuilder.Entity<EMail>()
          .Property(e => e.LName)
          .IsUnicode(false);

      modelBuilder.Entity<EMail>()
          .Property(e => e.Company)
          .IsUnicode(false);

      modelBuilder.Entity<EMail>()
          .Property(e => e.Phone)
          .IsUnicode(false);

      modelBuilder.Entity<EMail>()
          .Property(e => e.PermBanReason)
          .IsUnicode(false);

      modelBuilder.Entity<EMail>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<EMail>()
          .HasMany(e => e.EHists)
          .WithRequired(e => e.EMail)
          .WillCascadeOnDelete(false);

      modelBuilder.Entity<EMail>()
          .HasMany(e => e.Leads)
          .WithOptional(e => e.EMail)
          .HasForeignKey(e => e.AgentEmailId);

      modelBuilder.Entity<EMail>()
          .HasMany(e => e.LeadEmails)
          .WithRequired(e => e.EMail)
          .WillCascadeOnDelete(false);

      modelBuilder.Entity<Lead>()
          .Property(e => e.AgentEmailId)
          .IsUnicode(false);

      modelBuilder.Entity<Lead>()
          .Property(e => e.OppCompany)
          .IsUnicode(false);

      modelBuilder.Entity<Lead>()
          .Property(e => e.OppAddress)
          .IsUnicode(false);

      modelBuilder.Entity<Lead>()
          .Property(e => e.RoleTitle)
          .IsUnicode(false);

      modelBuilder.Entity<Lead>()
          .Property(e => e.RoleDescription)
          .IsUnicode(false);

      modelBuilder.Entity<Lead>()
          .Property(e => e.Agency)
          .IsUnicode(false);

      modelBuilder.Entity<Lead>()
          .Property(e => e.AgentName)
          .IsUnicode(false);

      modelBuilder.Entity<Lead>()
          .Property(e => e.MarketVenue)
          .IsUnicode(false);

      modelBuilder.Entity<Lead>()
          .Property(e => e.Note)
          .IsUnicode(false);

      modelBuilder.Entity<Lead>()
          .Property(e => e.NoteAlso)
          .IsUnicode(false);

      modelBuilder.Entity<Lead>()
          .Property(e => e.Status)
          .IsUnicode(false);

      modelBuilder.Entity<Lead>()
          .HasMany(e => e.LeadEmails)
          .WithRequired(e => e.Lead)
          .WillCascadeOnDelete(false);

      modelBuilder.Entity<LeadEmail>()
          .Property(e => e.EmailId)
          .IsUnicode(false);

      modelBuilder.Entity<LeadEmail>()
          .Property(e => e.HourlyRate)
          .HasPrecision(19, 4);

      modelBuilder.Entity<lkuLeadStatu>()
          .Property(e => e.ID)
          .IsUnicode(false);

      modelBuilder.Entity<lkuLeadStatu>()
          .Property(e => e.Name)
          .IsUnicode(false);

      modelBuilder.Entity<lkuLeadStatu>()
          .Property(e => e.Description)
          .IsUnicode(false);

      modelBuilder.Entity<lkuLeadStatu>()
          .HasMany(e => e.Leads)
          .WithOptional(e => e.lkuLeadStatu)
          .HasForeignKey(e => e.Status);

      modelBuilder.Entity<OBSOLETE_Contact___Use_EMail>()
          .Property(e => e.FName)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Contact___Use_EMail>()
          .Property(e => e.LName)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Contact___Use_EMail>()
          .Property(e => e.EMail)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Contact___Use_EMail>()
          .Property(e => e.Company)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Contact___Use_EMail>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Contact___Use_EMail>()
          .Property(e => e.Phone)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Contact___Use_EMail>()
          .Property(e => e.PermBanReason)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Contact___Use_EMail>()
          .HasMany(e => e.OBSOLETE_ContactHistory___Use_EHist)
          .WithRequired(e => e.OBSOLETE_Contact___Use_EMail)
          .HasForeignKey(e => e.ContactId)
          .WillCascadeOnDelete(false);

      modelBuilder.Entity<OBSOLETE_Contact___Use_EMail>()
          .HasMany(e => e.OBSOLETE_Opportunity_)
          .WithOptional(e => e.OBSOLETE_Contact___Use_EMail)
          .HasForeignKey(e => e.ContactId);

      modelBuilder.Entity<OBSOLETE_ContactHistory___Use_EHist>()
          .Property(e => e.LetterSubject)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_ContactHistory___Use_EHist>()
          .Property(e => e.LetterBody)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_ContactHistory___Use_EHist>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Interview>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Opportunity_>()
          .Property(e => e.Company)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Opportunity_>()
          .Property(e => e.Location)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Opportunity_>()
          .Property(e => e.Description)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Opportunity_>()
          .Property(e => e.Start)
          .IsUnicode(false);

      modelBuilder.Entity<OBSOLETE_Opportunity_>()
          .Property(e => e.Term)
          .IsUnicode(false);

      modelBuilder.Entity<Contact_RecentActivity_View>()
          .Property(e => e.FName)
          .IsUnicode(false);

      modelBuilder.Entity<Contact_RecentActivity_View>()
          .Property(e => e.LName)
          .IsUnicode(false);

      modelBuilder.Entity<Contact_RecentActivity_View>()
          .Property(e => e.EMail)
          .IsUnicode(false);

      modelBuilder.Entity<Contact_RecentActivity_View>()
          .Property(e => e.LastSubject)
          .IsUnicode(false);

      modelBuilder.Entity<Contact_RecentActivity_View>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<ContactForBroadcast_View>()
          .Property(e => e.FName)
          .IsUnicode(false);

      modelBuilder.Entity<ContactForBroadcast_View>()
          .Property(e => e.EMail)
          .IsUnicode(false);

      modelBuilder.Entity<OppContactFor>()
          .Property(e => e.Expr1)
          .IsUnicode(false);

      modelBuilder.Entity<OppContactView>()
          .Property(e => e.Company)
          .IsUnicode(false);

      modelBuilder.Entity<OppContactView>()
          .Property(e => e.Location)
          .IsUnicode(false);

      modelBuilder.Entity<OppContactView>()
          .Property(e => e.AgentCompany)
          .IsUnicode(false);

      modelBuilder.Entity<OppContactView>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<OppContactView>()
          .Property(e => e.Start)
          .IsUnicode(false);

      modelBuilder.Entity<OppContactView>()
          .Property(e => e.Term)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Dev>()
          .Property(e => e.ID)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Dev>()
          .Property(e => e.FName)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Dev>()
          .Property(e => e.LName)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Dev>()
          .Property(e => e.Company)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Dev>()
          .Property(e => e.Phone)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Dev>()
          .Property(e => e.PermBanReason)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Dev>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Prod>()
          .Property(e => e.ID)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Prod>()
          .Property(e => e.FName)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Prod>()
          .Property(e => e.LName)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Prod>()
          .Property(e => e.Company)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Prod>()
          .Property(e => e.Phone)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Prod>()
          .Property(e => e.PermBanReason)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_Avail_Prod>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Dev>()
          .Property(e => e.ID)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Dev>()
          .Property(e => e.FName)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Dev>()
          .Property(e => e.LName)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Dev>()
          .Property(e => e.Company)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Dev>()
          .Property(e => e.Phone)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Dev>()
          .Property(e => e.PermBanReason)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Dev>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Dev>()
          .Property(e => e.No_sends_after_cmapaign_end)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Prod>()
          .Property(e => e.ID)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Prod>()
          .Property(e => e.FName)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Prod>()
          .Property(e => e.LName)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Prod>()
          .Property(e => e.Company)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Prod>()
          .Property(e => e.Phone)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Prod>()
          .Property(e => e.PermBanReason)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Prod>()
          .Property(e => e.Notes)
          .IsUnicode(false);

      modelBuilder.Entity<vEMail_UnAvl_Prod>()
          .Property(e => e.No_sends_after_cmapaign_end)
          .IsUnicode(false);

      modelBuilder.Entity<vPartiesISentMyFreeStatusSinceLastCampaignStart>()
          .Property(e => e.ID)
          .IsUnicode(false);

      modelBuilder.Entity<vPartiesISentMyFreeStatusSinceLastCampaignStart>()
          .Property(e => e.FName)
          .IsUnicode(false);

      modelBuilder.Entity<vPartiesISentMyFreeStatusSinceLastCampaignStart>()
          .Property(e => e.LName)
          .IsUnicode(false);

      modelBuilder.Entity<vPartiesISentMyFreeStatusSinceLastCampaignStart>()
          .Property(e => e.Company)
          .IsUnicode(false);

      modelBuilder.Entity<vPartiesISentMyFreeStatusSinceLastCampaignStart>()
          .Property(e => e.Phone)
          .IsUnicode(false);

      modelBuilder.Entity<vPartiesISentMyFreeStatusSinceLastCampaignStart>()
          .Property(e => e.PermBanReason)
          .IsUnicode(false);

      modelBuilder.Entity<vPartiesISentMyFreeStatusSinceLastCampaignStart>()
          .Property(e => e.Notes)
          .IsUnicode(false);
    }
  }
}
