namespace GigaHunt.Helpers
{
  public class FirstLastNameParser
  {
    public static string? ExtractFirstNameFromEmail(string emailAddress) // 2023-11
    {
      try
      {
        if (!emailAddress.Contains('@'))
        {
          return null;
        }

        var lowercase = emailAddress[..emailAddress.IndexOf('@')].Trim('\'').Replace("_", " ").Replace(".", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.ToLower();
        if (lowercase is null)
        {
          return null;
        }

        var firstName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(lowercase);
        if (firstName.StartsWith("Mc") && firstName.Length > 3)
        {
          return firstName[..2] + char.ToUpper(firstName[2]) + firstName[3..];                // make sure the third letter is is capitalized:
        }

        return firstName;
      }
      catch
      {
        return new GigaHunt.Helpers.FirstLastNameParser("").FirstName;
      }
    }

    public FirstLastNameParser(string emailAddress) // old
    {
      try
      {
        emailAddress = emailAddress.Replace("'", "");
        if (emailAddress.IndexOf('@') > 0)
          emailAddress = emailAddress.Substring(0, emailAddress.IndexOf('@'));

        emailAddress = emailAddress.Replace("_", " ");
        emailAddress = emailAddress.Replace(".", " ");

        if (emailAddress.IndexOf(',') > 0)
        {
          _firstName = emailAddress.Split(',')[1].Trim();
          _lastName = emailAddress.Split(',')[0].Trim();
        }
        else if (emailAddress.IndexOf(' ') > 0)
        {
          _firstName = emailAddress.Split(' ')[0].Trim();
          _lastName = emailAddress.Split(' ')[1].Trim();
        }
        else
          _firstName = _lastName = emailAddress.Trim();
      }
      catch
      {
        _firstName = _lastName = emailAddress.Trim();
      }
    }

    readonly string _firstName, _lastName;
    public string FirstName => _firstName;
    public string LastName => _lastName;
  }
}
