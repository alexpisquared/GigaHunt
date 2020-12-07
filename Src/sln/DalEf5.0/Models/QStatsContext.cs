using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DalEf5.Models.Mapping;

namespace DalEf5.Models
{
    public partial class QStatsContext : DbContext
    {
        static QStatsContext()
        {
            Database.SetInitializer<QStatsContext>(null);
        }

        public QStatsContext()
            : base("Name=QStatsContext")
        {
        }

        public DbSet<Agency> Agencies { get; set; }
        public DbSet<AgencyOrg> AgencyOrgs { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CVOnlineUpdateTime> CVOnlineUpdateTimes { get; set; }
        public DbSet<CVWatchLog> CVWatchLogs { get; set; }
        public DbSet<CVWatchNormal> CVWatchNormals { get; set; }
        public DbSet<EHist> EHists { get; set; }
        public DbSet<EMail> EMails { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadEmail> LeadEmails { get; set; }
        public DbSet<lkuLeadStatu> lkuLeadStatus { get; set; }
        public DbSet<OBSOLETE_Contact___Use_EMail> OBSOLETE_Contact___Use_EMail { get; set; }
        public DbSet<OBSOLETE_ContactHistory___Use_EHist> OBSOLETE_ContactHistory___Use_EHist { get; set; }
        public DbSet<OBSOLETE_Interview> OBSOLETE_Interviews { get; set; }
        public DbSet<OBSOLETE_Opportunity_> OBSOLETE_Opportunity_ { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<Contact_RecentActivity_View> Contact_RecentActivity_View { get; set; }
        public DbSet<ContactForBroadcast_View> ContactForBroadcast_View { get; set; }
        public DbSet<OppContactFor> OppContactFors { get; set; }
        public DbSet<OppContactView> OppContactViews { get; set; }
        public DbSet<vEMail_Avail_Dev> vEMail_Avail_Dev { get; set; }
        public DbSet<vEMail_Avail_Prod> vEMail_Avail_Prod { get; set; }
        public DbSet<vEMail_UnAvl_Dev> vEMail_UnAvl_Dev { get; set; }
        public DbSet<vEMail_UnAvl_Prod> vEMail_UnAvl_Prod { get; set; }
        public DbSet<vPartiesISentMyFreeStatusSinceLastCampaignStart> vPartiesISentMyFreeStatusSinceLastCampaignStarts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AgencyMap());
            modelBuilder.Configurations.Add(new AgencyOrgMap());
            modelBuilder.Configurations.Add(new CampaignMap());
            modelBuilder.Configurations.Add(new CVOnlineUpdateTimeMap());
            modelBuilder.Configurations.Add(new CVWatchLogMap());
            modelBuilder.Configurations.Add(new CVWatchNormalMap());
            modelBuilder.Configurations.Add(new EHistMap());
            modelBuilder.Configurations.Add(new EMailMap());
            modelBuilder.Configurations.Add(new LeadMap());
            modelBuilder.Configurations.Add(new LeadEmailMap());
            modelBuilder.Configurations.Add(new lkuLeadStatuMap());
            modelBuilder.Configurations.Add(new OBSOLETE_Contact___Use_EMailMap());
            modelBuilder.Configurations.Add(new OBSOLETE_ContactHistory___Use_EHistMap());
            modelBuilder.Configurations.Add(new OBSOLETE_InterviewMap());
            modelBuilder.Configurations.Add(new OBSOLETE_Opportunity_Map());
            modelBuilder.Configurations.Add(new sysdiagramMap());
            modelBuilder.Configurations.Add(new Contact_RecentActivity_ViewMap());
            modelBuilder.Configurations.Add(new ContactForBroadcast_ViewMap());
            modelBuilder.Configurations.Add(new OppContactForMap());
            modelBuilder.Configurations.Add(new OppContactViewMap());
            modelBuilder.Configurations.Add(new vEMail_Avail_DevMap());
            modelBuilder.Configurations.Add(new vEMail_Avail_ProdMap());
            modelBuilder.Configurations.Add(new vEMail_UnAvl_DevMap());
            modelBuilder.Configurations.Add(new vEMail_UnAvl_ProdMap());
            modelBuilder.Configurations.Add(new vPartiesISentMyFreeStatusSinceLastCampaignStartMap());
        }
    }
}
