using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Emailing.NET6.MailKit;
internal class MailKitVM
{
  public string? Message { get; private set; }
  public string? ResponseMessage { get; private set; }

  public void OnPost(IConfigurationRoot cfg)
  {
    //Create a MimeMessage            
    var email = new MimeMessage();
    email.From.Add(new MailboxAddress(cfg["EmailSettings:FromName"], cfg["EmailSettings:FromEmail"]));
    email.To.Add(new MailboxAddress(cfg["EmailSettings:ToName"], cfg["EmailSettings:ToEmail"]));
    email.Subject = cfg["EmailSettings:Subject"];
    email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
    {
      Text = Message
    };

    //MimeMessage is ready, now send the Email.
    using var client = new SmtpClient();
    client.Connect(cfg["EmailSettings:Host"], int.Parse(cfg["EmailSettings:Port"]), false);
    client.Authenticate(cfg["EmailSettings:Username"], cfg["EmailSettings:Password"]);
    ResponseMessage += client.Send(email);
    client.Disconnect(true);
  }
}
