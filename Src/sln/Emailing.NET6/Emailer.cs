#define REALREADY			//uncomment only when ready:
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Principal;
using System.Threading.Tasks;
using WpfUserControlLib.Extensions;

namespace Emailing
{
  public class Emailer
  {
    public static async Task<bool> Send(string trgEmailAdrs, string msgSubject, string msgBody, string[]? attachedFilenames = null, string? signatureImage = null) => await Send(cFrom, trgEmailAdrs, msgSubject, msgBody, attachedFilenames, signatureImage);
    public static async Task<bool> Send(string from, string trgEmailAdrs, string msgSubject, string msgBody, string[]? attachedFilenames = null, string? signatureImage = null)
    {
      var sw = Stopwatch.StartNew();
      try
      {
        using (var mailMessage = new MailMessage(cFrom, trgEmailAdrs, msgSubject, msgBody))
        {
          if (!string.IsNullOrEmpty(signatureImage))
          {
            //int i = 0;
            foreach (var f in signatureImage.Split('|'))
            {
              var contentId = Guid.NewGuid().ToString();

              //AlternateView html_View = AlternateView.CreateAlternateViewFromString(msgBody + "<img src=\"cid:" + contentId + "\" alt=\"MCSD\"><hr />", null, "text/html");
              var html_View = AlternateView.CreateAlternateViewFromString(msgBody.Replace("AlexTiny_LinkedIn.png", "cid:" + contentId), null, "text/html");

              //AlternateView plainView = AlternateView.CreateAlternateViewFromString(msgBody, null, "text/plain");

              var imageResource = new LinkedResource(f, new ContentType(MediaTypeNames.Image.Jpeg))
              {
                ContentId = contentId
              };
              html_View.LinkedResources.Add(imageResource);

              //mailMessage.AlternateViews.Add(plainView);
              mailMessage.AlternateViews.Add(html_View);
            }
          }

          mailMessage.Subject = msgSubject;
          mailMessage.Body = msgBody;
          mailMessage.IsBodyHtml = true;

          if (attachedFilenames != null)
          {
            foreach (var fnm in attachedFilenames)
            {
              if (!string.IsNullOrEmpty(fnm))
                if (!File.Exists(fnm))
                  throw new FileNotFoundException(fnm);
                else
                  mailMessage.Attachments.Add(new Attachment(fnm));
            }
          }

          //message.Attachments.Add(new Attachment(@"C:\Documents and Settings\Grandma\Application Data\Microsoft\Signatures\QStatusUpdate(Wrd)_files\image002.jpg"));
          //message.Attachments.Add(new Attachment(@"C:\Documents and Settings\Grandma\Application Data\Microsoft\Signatures\QStatusUpdate(Wrd)_files\image001.png"));


          using (var client = new SmtpClient("smtp.office365.com", 587) { EnableSsl = true, Credentials = new System.Net.NetworkCredential(GetMicrosoftAccountName(), File.ReadAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\1PageNote\\OutlookKeysOrSomething.txt")) }) // https://account.live.com/proofs/Manage/additional?mkt=en-us - App passwords   Some apps ... don't support security codes for two-step verification. In these cases, you need to create an app password to sign in. Learn more about app passwords.   Create a new app password
          {
            try { await client.SendMailAsync(mailMessage); } // letter does not appear in the Outlook ==> use DB to track sent messages.
            catch (Exception ex) { ex.Pop($"Error emailing to: {trgEmailAdrs}"); throw; }           //tu: add to App.cfg: <system.net><mailSettings><smtp deliveryMethod="Network" from="test@foo.com"><!--userName="pigida@aei.ca" password=""--><!--port="25"--><network host="mail.aei.ca" defaultCredentials="true"/></smtp></mailSettings></system.net>
          }
        }

        var logMsg = string.Format("{0} ({3}.{4})   sent to:  {1,-49} Subj: {2} \t (took {5:m\\:ss\\.f}){6}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), trgEmailAdrs, msgSubject, Environment.MachineName, Environment.UserName, sw.Elapsed, Environment.NewLine);
        Trace.Write(logMsg);
        if (isFirst) { isFirst = false; File.AppendAllText(LogFile, Environment.NewLine); }
        File.AppendAllText(LogFile, logMsg);

        return true;
      }
      catch (FormatException ex) { ex.Pop(trgEmailAdrs); }
      catch (SmtpException ex) { ex.Pop(trgEmailAdrs); }
      catch (Exception ex) { ex.Pop(trgEmailAdrs); }

      return false;
    }
    public static string GetMicrosoftAccountName() //todo: move it to a proper place.
    {
      var wi = WindowsIdentity.GetCurrent();
      var groups = from g in wi.Groups
                   select new SecurityIdentifier(g.Value)
                   .Translate(typeof(NTAccount)).Value;

      var msAccount = (from g in groups
                       where g.StartsWith(@"MicrosoftAccount\")
                       select g).FirstOrDefault();

      return msAccount == null ? wi.Name : msAccount.Substring(@"MicrosoftAccount\".Length);
    }

    public static async Task SendPhoto(string photoFullPath, string trgEmailAdrs)
    {
      var fi = new FileInfo(photoFullPath);

      await Send(trgEmailAdrs,
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
      System.Threading.Thread.Sleep(1000);

      await Send(trgEmailAdrs,
         string.Format("Photo: {0}. ISent: {1} - {2}.",
            Path.GetFileName(photoFullPath),
            DateTime.Now.ToShortDateString(),
            DateTime.Now.ToLongTimeString()),
         string.Format("Photo:\r\n\t{0}\r\nSent:\r\n\t{1}\r\n\t{2}",
            Path.GetFileName(photoFullPath),
            DateTime.Now.ToLongDateString(),
            DateTime.Now.ToLongTimeString()),
         new string[] { photoFullPath }, @"C:\g\GigaHunt\Src\sln\AvailStatusEmailer\Assets\MCSD Logo - Latest as of 2009.gif");
      System.Threading.Thread.Sleep(1000);
    }

    static string LogFile => @"C:\temp\Logs\CV.Emailed.txt";// Path.Combine(OneDrive.Folder(@"Public\Logs"), "CV.Emailed.txt");
    const string cFrom = "Alex.Pigida@outlook.com";   
    static bool isFirst = true;
  }
}