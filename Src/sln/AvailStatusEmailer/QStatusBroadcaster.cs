using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AAV.Sys.Ext;
using AAV.WPF.Ext;
using Db.QStats.DbModel;
using Emailing;
using OutlookToDbWpfApp;
namespace AvailStatusEmailer;
public static class QStatusBroadcaster
{
  public const string ASU = "Availability Schedule Update - ";

  public static async Task<bool> SendLetter_UpdateDb(bool isAvailable, string email, string firstName)
  {
    var timestamp = DateTime.Now;

    var (success, report) = await sendLetter(email, firstName, isAvailable, timestamp);
    if (success)
    {
      _ = await DbActor.InsertContactHistoryItem(false, timestamp, email, firstName, "std .net4", isAvailable ? "ASU - 4 CVs - 2023-11" : "Std Busy");
      return true;
    }

    _ = await DbActor.MarkAsNotUsable(email, report);
    return false;
  }

  static async Task<(bool success, string report)> sendLetter(string emailAddress, string firstName, bool isAvailable, DateTime timestamp)
  {
    try
    {
      var html = @"C:\g\GigaHunt\Src\sln\AvailStatusEmailer\Assets\AvailabilityStatus_AvailableNow.htm"; //isResumeFeatureUpdate ? $@"C:\g\GigaHunt\Src\sln\AvailStatusEmailer\Assets\AvailabilityStatus_AvailableNow_FreshCV.htm" : $@"C:\g\GigaHunt\Src\sln\AvailStatusEmailer\Assets\AvailabilityStatus_{(isAvailable ? "AvailableNow" : "Unavailable")}.htm";

      var body = new StreamReader(html).ReadToEnd();
      var subj = /*isResumeFeatureUpdate*/false ? "resume feature update" : ASU + (isAvailable ? "Open for opportunities in Toronto++" : "Unavailable");

      var attachmnt = isAvailable ? new string[] {
        @"C:\c\docs\CV\ikmnet assessment - Alex Pigida - 95304315.pdf",
        @"C:\c\docs\CV\Resume - Alex Pigida - short summary.docx",
        @"C:\c\docs\CV\Resume - Alex Pigida - short summary.pdf",
        @"C:\c\docs\CV\Resume - Alex Pigida - long version.docx",
        @"C:\c\docs\CV\Resume - Alex Pigida - long version.pdf"
      } : new string[0];
      var avlbldate = DateTime.Today < new DateTime(2022, 10, 15) ? new DateTime(2022, 11, 1) : DateTime.Today.AddDays(14);
      var monthPart = avlbldate.Day < 10 ? "early" : avlbldate.Day > 20 ? "late" : "mid";
      var startDate = $"{monthPart} {avlbldate:MMMM yyyy}";
      var senttDate = $"{timestamp:yyMMddHHmmss}";

      return await Emailer.Send(
        emailAddress,
        subj,
        body.Replace("{0}", nameCasing_Mc_only_so_far(firstName)).Replace("{1}", emailAddress).Replace("{2}", startDate).Replace("{3}", senttDate),
        attachmnt, @"C:\g\GigaHunt\Src\sln\AvailStatusEmailer\Assets\AlexTiny_LinkedIn.png");//@"C:\g\GigaHunt\Src\sln\AvailStatusEmailer\Assets\MCSD Logo - Latest as of 2009.gif|C:\g\GigaHunt\Src\sln\AvailStatusEmailer\Assets\linkedIn66x16.png|C:\g\GigaHunt\Src\sln\AvailStatusEmailer\Assets\AlexTiny_LinkedIn.png");
    }
    catch (Exception ex) { ex.Pop($"{emailAddress}"); return (false, ex.Message); }
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
  public static async Task<int> InsertContactHistoryItem(bool isRcvd, DateTime timeSent, string email, string firstName, string subject, string body)
  {
    try
    {
      using var db = A0DbContext.Create();
      var em = db.EMails.FirstOrDefault(r => r.ID == email && r.ReSendAfter != null);
      if (em != null)
        em.ReSendAfter = null;

      _ = await OutlookToDbWindow.CheckInsertEMailEHistAsync(db, email, firstName, "", subject, body, timeSent, "..from std broadcast send", isRcvd ? "R" : "S"); // db.EHists.Add(new EHist { EMailID = email, RecivedOrSent = isRcvd ? "R" : "S", EmailedAt = timeSent, LetterSubject = subject, LetterBody = body, Notes = "", AddedAt = AvailStatusEmailer.App.Now });

      return await db.SaveChangesAsync();
    }
    catch (Exception ex) { ex.Pop(); throw; }
  }
  public static async Task<int> MarkAsNotUsable(string email, string exceptionMessage)
  {
    try
    {
      using var db = A0DbContext.Create();
      var em = db.EMails.FirstOrDefault(r => r.ID == email);
      if (em != null)
        em.PermBanReason = $"Err: {exceptionMessage}   {em.PermBanReason}";

      return await db.SaveChangesAsync();
    }
    catch (Exception ex) { _ = ex.Log(); throw; }
  }
}