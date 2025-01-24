using System.Net.Mime;

namespace Emailing.NET6;
public class Emailer_Old_MS_Phases_Out
{
  public async Task<(bool success, string report)> Send(string trgEmailAdrs, string msgSubject, string msgBody, string[]? attachedFilenames = null, string? signatureImage = null) => await Send(cFrom, trgEmailAdrs, msgSubject, msgBody, attachedFilenames, signatureImage);

  async Task<(bool success, string report)> Send(string from, string trgEmailAdrs, string msgSubject, string msgBody, string[]? attachedFilenames = null, string? signatureImagePsv = null)
  {
    var sw = Stopwatch.StartNew();
    var report = "";

    try
    {
      using (var mailMessage = new MailMessage(cFrom, trgEmailAdrs, msgSubject, msgBody))
      {
        if (!string.IsNullOrEmpty(signatureImagePsv))
          foreach (var image in signatureImagePsv.Split('|'))
          {
            var contentId = Guid.NewGuid().ToString();

            //AlternateView html_View = AlternateView.CreateAlternateViewFromString(msgBody + "<img src=\"cid:" + contentId + "\" alt=\"MCSD\"><hr />", null, "text/html");
            var html_View = AlternateView.CreateAlternateViewFromString(msgBody.Replace("AlexTiny_LinkedIn.png", "cid:" + contentId), null, "text/html");

            //AlternateView plainView = AlternateView.CreateAlternateViewFromString(msgBody, null, "text/plain");

            var imageResource = new LinkedResource(image, new ContentType(MediaTypeNames.Image.Jpeg))
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
                mailMessage.Attachments.Add(new Attachment(fnm));

        //message.Attachments.Add(new Attachment("""C:\Documents and Settings\Grandma\Application Data\Microsoft\Signatures\QStatusUpdate(Wrd)_files\image002.jpg"""));
        //message.Attachments.Add(new Attachment("""C:\Documents and Settings\Grandma\Application Data\Microsoft\Signatures\QStatusUpdate(Wrd)_files\image001.png"""));

        var appPassword = new ConfigurationBuilder().AddUserSecrets<Emailer_Old_MS_Phases_Out>().Build()["AppPassword"] ?? throw new ArgumentNullException("#323 no key"); //tu: ad hoc user secrets
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

        if (DateTime.Now == DateTime.Today)
          await client.SendMailAsync(mailMessage); //todo: this fails for MinNavTmp only!!!  Why??????????????????????????
        else
          client.Send(mailMessage);
      }

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

  static string LogFile => """C:\temp\Logs\CV.Emailed.txt""";// Path.Combine(OneDrive.Folder(@"Public\Logs"), "CV.Emailed.txt");
  const string cFrom = "Alex.Pigida@outlook.com";
  static bool isFirst = true;
  readonly Microsoft.Extensions.Logging.ILogger _lgr;

  public Emailer_Old_MS_Phases_Out(Microsoft.Extensions.Logging.ILogger lgr) => this._lgr = lgr;
}