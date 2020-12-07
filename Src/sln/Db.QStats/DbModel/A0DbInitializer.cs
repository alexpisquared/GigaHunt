using System;
using System.Data.Entity;
using System.Diagnostics;

namespace Db.QStats.DbModel
{
  public class DBInitializer : DropCreateDatabaseIfModelChanges<A0DbContext>
  {
    public static void SetDbInitializer()
    {
      try
      {
        Database.SetInitializer(new CreateDatabaseIfNotExists<A0DbContext>());
        Database.SetInitializer(new DropCreateDatabaseIfModelChanges<A0DbContext>());
        Database.SetInitializer(new DBInitializer());
      }
      catch (Exception ex) { Debug.WriteLine(ex.Message); if (Debugger.IsAttached) Debugger.Break(); else throw; }
    }

    protected override void Seed(A0DbContext dbCtx)
    {
      try
      {
        base.Seed(dbCtx);

        var g1 = dbCtx.lkuLeadStatus.Add(new lkuLeadStatu { Name = "New1", Description = "DbIni-generated" });
        var g2 = dbCtx.lkuLeadStatus.Add(new lkuLeadStatu { Name = "New2", Description = "DbIni-generated" });

        //dbCtx.MediaUnits.Add(new MediaUnit { ID = 1, AddedAt = DateTime.Now, FileName = "abc.wav", LkuGenre = g1 });
        //dbCtx.MediaUnits.Add(new MediaUnit { ID = 2, AddedAt = DateTime.Now, FileName = "abc.wav", LkuGenre = g1 });
        //dbCtx.MediaUnits.Add(new MediaUnit { ID = 3, AddedAt = DateTime.Now, FileName = "abc.wav", LkuGenre = g2 });

        dbCtx.SaveChanges();
      }
      catch (Exception ex) { Debug.WriteLine(ex.Message); if (Debugger.IsAttached) Debugger.Break(); else throw; }
    }
  }
}
