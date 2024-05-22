using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Emailing.NET6.MailKit;
internal class MailKitVM
{
  readonly IConfigurationRoot _cfg;
  public MailKitVM(IConfigurationRoot cfg) => _cfg = cfg;

  public string? Message { get; private set; }
  public string? ResponseMessage { get; private set; }

  public void ProtoTest() => OnPost();
  public void OnPost(
    string fromName = "Oleksa",
    string fromEmail = "alex.pigida@outlook.com",
    string toName = "Alex",
    string toEmail = "pigida@gmail.com",
    string subject = "Test subject",
    string message = "Test body",
    string host = "smtp.office365.com",
    int port = 587 //, string username = "username", string password = "password"
    )
  {
    var email = new MimeMessage();
    email.From.Add(new MailboxAddress(fromName, fromEmail));
    email.To.Add(new MailboxAddress(toName, toEmail));
    email.Subject = subject;
    email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

    var appPassword = new ConfigurationBuilder().AddUserSecrets<Emailer>().Build()["AppPassword"] ?? throw new ArgumentNullException("#32 no key"); //tu: ad hoc user secrets
    var credentials = new NetworkCredential(EmailerHelpers.GetMicrosoftAccountName(), appPassword);

    //MimeMessage is ready, now send the Email.
    using var client = new SmtpClient();
    client.Connect(host, port, false);
    client.Authenticate(credentials); // client.Authenticate(username, password);
    ResponseMessage += client.Send(email);
    client.Disconnect(true);
  }

  public void OnPost() // https://github.com/sreejukg/SendEmail/blob/415a83cc915f4a755d106a8d2154c503c35fd15d/SendMailProject/SendMailProject/Pages/Index.cshtml.cs#L29
  {
    var email = new MimeMessage();
    email.From.Add(new MailboxAddress(_cfg["EmailSettings:FromName"], _cfg["EmailSettings:FromEmail"]));
    email.To.Add(new MailboxAddress(_cfg["EmailSettings:ToName"], _cfg["EmailSettings:ToEmail"]));
    email.Subject = _cfg["EmailSettings:Subject"];
    email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
    {
      Text = Message
    };

    //MimeMessage is ready, now send the Email.
    using var client = new SmtpClient();
    client.Connect(_cfg["EmailSettings:Host"], int.Parse(_cfg["EmailSettings:Port"] ?? throw new ArgumentNullException("#######LKJLKJ 132")), false);
    client.Authenticate(_cfg["EmailSettings:Username"], _cfg["EmailSettings:Password"]);
    ResponseMessage += client.Send(email);
    client.Disconnect(true);
  }
}