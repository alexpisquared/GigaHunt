namespace GigaHunt.AsLink;

public partial class QStatsDbHelper
{
  public static void InsertPhoneNumbersIntoDB(QstatsRlsContext dbq, List<string> pnLst, string emailId, DateTime emailedAt, DateTime now) { pnLst.ForEach(pn => InsertPhoneNumberIntoDB(dbq, emailId, emailedAt, now, pn)); }
  public static void InsertPhoneNumberIntoDB(QstatsRlsContext dbq, string emailId, DateTime emailedAt, DateTime now, string phoneNumber)
  {
    var phone = dbq.Phones.FirstOrDefault(r => r.PhoneNumber == phoneNumber) ?? dbq.Phones.Add(new Phone { AddedAt = now, SeenFirst = emailedAt, SeenLast = emailedAt, PhoneNumber = phoneNumber }).Entity;
    if (phone.SeenFirst > emailedAt) phone.SeenFirst = emailedAt;
    else
    if (phone.SeenLast < emailedAt) phone.SeenLast = emailedAt;

    InsertPhoneEmailXRef(dbq, emailId, now, phone);
    InsertPhoneAgencyXRef(dbq, emailId, now, phone);
  }
  public static void InsertPhoneAgencyXRef(QstatsRlsContext dbq, string emailId, DateTime now, Phone phone)
  {
    if (!dbq.Phones.Any(r => r.PhoneNumber == phone.PhoneNumber))
    {
      phone = dbq.Phones.Add(phone).Entity;
    }

    const int maxLen = 256;

    var agencyId = GetCompany(emailId).Length > maxLen ? GetCompany(emailId).Substring(GetCompany(emailId).Length - maxLen, maxLen) : GetCompany(emailId);

    var agency = dbq.Agencies.Find(agencyId) ?? dbq.Agencies.Add(new Agency { Id = agencyId, AddedAt = now, IsBroadcastee = true, Note = "Autoadded/assigned to be a Boradcastee: //todo: review." }).Entity;

    if (!dbq.PhoneAgencyXrefs.Any(r => r.PhoneId == phone.Id && r.AgencyId == agencyId))
    {
      dbq.PhoneAgencyXrefs.Add(new PhoneAgencyXref { Phone= phone, AgencyId = agencyId, Note = "", AddedAt = now, Agency = agency });
    }
  }
  public static void InsertPhoneEmailXRef(QstatsRlsContext dbq, string emailId, DateTime now, Phone phone)
  {
    var email = dbq.Emails.Find(emailId);
    if (email is null)
      Write($"(email is null): {emailId} ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■\n");
    else
    {
      if (string.IsNullOrEmpty(email.Phone)) email.Phone = phone.PhoneNumber;

      if (!dbq.PhoneEmailXrefs.Any(r => r.PhoneId == phone.Id && r.EmailId == email.Id))
      {
        dbq.PhoneEmailXrefs.Add(new PhoneEmailXref { PhoneId = phone.Id, EmailId = email.Id, Note = "", AddedAt = now, Phone = phone });
      }
    }
  }
  static string GetCompany(string email) { return email.Split("@").LastOrDefault()?.Split(".").FirstOrDefault()?.ToLower() ?? "NoCompanyName"; }
}