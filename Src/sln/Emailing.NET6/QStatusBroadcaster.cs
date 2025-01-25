using Azure.Identity;
using Microsoft.Graph;
using MsGraphLibVer1.Ver1Only;

namespace Emailing.NET6;
public static class QStatusBroadcaster
{
  public const string Asu = "Availability Schedule Update - ";

  // [Obsolete] :why Copilot decides to mark it such?
  public static async Task<bool> SendLetter_UpdateDb(bool isAvailable, string email, string firstName)
  {
    var timestamp = DateTime.Now;

    var (success, report) = await SendLetter(email, firstName, isAvailable, timestamp);
    if (success)
    {
      _ = await DbActor.InsertContactHistoryItem(false, timestamp, timestamp, email, firstName, "asu .net 8.0 a", isAvailable ? "ASU - 4 CVs - 2023-11" : "Std Busy");
      return true;
    }

    _ = await DbActor.MarkAsNotUsable(email, report);
    return false;
  }
  public static async Task<(bool success, string report)> SendLetter(string emailAddress, string firstName, bool isAvailable, DateTime timestamp, Microsoft.Extensions.Logging.ILogger? lgr = null) // called by MVVM
  {
    try
    {
      var html = """C:\g\GigaHunt\Src\sln\AvailStatusEmailer\Assets\AvailabilityStatus_AvailableNow.htm""";//isResumeFeatureUpdate ? $"C:\g\GigaHunt\Src\sln\GigaHunt\Assets\AvailabilityStatus_AvailableNow_FreshCV.htm" : $"""C:\g\GigaHunt\Src\sln\GigaHunt\Assets\AvailabilityStatus_{(isAvailable ? "AvailableNow" : "Unavailable")}.htm """;
      var body = await File.ReadAllTextAsync(html);
      var subj = Asu + "Open for opportunities in Toronto++";

      var attachments = isAvailable ? Directory.GetFiles(@"C:\g\GigaHunt\docs\CV\AsuBroadcastPackage") : Array.Empty<string>();

      var startDate = CalculateStartDate();
      var senttDate = $"{timestamp:MMddHHmmss}";
      var clientId = new ConfigurationBuilder().AddUserSecrets<QStatusBroadcaster_>().Build()["AppConfig:MicrosoftGraphClientId"] ?? throw new InvalidOperationException("¦·MicrosoftGraphClientId is missing in configuration");

#if DEBUG__
      await GraphExplorer.ExploreGraph_SendEmailHtml(
        new MsGraphLibVer1.MyGraphDriveServiceClient(clientId).DriveClient,
        emailAddress,
        subj,
        body.Replace("{0}", nameCasing_Mc_only_so_far(firstName)).Replace("{1}", emailAddress).Replace("{2}", startDate).Replace("{3}", senttDate),
        attachments, """C:\g\GigaHunt\Src\sln\GigaHunt\Assets\AlexTiny_LinkedIn.png""");
#endif

      return await new Emailer2025(lgr).Send(
        emailAddress,
        subj,
        body.Replace("{0}", nameCasing_Mc_only_so_far(firstName)).Replace("{1}", emailAddress).Replace("{2}", startDate).Replace("{3}", senttDate),
        attachments, """C:\g\GigaHunt\Src\sln\GigaHunt\Assets\AlexTiny_LinkedIn.png""");//@"C:\g\GigaHunt\Src\sln\GigaHunt\Assets\MCSD Logo - Latest as of 2009.gif|C:\g\GigaHunt\Src\sln\GigaHunt\Assets\linkedIn66x16.png|C:\g\GigaHunt\Src\sln\GigaHunt\Assets\AlexTiny_LinkedIn.png");
    }
    catch (Exception ex) { var report = ex.Log($"{emailAddress}"); return (false, report); }
  }

  static string CalculateStartDate()
  {
    var eoContract = new DateTime(2025, 2, 28).AddDays(2);
    var avlbldate = DateTime.Today < eoContract ? eoContract : DateTime.Today.AddDays(14);
    var monthPart = avlbldate.Day < 10 ? "early" : avlbldate.Day > 20 ? "late" : "mid";
    var startDate = $"{monthPart} {avlbldate:MMMM yyyy}";
    return startDate;
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

public class QStatusBroadcaster_ { }
