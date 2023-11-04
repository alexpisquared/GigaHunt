namespace GigaHunt;
public static class QStatusBroadcaster
{
  static readonly DateTime _batchNow = DateTime.Now;
  public const string Asu = "Availability Schedule Update - ";

  public static async Task<bool> SendLetter_UpdateDb(bool isAvailable, string emailAdrs, string firstName)
  {
    if (await sendLetter(emailAdrs, firstName, isAvailable))
    {
      _ = await DbActor.InsertContactHistoryItem(false, _batchNow, _batchNow, emailAdrs, firstName, "std", isAvailable ? "Std Available 4 CVs" : "Std Busy");
      return true;
    }

    return false;
  }

  static async Task<bool> sendLetter(string emailAddress, string firstName, bool isAvailable)
  {
    try
    {
      var html = //isResumeFeatureUpdate ?
                 //$"C:\g\GigaHunt\Src\sln\GigaHunt\Assets\AvailabilityStatus_AvailableNow_FreshCV.htm" :
        $"""C:\g\GigaHunt\Src\sln\GigaHunt\Assets\AvailabilityStatus_{(isAvailable ? "AvailableNow" : "Unavailable")}.htm """;

      var body = new StreamReader(html).ReadToEnd();
      var subj = /*isResumeFeatureUpdate*/false ? "resume feature update" : Asu + (isAvailable ? "Open for opportunities in Toronto++" : "Unavailable");

      var attachmnt = isAvailable ? new string[] {
        """C:\c\docs\CV\ikmnet assessment - Alex Pigida - 95304315.pdf""",
        """C:\c\docs\CV\Resume - Alex Pigida - short summary.docx""",
        """C:\c\docs\CV\Resume - Alex Pigida - short summary.pdf""",
        """C:\c\docs\CV\Resume - Alex Pigida - long version.docx""",
        """C:\c\docs\CV\Resume - Alex Pigida - long version.pdf"""
      } : new string[0];
      var avlbldate = DateTime.Today < new DateTime(2022, 10, 15) ? new DateTime(2022, 11, 1) : DateTime.Today.AddDays(14);
      var monthPart = avlbldate.Day < 10 ? "early" : avlbldate.Day > 20 ? "late" : "mid";
      var startDate = $"{monthPart} {avlbldate:MMMM yyyy}";

      return await Emailing.NET6.Emailer.Send(
        emailAddress,
        subj,
        body.Replace("{0}", nameCasing_Mc_only_so_far(firstName)).Replace("{1}", emailAddress).Replace("{2}", startDate),
        attachmnt, """C:\g\GigaHunt\Src\sln\GigaHunt\Assets\AlexTiny_LinkedIn.png""");//@"C:\g\GigaHunt\Src\sln\GigaHunt\Assets\MCSD Logo - Latest as of 2009.gif|C:\g\GigaHunt\Src\sln\GigaHunt\Assets\linkedIn66x16.png|C:\g\GigaHunt\Src\sln\GigaHunt\Assets\AlexTiny_LinkedIn.png");
    }
    catch (Exception ex) { ex.Log($"{emailAddress}"); return false; }
  }
  static string nameCasing_Mc_only_so_far(string fname)
  {
    var fName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(fname.ToLower()); // ALEX will be ALEX without .ToLower() (2020-12-03)

    var pos = fname.IndexOf("Mc", StringComparison.InvariantCultureIgnoreCase);
    if (pos > 0 && fname.Length > pos + 2)
      fName = fname.Replace(fname.Substring(pos, 3), fname.Substring(pos, 1).ToUpper() + fname.Substring(pos + 1, 1).ToLower() + fname.Substring(pos + 2, 1).ToUpper());

    return fName;
  }
}

public class DbActor
{
  public static async Task<int> InsertContactHistoryItem(bool isRcvd, DateTime? sentOn, DateTime timeSent, string email, string firstName, string subject, string body)
  {
    try
    {
      using var db = QstatsRlsContext.Create();
      var em = db.Emails.FirstOrDefault(r => r.Id == email && r.ReSendAfter != null);
      if (em != null)
        em.ReSendAfter = null;

      _ = await OutlookToDbWindowHelpers.CheckInsert_EMail_EHist_Async(db, email, firstName, "", subject, body, sentOn, timeSent, "..from std broadcast send", isRcvd ? "R" : "S"); 

      return await db.SaveChangesAsync();
    }
    catch (Exception ex) { ex.Log(); throw; }
  }
}