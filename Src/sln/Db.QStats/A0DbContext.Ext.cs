//using AAV.Sys.AsLink;
using AAV.Sys.Helpers;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;


namespace Db.QStats.DbModel
{
  public partial class A0DbContext : DbContext
  {
    public static A0DbContext Create() => new A0DbContext();
    A0DbContext() : base(_dbName = AAV.Sys.Helpers.SqlConStrHelper.ConStr("QStats", _dbgRls, _dbLocation, "", "")) { }

    static string _dbName = "Unknown yet: use dbx once to see.";
    public static string DbNameOnly => AAV.Sys.Helpers.SqlConStrHelper.DbNameFind(_dbName);

    const string _dbgRls =
#if DEBUG
          "Dbg";
#else
          "Rls";
#endif

    const AAV.Sys.Helpers.SqlConStrHelper.DbLocation _dbLocation =
#if AZURE_IS_AFFORDABLE                             
      SqlConStrHelper.DbLocation.Azure; // need to make invoices from Ofc!!!   /// May 23, 2019: apparently, TimeTrackDb..._GP (gen.purp. DB) takes $2/day !!! ...but if it is once a month, then it is better than $.10/day. Final decision pending on either auto stop after 6 hr works its miracle.
#elif ONEDRIVE_LOCALDB                              
      SqlConStrHelper.DbLocation.Local; // need to make invoices from Ofc!!!
#else //SQL_DB_INSTANCE
    AAV.Sys.Helpers.SqlConStrHelper.DbLocation.DbIns;   // keep as a fallback for dev-t (codefirst/datafirst model gen, etc.)
#endif
  }

  public partial class EMail
  {
    [NotMapped] public int? Ttl_Sent { get; set; }
    [NotMapped] public int? Ttl_Rcvd { get; set; }
    [NotMapped] public DateTime? LastSent { get; set; }
    [NotMapped] public DateTime? LastRcvd { get; set; }
  }

  //public enum LastWindow { Menu, Leads, Agents, Broadcast, OutlookToDb };
}
