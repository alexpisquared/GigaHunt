namespace GigaHunt.Helpers
{
  public class FirstLastNameParser
  {
    public FirstLastNameParser(string emailAddress)
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
