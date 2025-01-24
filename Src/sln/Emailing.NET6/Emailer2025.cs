using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Emailing.NET6;
public class Emailer2025
{
  static string LogFile => """C:\temp\Logs\CV.Emailed.txt""";// Path.Combine(OneDrive.Folder(@"Public\Logs"), "CV.Emailed.txt");
  const string appReg = "EmailAssistantAnyAndPersonal2_2024"; 
  static bool isFirst = true;
  readonly ILogger _lgr;
  readonly IConfiguration _configuration;
  readonly GraphServiceClient _graphClient;

  public Emailer2025(ILogger lgr)
  {
    _lgr = lgr;
    _configuration = new ConfigurationBuilder().AddUserSecrets<Emailer2025>().Build();

    var clientId = _configuration[$"{appReg}:ClientId"] ?? throw new InvalidOperationException("¦·MicrosoftGraphClientId is missing in configuration");

    _graphClient = new MsGraphLibVer1.MyGraphDriveServiceClient(clientId).DriveClient;       //new GraphServiceClient(new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions { TenantId = "consumers", ClientId = clientId, RedirectUri = new Uri("http://localhost") }), ["Mail.Send", "Mail.Send.Shared", "User.Read"]); // Important: Use "consumers" for personal accounts //tu: this one keeps being INTERACTIVE!!! ASKS FOR LOGIN EVERY TIME!!! even on OLD GOOD ONE: EmailAssistantAnyAndPersonal2.
  }

  public async Task<(bool success, string report)> Send(string trgEmailAdrs, string msgSubject, string msgBody, string[]? attachedFilenames = null, string? signatureImage = null)
  {
    var sw = Stopwatch.StartNew();
    var report = "";

    try
    {
      await _graphClient.Me.SendMail.PostAsync(new Microsoft.Graph.Me.SendMail.SendMailPostRequestBody       //await graphClient.Users["alex.pigida@outlook.com"].SendMail.PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
      {
        Message = new Message
        {
          Subject = msgSubject,
          Body = new ItemBody { Content = msgBody, ContentType = BodyType.Html },
          ToRecipients = [new Recipient { EmailAddress = new EmailAddress { Address = trgEmailAdrs } }],
          Attachments = attachedFilenames?.Select(file => new FileAttachment { Name = Path.GetFileName(file), ContentBytes = File.ReadAllBytes(file), OdataType = "#microsoft.graph.fileAttachment" }).Cast<Microsoft.Graph.Models.Attachment>().ToList()
        },
        SaveToSentItems = true
      });

      var logMsg = string.Format("{0} ({3}.{4})   sent to:  {1,-49} Subj: {2} \t (took {5:m\\:ss\\.f}){6}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), trgEmailAdrs, msgSubject, Environment.MachineName, Environment.UserName, sw.Elapsed, Environment.NewLine);
      Write(logMsg);
      if (isFirst) { isFirst = false; File.AppendAllText(LogFile, Environment.NewLine); }

      File.AppendAllText(LogFile, logMsg);

      return (true, report);
    }
    catch (System.Net.Mail.SmtpException ex)
    {/*
      As of 2025-01-21 22:07:00 started getting this error:
      The SMTP server requires a secure connection or the client was not authenticated. The server response was: 5.7.57 Client not authenticated to send mail. Error: 535 5.7.139 Authentication unsuccessful, basic authentication is disabled. [YT3PR01CA0122.CANPRD01.PROD.OUTLOOK.COM 2025-01-22T03:07:00.315Z 08DD393115E98C2B]
      Going to this link go to https://account.live.com/proofs/Manage/additional?mkt=en-us 
      and clicking on "Create new app password" and using the created password did not help.
      How to fix that?
      */
      report = ex.Message; _lgr?.LogWarning(ex.Message); Console.Beep(3333, 333);
    }
    catch (IOException ex) { report = ex.Message; _lgr?.LogWarning(ex.Message); Console.Beep(3333, 3333); }
    catch (Exception ex) { report = ex.Log(trgEmailAdrs); _lgr?.LogError(ex.Message); }

    return (false, report);
  }

  public async Task SendPhoto(string photoFullPath, string trgEmailAdrs)
  {
    var fi = new FileInfo(photoFullPath);

    _ = await Send(trgEmailAdrs,
       string.Format("Photo: {0}. ISent: {1} - {2}. Forewarnig...",
          Path.GetFileName(photoFullPath),
          DateTime.Now.ToShortDateString(),
          DateTime.Now.ToLongTimeString()),
       string.Format("Forewarnig:\r\n\nThe next email is going to contain the following image file: \r\n\n\t {0}  ({3:N1} kb).\r\n\n" +
          "If it did not reach your mailbox by \r\n\n\t {1}  \r\n\n\t {2}, \r\n\nplease let me know by replying to this letter.\r\n\nSasha's computer.",
          Path.GetFileName(photoFullPath),
          DateTime.Now.AddDays(1).ToLongDateString(),
          DateTime.Now.AddDays(1).ToLongTimeString(),
          fi.Length / 1000),
       Array.Empty<string>(), @"C:\g\GigaHunt\Src\sln\AvailStatusEmailer\Assets\MCSD Logo - Latest as of 2009.gif");
    Thread.Sleep(1000);

    _ = await Send(trgEmailAdrs,
       string.Format("Photo: {0}. ISent: {1} - {2}.",
          Path.GetFileName(photoFullPath),
          DateTime.Now.ToShortDateString(),
          DateTime.Now.ToLongTimeString()),
       string.Format("Photo:\r\n\t{0}\r\nSent:\r\n\t{1}\r\n\t{2}",
          Path.GetFileName(photoFullPath),
          DateTime.Now.ToLongDateString(),
          DateTime.Now.ToLongTimeString()),
       new string[] { photoFullPath }, @"C:\g\GigaHunt\Src\sln\AvailStatusEmailer\Assets\MCSD Logo - Latest as of 2009.gif");
    Thread.Sleep(1000);
  }
}

// Custom OAuth2 credentials class
public class OAuth2Credentials : ICredentialsByHost
{
  private readonly string _accessToken;

  public OAuth2Credentials(string accessToken) => _accessToken = accessToken;

  public NetworkCredential GetCredential(string host, int port, string authType) => new("", _accessToken);
}