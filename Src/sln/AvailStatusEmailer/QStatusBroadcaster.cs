using AAV.WPF.Ext;
using Db.QStats.DbModel;
//using AvailStatusEmailer.Properties;
using Emailing;
using OutlookToDbWpfApp;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AvailStatusEmailer
{
  public static class QStatusBroadcaster
  {
    static readonly DateTime _batchNow = AvailStatusEmailer.App.Now;
    public const string Asu = "Availability Schedule Update - ";

    public static async Task<bool> SendLetter_UpdateDb(bool isAvailable, string emailAdrs, string firstName)
    {
#if DEBUG_
      await Task.Delay(333);
#else
      if (await sendLetter(emailAdrs, firstName, isAvailable))
      {
        await DbActor.InsertContactHistoryItem(false, _batchNow, emailAdrs, firstName, "std", isAvailable ? "Std Available 4 CVs" : "Std Busy");
        return true;
      }
#endif

      return false;
    }

    static async Task<bool> sendLetter(string emailAddress, string firstName, bool isAvailable)
    {
      try
      {
        var html = //isResumeFeatureUpdate ?
                   //$@"C:\c\Lgc\Mail\AvailStatusEmailer\Resources\AvailabilityStatus_AvailableNow_FreshCV.htm" :
          $@"C:\c\Lgc\Mail\AvailStatusEmailer\Resources\AvailabilityStatus_{(isAvailable ? "AvailableNow" : "Unavailable")}.htm";

        var body = new StreamReader(html).ReadToEnd();
        var subj = /*isResumeFeatureUpdate*/false ? "resume feature update" : Asu + (isAvailable ? "Open for opportunities in Toronto++" : "Unavailable");

        var attachmnt = isAvailable ? new string[] {
          @"C:\c\docs\CV\ikmnet assessment - Alex Pigida - 95304315.pdf",
          @"C:\c\docs\CV\Resume - Alex Pigida - short summary.docx",
          @"C:\c\docs\CV\Resume - Alex Pigida - short summary.pdf",
          @"C:\c\docs\CV\Resume - Alex Pigida - long detailed version.docx",
          @"C:\c\docs\CV\Resume - Alex Pigida - long detailed version.pdf"
        } : new string[0];
        var avlbldate = DateTime.Today.AddDays(14);
        var monthPart = avlbldate.Day < 10 ? "early" : avlbldate.Day > 20 ? "late" : "mid";
        var startDate = $"{monthPart} {avlbldate:MMMM yyyy}";

        return await Emailer.Send(
          emailAddress,
          subj,
          body.Replace("{0}", nameCasing_Mc_only_so_far(firstName)).Replace("{1}", emailAddress).Replace("{2}", startDate),
          attachmnt, @"C:\c\Lgc\Mail\AvailStatusEmailer\Resources\AlexTiny_LinkedIn.png");//@"C:\c\Lgc\Mail\AvailStatusEmailer\Resources\MCSD Logo - Latest as of 2009.gif|C:\c\Lgc\Mail\AvailStatusEmailer\Resources\linkedIn66x16.png|C:\c\Lgc\Mail\AvailStatusEmailer\Resources\AlexTiny_LinkedIn.png");
      }
      catch (Exception ex) { ex.Pop($"{emailAddress}"); return false; }
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
        using (var db = A0DbContext.Create())
        {
          var em = db.EMails.FirstOrDefault(r => r.ID == email && r.ReSendAfter != null);
          if (em != null)
            em.ReSendAfter = null;

          await OutlookToDbWindow.CheckInsertEMailEHistAsync(db, email, firstName, "", subject, body, timeSent, "..from std broadcast send", isRcvd ? "R" : "S"); // db.EHists.Add(new EHist { EMailID = email, RecivedOrSent = isRcvd ? "R" : "S", EmailedAt = timeSent, LetterSubject = subject, LetterBody = body, Notes = "", AddedAt = AvailStatusEmailer.App.Now });

          return await db.SaveChangesAsync();
        }
      }
      catch (Exception ex) { ex.Pop(); throw; }
    }
  }
}