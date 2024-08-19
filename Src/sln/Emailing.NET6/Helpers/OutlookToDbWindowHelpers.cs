using GigaHunt.AsLink;
using Microsoft.Extensions.Logging;
using static Azure.Core.HttpHeader;

namespace Emailing.NET6.Helpers;

public class OutlookToDbWindowHelpers
{
  readonly DateTime _batchNow = DateTime.Now;
  Microsoft.Extensions.Logging.ILogger? _lgr;

  public OutlookToDbWindowHelpers(Microsoft.Extensions.Logging.ILogger? lgr) => _lgr = lgr;

  public async Task<bool?> CheckInsert_EMail_EHist_Async(QstatsRlsContext dbq, string email, string firstName, string lastName, string? subject, string? body, DateTime? sentOn, DateTime? timeRecdSent, string isRcvd, string RS, string? notes = null)
  {
    var now = DateTime.Now;
    var (email1, isNew) = await CheckInsertEMailAsync(dbq, email, firstName, lastName, notes, now, RS == "S");
    if (email1 == null)
      return null;

    await EHistInsUpdSaveAsync(dbq, subject, body, sentOn ?? now, timeRecdSent ?? now, RS, email1, notes, now);

    return isNew;
  }

  public async Task<(Email? email1, bool? isNew)> CheckInsertEMailAsync(QstatsRlsContext dbq, string emailAddress, string firstName, string lastName, string? notes, DateTime now, bool addToTheEndOfBroadcastQueue = true)
  {
    const int maxLen = 256;

    if (emailAddress.EndsWith("@msg.monster.com") && emailAddress.Length > 46) // ~ 3212846259f94b158701020f5ca8ac4e@msg.monster.com
      return (null, null);

    if (emailAddress.Length > maxLen)
      emailAddress = emailAddress.Substring(emailAddress.Length - maxLen, maxLen);

    var emailExisting = dbq.Emails.Find(emailAddress);
    if (emailExisting != null)
    {
      if (!string.IsNullOrEmpty(notes))
      {
        emailExisting.Notes = $"{now:yy.MM.dd HH:mm}  {notes}\n{emailExisting.Notes}";
        emailExisting.LastAction = now;
        _ = await dbq.TrySaveReportAsync("checkInsertEMail");
      }
      return (emailExisting, false);
    }

    var agency = OutlookHelper6.GetCompanyName(emailAddress);

    try
    {
      var r2 = dbq.Agencies.Any(r => r.Id.Equals(agency.ToLower()));
      var r3 = dbq.Agencies.Any(r => r.Id.Equals(agency.ToUpper()));

      if (!dbq.Agencies.Any(r => r.Id.Equals(agency))) //i think db is set to be case ignore:  , StringComparison.InvariantCultureIgnoreCase)) )
        _ = dbq.Agencies.Add(new Agency
        {
          Id = agency.Length > maxLen ? agency.Substring(agency.Length - maxLen, maxLen) : agency,
          AddedAt = now
        });
    }
    catch (Exception ex) { _ = ex.Log("."); throw; }

    var emailNew = dbq.Emails.Add(new Email
    {
      Id = emailAddress.Length > maxLen ? emailAddress.Substring(emailAddress.Length - maxLen, maxLen) : emailAddress,
      Company = agency,
      Fname = firstName,
      Lname = lastName,
      Notes = notes,
      AddedAt = now,
      ReSendAfter = null,
      NotifyPriority = addToTheEndOfBroadcastQueue ? int.Parse($"{now:yyMMdd}") : -333
    }).Entity;

    _ = await dbq.TrySaveReportAsync("checkInsertEMail");

    return (emailNew, true);
  }

  public async Task EHistInsUpdSaveAsync(QstatsRlsContext dbq, string? subject, string? body, DateTime? sentOn, DateTime timeRecdSent, string rs, Email email, string? notes, DateTime now)
  {
    try
    {
      const int dupeEntryPreventionInMin = 2;
      var gt = timeRecdSent.AddMinutes(-dupeEntryPreventionInMin);
      var lt = timeRecdSent.AddMinutes(+dupeEntryPreventionInMin);         //var ch = isRcvd ? ctx.EHists.Where(p => p.EmailedAt.HasValue && gt < p.EmailedAt.Value && p.EmailedAt.Value < lt && p.EMailId == id.Id) : ctx.EHists.Where(p => p.EmailedAt.HasValue && gt < p.EmailedAt.Value && p.EmailedAt.Value < lt && p.EMailId == id.Id); if (ch.Count() < 1)
      var eh = dbq.Ehists.FirstOrDefault(p => p.RecivedOrSent == rs && p.EmailId == email.Id && gt < p.EmailedAt && p.EmailedAt < lt);
      if (eh is not null)
      {
        _ = await PhoneNumbersGetInsSave(dbq, timeRecdSent, email, eh, now);

        if (eh.SentOn != sentOn || notes != null)
        {
          eh.SentOn = sentOn;
          eh.Notes = notes ?? "";
          _ = await dbq.TrySaveReportAsync("checkInsertEHist SentOn update");
        }

        _lgr?.LogTrace($"│   No EHist added: There is already the same record in DB within the +-{dupeEntryPreventionInMin} minute range: {(timeRecdSent - eh.EmailedAt).TotalSeconds,4:N1} sec apart.   {email.Id}");
      }
      else
      {
        var newEH = new Ehist
        {
          RecivedOrSent = rs,
          Email = email,
          LetterBody = string.IsNullOrEmpty(body) ? "" : body.Replace("\n\n\n", "\n\n").Replace("\n\n", "\n").Replace("\n\n\n", "\n\n").Replace("\n\n", "\n"),
          LetterSubject = subject,
          AddedAt = now,
          Notes = notes ?? "",
          SentOn = sentOn,
          EmailedAt = timeRecdSent
        };
        var newCH2 = dbq.Ehists.Add(newEH);

        var dbReport = await dbq.TrySaveReportAsync("checkInsertEHist New letter");
        _lgr?.LogTrace($"│   EHist added for  {email.Id,-50} {dbReport.report}");

        _ = await PhoneNumbersGetInsSave(dbq, timeRecdSent, email, newEH, now);
      }
    }
    catch (Exception ex) { _ = ex.Log(); throw; }
  }

  async Task<string> PhoneNumbersGetInsSave(QstatsRlsContext dbq, DateTime timeRecdSent, Email email, Ehist newEH, DateTime now)
  {
    var phones = RegexHelper.GetUniquePhoneNumbersFromLetter(newEH);
    phones.ToList().ForEach(pn => QStatsDbHelper.InsertPhoneNumberIntoDB(dbq, email.Id, timeRecdSent, now, pn));

    var (success, rowsSavedCnt, report) = await dbq.TrySaveReportAsync(nameof(OutlookToDbWindowHelpers));

    return report;
  }
}