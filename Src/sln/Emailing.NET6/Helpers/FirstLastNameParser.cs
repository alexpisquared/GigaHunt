namespace Emailing.NET6.Helpers;

public class FirstLastNameParser
{
  public static string? ExtractFirstNameFromEmail(string emailAddress) // 2023-11
  {
    try
    {
      if (!emailAddress.Contains('@'))
        return null;

      var fName = emailAddress[..emailAddress.IndexOf('@')].Trim('\'').Replace("_", " ").Replace(".", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
      if (fName is null)
        return null;

      var firstName = ToTitleCase(fName);

      return firstName;
    } catch
    {
      return new FirstLastNameParser("").FirstName;
    }
  }

  public static string ToTitleCase(string rawName)
  {
    var firstName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(rawName.ToLower());
    if (firstName.StartsWith("Mc") && firstName.Length > 3)
      return firstName[..2] + char.ToUpper(firstName[2]) + firstName[3..];                // make sure the third letter is is capitalized:

    return firstName;
  }

  public FirstLastNameParser(string emailAddress) // old
  {
    try
    {
      emailAddress = emailAddress.Replace("'", "");
      if (emailAddress.IndexOf('@') > 0)
        emailAddress = emailAddress[..emailAddress.IndexOf('@')];

      emailAddress = emailAddress.Replace("_", " ");
      emailAddress = emailAddress.Replace(".", " ");

      if (emailAddress.IndexOf(',') > 0)
      {
        FirstName = emailAddress.Split(',')[1].Trim();
        LastName = emailAddress.Split(',')[0].Trim();
      } else if (emailAddress.IndexOf(' ') > 0)
      {
        FirstName = emailAddress.Split(' ')[0].Trim();
        LastName = emailAddress.Split(' ')[1].Trim();
      } else
        FirstName = LastName = emailAddress.Trim();
    } catch
    {
      FirstName = LastName = emailAddress.Trim();
    }
  }

  public string FirstName { get; }
  public string LastName { get; }
}
