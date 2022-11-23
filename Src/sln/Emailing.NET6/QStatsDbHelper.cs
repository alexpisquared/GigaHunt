namespace GigaHunt.AsLink;

public partial class QStatsDbHelper
{
  public static void InsertPhoneNumbersIntoDB(QstatsRlsContext dbx, List<string> pnLst, string emailId, DateTime emailedAt, DateTime now) { pnLst.ForEach(pn => InsertPhoneNumberIntoDB(dbx, emailId, emailedAt, now, pn)); }
  public static void InsertPhoneNumberIntoDB(QstatsRlsContext dbx, string emailId, DateTime emailedAt, DateTime now, string phnum)
  {
    //if (Debugger.IsAttached && !dbx.Phones.Any(r => r.PhoneNumber == phnum)) Debugger.Break();

    var phone = dbx.Phones.FirstOrDefault(r => r.PhoneNumber == phnum) ?? dbx.Phones.Add(new Phone { AddedAt = now, SeenFirst = emailedAt, SeenLast = emailedAt, PhoneNumber = phnum }).Entity;

    if (phone.SeenFirst > emailedAt) phone.SeenFirst = emailedAt;
    else
    if (phone.SeenLast < emailedAt) phone.SeenLast = emailedAt;

    InsertPhoneEmailXRef(dbx, emailId, now, phnum, phone);
    InsertPhoneAgencyXRef(dbx, emailId, now, phnum, phone);
  }
  public static void InsertPhoneAgencyXRef(QstatsRlsContext dbx, string emailId, DateTime now, string phnum, Phone phone)
  {
    if (Debugger.IsAttached && !dbx.Phones.Any(r => r.PhoneNumber == phnum)) Debugger.Break(); //todo: make sure OK.

    const int maxLen = 256;

    var agencyId = GetCompany(emailId).Length > maxLen ? GetCompany(emailId).Substring(GetCompany(emailId).Length - maxLen, maxLen) : GetCompany(emailId);

    var agency = dbx.Agencies.Find(agencyId) ?? dbx.Agencies.Add(new Agency { Id = agencyId, AddedAt = now, IsBroadcastee = true, Note = "Autoadded/assigned to be a Boradcastee: //todo: review." }).Entity;

    if (!dbx.PhoneAgencyXrefs.Any(r => r.PhoneId == phone.Id && r.AgencyId == agencyId))
    {
      dbx.PhoneAgencyXrefs.Add(new PhoneAgencyXref { PhoneId = phone.Id, AgencyId = agencyId, Note = "", AddedAt = now, Phone = phone, Agency = agency });
    }
  }
  public static void InsertPhoneEmailXRef(QstatsRlsContext dbx, string emailId, DateTime now, string phnum, Phone phone)
  {
    var email = dbx.Emails.Find(emailId);
    if (email is null)
      Console.Write($"(email is null): {emailId} ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■\n");
    else
    {
      if (string.IsNullOrEmpty(email.Phone)) email.Phone = phnum;

      if (!dbx.PhoneEmailXrefs.Any(r => r.PhoneId == phone.Id && r.EmailId == email.Id))
      {
        dbx.PhoneEmailXrefs.Add(new PhoneEmailXref { PhoneId = phone.Id, EmailId = email.Id, Note = "", AddedAt = now, Phone = phone });
      }
    }
  }
  static string GetCompany(string email) { return email.Split("@").LastOrDefault()?.Split(".").FirstOrDefault()?.ToLower() ?? "NoCompanyName"; }
}