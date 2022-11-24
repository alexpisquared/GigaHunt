using DB.QStats.Std.Models;
using Emailing.NET6;
using GigaHunt.AsLink;

namespace OutlookToDbWpfApp;

public static class OutlookToDbWindowHelpers
{
  static readonly DateTime Now = DateTime.Now;

  public static async Task<bool> CheckInsert_EMail_EHist_Async(QstatsRlsContext _db, string email, string firstName, string lastName, string? subject, string? body, DateTime? sentOn, DateTime? timeRecdSent, string isRcvd, string RS)
  {
    var em = await CheckInsertEMailAsync(_db, email, firstName, lastName, isRcvd);
    if (em == null) return false;

    await EHistInsUpdSaveAsync(_db, subject, body, sentOn, timeRecdSent ?? DateTime.Now, RS, em);

    var isNew = em?.AddedAt == Now;
    return isNew;
  }

  public static async Task<Email?> CheckInsertEMailAsync(QstatsRlsContext _db, string email, string firstName, string lastName, string notes)
  {
    const int maxLen = 256;

    if (email.EndsWith("@msg.monster.com") && email.Length > 46) // ~ 3212846259f94b158701020f5ca8ac4e@msg.monster.com
      return null;

    if (email.Length > maxLen)
      email = email.Substring(email.Length - maxLen, maxLen);

    var em = _db.Emails.Find(email);
    if (em == null)
    {
      var agency = OutlookHelper6.GetCompanyName(email);

      try
      {
        var r2 = _db.Agencies.Any(r => r.Id.Equals(agency.ToLower()));
        var r3 = _db.Agencies.Any(r => r.Id.Equals(agency.ToUpper()));

        if (!_db.Agencies.Any(r => r.Id.Equals(agency))) //i think db is set to be case ignore:  , StringComparison.InvariantCultureIgnoreCase)) )
        {
          _ = _db.Agencies.Add(new Agency
          {
            Id = agency.Length > maxLen ? agency.Substring(agency.Length - maxLen, maxLen) : agency,
            AddedAt = Now
          });
        }
      }
      catch (Exception ex) { ex.Log("."); throw; }


      em = _db.Emails.Add(new Email
      {
        Id = email.Length > maxLen ? email.Substring(email.Length - maxLen, maxLen) : email,
        Company = agency,
        Fname = firstName,
        Lname = lastName,
        Notes = notes,
        AddedAt = Now,
        ReSendAfter = null,
        NotifyPriority = 99
      }).Entity;

      _ = await _db.TrySaveReportAsync("checkInsertEMail");
    }

    return em;
  }

  public static async Task EHistInsUpdSaveAsync(QstatsRlsContext dbx, string? subject, string? body, DateTime? sentOn, DateTime timeRecdSent, string rs, Email email)
  {
    //insertEMailEHistItem(isRcvd, timeRecdSent, email, subject, body);		}		void insertEMailEHistItem(bool isRcvd, DateTime timeRecdSent, Email email, string subject, string body)		{
    try
    {
      var gt = timeRecdSent.AddMinutes(-5);
      var lt = timeRecdSent.AddMinutes(+5);         //var ch = isRcvd ? ctx.EHists.Where(p => p.EmailedAt.HasValue && gt < p.EmailedAt.Value && p.EmailedAt.Value < lt && p.EMailId == id.Id) : ctx.EHists.Where(p => p.EmailedAt.HasValue && gt < p.EmailedAt.Value && p.EmailedAt.Value < lt && p.EMailId == id.Id); if (ch.Count() < 1)
      var eh = dbx.Ehists.FirstOrDefault(p => p.RecivedOrSent == rs && p.EmailId == email.Id && gt < p.EmailedAt && p.EmailedAt < lt);
      if (eh is not null)
      {
       await PhoneNumbersGetInsSave(dbx, timeRecdSent, email, eh);

        if (eh.SentOn != sentOn)
        {
          eh.SentOn = sentOn;
          _ = await dbx.TrySaveReportAsync("checkInsertEHist SentOn update");
        }
      }
      else
      {
        var newEH = new Ehist
        {
          RecivedOrSent = rs,
          Email = email,
          LetterBody = string.IsNullOrEmpty(body) ? "" : body.Replace("\n\n\n", "\n\n").Replace("\n\n", "\n").Replace("\r\n\r\n\r\n", "\n\n").Replace("\r\n\r\n", "\n"),
          LetterSubject = subject,
          AddedAt = Now,
          Notes = "",
          SentOn = sentOn,
          EmailedAt = timeRecdSent
        };
        var newCH2 = dbx.Ehists.Add(newEH);

        _ = await dbx.TrySaveReportAsync("checkInsertEHist New letter");

        await PhoneNumbersGetInsSave(dbx, timeRecdSent, email, newEH);
      }
    }
    catch (Exception ex) { ex.Log(); throw; }
  }

  static async Task PhoneNumbersGetInsSave(QstatsRlsContext dbx, DateTime timeRecdSent, Email email, Ehist newEH)
  {
    var phones = RegexHelper.GetUniquePhoneNumbersFromLetter(newEH);
    phones.ToList().ForEach(pn => QStatsDbHelper.InsertPhoneNumberIntoDB(dbx, email.Id, timeRecdSent, Now, pn));

    _ = await dbx.TrySaveReportAsync(nameof(OutlookToDbWindowHelpers));
  }
}