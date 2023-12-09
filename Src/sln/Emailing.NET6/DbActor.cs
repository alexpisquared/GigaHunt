namespace Emailing.NET6;

[Obsolete]
public class DbActor
{
  public static async Task<int> InsertContactHistoryItem(bool isRcvd, DateTime? sentOn, DateTime timeSent, string email, string firstName, string subject, string body)
  {
    try
    {
      using var dbq = QstatsRlsContext.Create();
      var em = dbq.Emails.FirstOrDefault(r => r.Id == email && r.ReSendAfter != null);
      if (em != null)
        em.ReSendAfter = null;

      _ = await new OutlookToDbWindowHelpers(null).CheckInsert_EMail_EHist_Async(dbq, email, firstName, "", subject, body, sentOn, timeSent, "..from std broadcast send", isRcvd ? "R" : "S");

      return await dbq.SaveChangesAsync();
    }
    catch (Exception ex) { _ = ex.Log(); throw; }
  }
  public static async Task<int> MarkAsNotUsable(string email, string exceptionMessage)
  {
    try
    {
      using var dbq = QstatsRlsContext.Create();
      var em = dbq.Emails.FirstOrDefault(r => r.Id == email);
      if (em != null)
        em.PermBanReason = $"Err: {exceptionMessage}   {em.PermBanReason}";

      return await dbq.SaveChangesAsync();
    }
    catch (Exception ex) { _ = ex.Log(); throw; }
  }
}