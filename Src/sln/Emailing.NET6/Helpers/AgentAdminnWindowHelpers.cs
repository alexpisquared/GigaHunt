using System.Reflection;
using System.Windows;
using GigaHunt.AsLink;

namespace GigaHunt.View;

public static class AgentAdminnWindowHelpers
{
  public static async Task<bool> CheckAskToSaveDispose_CanditdteForGlobalRepltAsync(QstatsRlsContext dbx, bool dispose, Func<Task> savePlusMetadata)
  {
    try
    {
      if (dbx.ChangeTracker.Entries().Any(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
      {
#if true
        await savePlusMetadata();
        if (dispose) dbx.Dispose();
        return false;
#else
        App.Speak("Would you like to save the changes?");
        switch (MessageBox.Show($"{dbx.GetDbChangesReport()}", "Would you like to save the changes?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
        {
          default:
          case MessageBoxResult.Yes: await savePlusMetadata(); if (dispose) dbx.Dispose(); return false;
          case MessageBoxResult.No: if (dispose) dbx.Dispose(); return false;
          case MessageBoxResult.Cancel: return true;
        }
#endif
      }
      else
      {
        Trace.WriteLine("Nothing to save!"); await Task.Delay(333);
        return false;
      }
    }
    catch (Exception ex) { ex.Log(); return true; }
  }
  public static async Task<string> SaveAndUpdateMetadata(QstatsRlsContext db)
  {
    var now = BPR___.Now;

    while (true)
    {
      try
      {
        foreach (var row in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
          if (row.Entity is Email email) // sanity check
          {
            var agencyCompany = email.Company;

            if (agencyCompany is not null && db.Agencies.FirstOrDefault(r => string.Compare(r.Id, agencyCompany, true) == 0) == null)
              _ = db.Agencies.Add(new Agency { Id = agencyCompany, AddedAt = now });

            email.AddedAt = now;
            email.NotifyPriority = 99;
          }

        foreach (var row in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
          if (row.Entity is Email email) // sanity check
            email.ModifiedAt = now;

        var (success, rowsSavedCnt, report) = await db.TrySaveReportAsync("OutlookToDb.cs");

        Trace.WriteLine(report); await Task.Delay(333);

        return report;
      }
      catch (Exception)
      {
        throw;
        ////if (bKeepShowingMoveError)
        //switch (MessageBox.Show(string.Format("Error in {0}.{1}():\n\n{2}\n\nRetry?\nYes - keep showing\nNo - skip showing\nCancel - Cancel operation", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, MethodBase.GetCurrentMethod()?.Name,
        //  ex.InnerException == null ? ex.Message :
        //  ex.InnerException.InnerException == null ? ex.InnerException.Message :
        //  ex.InnerException.InnerException.Message), "Exception ", MessageBoxButton.YesNoCancel, MessageBoxImage.Error))
        //{
        //  case MessageBoxResult.Yes: break;
        //  //	case MessageBoxResult.No: bKeepShowingMoveError = false; break;
        //  case MessageBoxResult.Cancel: return "Cancelled";// tbkTitle.Text;
        //}
      }
    }
  }
}