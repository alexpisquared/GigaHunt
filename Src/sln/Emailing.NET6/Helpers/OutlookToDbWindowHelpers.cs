using GigaHunt.AsLink;
using static Azure.Core.HttpHeader;

namespace Emailing.NET6.Helpers;

public static class OutlookToDbWindowHelpers
{
  static readonly DateTime _batchNow = DateTime.Now;

  public static async Task<bool> CheckInsert_EMail_EHist_Async(QstatsRlsContext dbq, string email, string firstName, string lastName, string? subject, string? body, DateTime? sentOn, DateTime? timeRecdSent, string isRcvd, string RS, string? notes = null)
  {
    var now = DateTime.Now;
    var em = await CheckInsertEMailAsync(dbq, email, firstName, lastName,  notes, now);
    if (em == null) return false;

    await EHistInsUpdSaveAsync(dbq, subject, body, sentOn ?? now, timeRecdSent ?? now, RS, em, notes, now);

    var isNew = em?.AddedAt == _batchNow; //todo: revisit this logic as it is not correct. 2023-11-26
    return isNew;
  }

  public static async Task<Email?> CheckInsertEMailAsync(QstatsRlsContext dbq, string email, string firstName, string lastName, string? notes, DateTime now )
  {
    const int maxLen = 256;

    if (email.EndsWith("@msg.monster.com") && email.Length > 46) // ~ 3212846259f94b158701020f5ca8ac4e@msg.monster.com
      return null;

    if (email.Length > maxLen)
      email = email.Substring(email.Length - maxLen, maxLen);

    var em = dbq.Emails.Find(email);
    if (em != null)
    {
      em.Notes = $"{now:yyMMdd}: {notes ?? "?!?!"} | {em.Notes}";
      em.LastAction = now;
      _ = await dbq.TrySaveReportAsync("checkInsertEMail");
    }
    else
    {
      var agency = OutlookHelper6.GetCompanyName(email);

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

      em = dbq.Emails.Add(new Email
      {
        Id = email.Length > maxLen ? email.Substring(email.Length - maxLen, maxLen) : email,
        Company = agency,
        Fname = firstName,
        Lname = lastName,
        Notes = notes,
        AddedAt = now,
        ReSendAfter = null,
        NotifyPriority = 1000000
      }).Entity;

      _ = await dbq.TrySaveReportAsync("checkInsertEMail");
    }

    return em;
  }

  public static async Task EHistInsUpdSaveAsync(QstatsRlsContext dbq, string? subject, string? body, DateTime? sentOn, DateTime timeRecdSent, string rs, Email email, string? notes, DateTime now)
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

        _ = new Exception().Log($"??? No EHist added: There is already the same record in DB within the +-{dupeEntryPreventionInMin} minute range ???  {email.Id}");
      }
      else
      {
        var newEH = new Ehist
        {
          RecivedOrSent = rs,
          Email = email,
          LetterBody = string.IsNullOrEmpty(body) ? "" : body.Replace("\n\n\n", "\n\n").Replace("\n\n", "\n").Replace("\r\n\r\n\r\n", "\n\n").Replace("\r\n\r\n", "\n"),
          LetterSubject = subject,
          AddedAt = now,
          Notes = notes ?? "",
          SentOn = sentOn,
          EmailedAt = timeRecdSent
        };
        var newCH2 = dbq.Ehists.Add(newEH);

        _ = await dbq.TrySaveReportAsync("checkInsertEHist New letter");

        _ = await PhoneNumbersGetInsSave(dbq, timeRecdSent, email, newEH, now);
      }
    }
    catch (Exception ex) { _ = ex.Log(); throw; }
  }

  static async Task<string> PhoneNumbersGetInsSave(QstatsRlsContext dbq, DateTime timeRecdSent, Email email, Ehist newEH, DateTime now)
  {
    var phones = RegexHelper.GetUniquePhoneNumbersFromLetter(newEH);
    phones.ToList().ForEach(pn => QStatsDbHelper.InsertPhoneNumberIntoDB(dbq, email.Id, timeRecdSent, now, pn));

    var (success, rowsSavedCnt, report) = await dbq.TrySaveReportAsync(nameof(OutlookToDbWindowHelpers));

    return report;
  }
}