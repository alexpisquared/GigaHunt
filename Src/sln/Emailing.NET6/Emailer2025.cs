using Microsoft.Graph;
using Microsoft.Graph.Models;
namespace Emailing.NET6;
public class Emailer2025
{
  const string appReg = "EmailAssistantAnyAndPersonal2_2024";
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

  public async Task<(bool success, string report)> Send(string emailAddress, string msgSubject, string msgBody, string[]? attachedFilenames = null, string? imageFullPathFilename = null)
  {
    var sw = Stopwatch.StartNew();

    try
    {
      var message = new Message
      {
        Subject = msgSubject,
        Body = new ItemBody { Content = msgBody, ContentType = BodyType.Html },
        ToRecipients = [new Recipient { EmailAddress = new EmailAddress { Address = emailAddress } }],
        Attachments = []
      };

      if (attachedFilenames?.Length > 0) // Add regular attachments if any
      {
        message.Attachments.AddRange(
            attachedFilenames.Select(file =>
                new FileAttachment
                {
                  Name = Path.GetFileName(file),
                  ContentBytes = File.ReadAllBytes(file),
                  OdataType = "#microsoft.graph.fileAttachment"
                }));
      }

      if (imageFullPathFilename != null && File.Exists(imageFullPathFilename)) // Add inline image if provided
      {
        message.Attachments.Add(new FileAttachment
        {
          Name = Path.GetFileName(imageFullPathFilename),
          ContentBytes = File.ReadAllBytes(imageFullPathFilename),
          ContentType = "image/jpeg",
          ContentId = "image1",
          IsInline = true,
          OdataType = "#microsoft.graph.fileAttachment"
        });
      }

      await _graphClient.Me.SendMail.PostAsync(new Microsoft.Graph.Me.SendMail.SendMailPostRequestBody { Message = message, SaveToSentItems = true });

      _lgr.LogInformation($"sent to:  {emailAddress,-49} Subj: {msgSubject} \t (took {sw.Elapsed:m\\:ss\\.f})");

      return (true, "Success sending email.");
    }
    catch (Exception ex)
    {
      _lgr.LogError(ex, ex.Log(emailAddress));
      return (false, ex.Log(emailAddress));
    }
  }

  public async Task<(bool success, string report)> ListInboxItemsMatchingEnailAddress(string emailAddress)
  {
     
  }
}