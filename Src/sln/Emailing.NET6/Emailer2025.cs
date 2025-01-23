//using EnvDTE;
//using Microsoft.Office.Interop.Outlook;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using System.Net.Mime;

namespace Emailing.NET6;
public class Emailer2025
{
  static string LogFile => """C:\temp\Logs\CV.Emailed.txt""";// Path.Combine(OneDrive.Folder(@"Public\Logs"), "CV.Emailed.txt");
  const string cFrom = "Alex.Pigida@outlook.com", appReg = "AppIdNew2025_8821bef3"; // AppIdNew2025_3936846f"; // AppIdOld2024_af27ddbb"; // 
  static bool isFirst = true;
  readonly ILogger _lgr;
  readonly IConfiguration _configuration;
  readonly GraphServiceClient graphClient;

  public Emailer2025(ILogger lgr)
  {
    _lgr = lgr;
    _configuration = new ConfigurationBuilder().AddUserSecrets<Emailer2025>().Build();

    var options = new InteractiveBrowserCredentialOptions
    {
      TenantId = "consumers", // Important: Use "consumers" for personal accounts
      ClientId = _configuration[$"{appReg}:ClientId"],
      RedirectUri = new Uri("http://localhost")
    };

    var credential = new InteractiveBrowserCredential(options);
    graphClient = new GraphServiceClient(credential, ["Mail.Send", "Mail.Send.Shared", "User.Read"]);
  }

  public async Task<(bool success, string report)> Send(string trgEmailAdrs, string msgSubject, string msgBody, string[]? attachedFilenames = null, string? signatureImage = null) => await Send(cFrom, trgEmailAdrs, msgSubject, msgBody, attachedFilenames, signatureImage);

  async Task<(bool success, string report)> Send(string from, string trgEmailAdrs, string msgSubject, string msgBody, string[]? attachedFilenames = null, string? signatureImage = null)
  {
    var sw = Stopwatch.StartNew();
    var report = "";

    try
    {
      await graphClient.Me.SendMail.PostAsync(new Microsoft.Graph.Me.SendMail.SendMailPostRequestBody       //await graphClient.Users["alex.pigida@outlook.com"].SendMail.PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
      {
        Message = new Message
        {
          Subject = msgSubject,
          Body = new ItemBody { Content = msgBody, ContentType = BodyType.Html },
          ToRecipients = [new Recipient { EmailAddress = new EmailAddress { Address = trgEmailAdrs } }],
          Attachments = attachedFilenames?.Select(file => new FileAttachment
          {
            Name = Path.GetFileName(file),
            ContentBytes = File.ReadAllBytes(file),
            OdataType = "#microsoft.graph.fileAttachment"
          }).Cast<Microsoft.Graph.Models.Attachment>().ToList()
        },
        SaveToSentItems = true
      });

      /*
      using (var mailMessage = new MailMessage(cFrom, trgEmailAdrs, msgSubject, msgBody))
      {
        if (!string.IsNullOrEmpty(signatureImage))
          //int i = 0;
          foreach (var f in signatureImage.Split('|'))
          {
            var contentId = Guid.NewGuid().ToString();

            //AlternateView html_View = AlternateView.CreateAlternateViewFromString(msgBody + "<img src=\"cid:" + contentId + "\" alt=\"MCSD\"><hr />", null, "text/html");
            var html_View = AlternateView.CreateAlternateViewFromString(msgBody.Replace("AlexTiny_LinkedIn.png", "cid:" + contentId), null, "text/html");

            //AlternateView plainView = AlternateView.CreateAlternateViewFromString(msgBody, null, "text/plain");

            var imageResource = new System.Net.Mail.LinkedResource(f, new System.Net.Mime.ContentType(MediaTypeNames.Image.Jpeg))
            {
              ContentId = contentId
            };
            html_View.LinkedResources.Add(imageResource);

            //mailMessage.AlternateViews.Add(plainView);
            mailMessage.AlternateViews.Add(html_View);
          }

        mailMessage.Subject = msgSubject;
        mailMessage.Body = msgBody;
        mailMessage.IsBodyHtml = true;

        if (attachedFilenames != null)
          foreach (var fnm in attachedFilenames)
            if (!string.IsNullOrEmpty(fnm))
              if (!File.Exists(fnm))
                throw new FileNotFoundException(fnm);
              else
                mailMessage.Attachments.Add(new System.Net.Mail.Attachment(fnm));

        //message.Attachments.Add(new Attachment("""C:\Documents and Settings\Grandma\Application Data\Microsoft\Signatures\QStatusUpdate(Wrd)_files\image002.jpg"""));
        //message.Attachments.Add(new Attachment("""C:\Documents and Settings\Grandma\Application Data\Microsoft\Signatures\QStatusUpdate(Wrd)_files\image001.png"""));

#if true
        /* The app password approach you tried isn't working because Microsoft is moving away from this authentication method.
        To fix this, you'll need to:
        1.	Use OAuth 2.0 authentication instead of basic authentication
        2.	Register your application in Azure AD by following answer to "What are the steps to register my application in Azure AD for OAuth 2.0 authentication?"
        3.	Update your code to use Microsoft.Graph or Microsoft.Identity.Client for authentication
        Here's a high-level example of what needs to change: * /

        // Get OAuth 2.0 token
        var scopes = new[] { "https://graph.microsoft.com/.default" };

        var _confidentialClientApplication = ConfidentialClientApplicationBuilder
    .Create(_configuration[$"{appReg}:ClientId"])
    .WithClientSecret(_configuration[$"{appReg}:ClientSecret"])
    .WithTenantId(_configuration["TenantId"])
    .Build();


        var authResult = await _confidentialClientApplication
            .AcquireTokenForClient(scopes)
            .ExecuteAsync();

        using var client = new SmtpClient
        {
          Host = "smtp.office365.com",
          Port = 587,
          EnableSsl = true,
          DeliveryMethod = SmtpDeliveryMethod.Network,
          UseDefaultCredentials = false,
          Credentials = new OAuth2Credentials(authResult.AccessToken)
        };
#else
        var appPassword = _configuration["AppPassword"] ?? throw new ArgumentNullException("#323 no key"); //tu: ad hoc user secrets
        var credentials = new NetworkCredential(EmailerHelpers.GetMicrosoftAccountName(), appPassword);
       
        using var client = new SmtpClient // see readme # 8979 !!!!
        {
          Host = "smtp.office365.com",
          Port = 587,
          EnableSsl = true,
          DeliveryMethod = SmtpDeliveryMethod.Network,
          UseDefaultCredentials = false,
          Credentials = credentials
        };
#endif

        if (DateTime.Now == DateTime.Today)
          await client.SendMailAsync(mailMessage); //todo: this fails for MinNavTmp only!!!  Why??????????????????????????
        else
          client.Send(mailMessage);
      }
*/

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
      Going to this link           go to https://account.live.com/proofs/Manage/additional?mkt=en-us 
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