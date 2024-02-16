namespace Emailing.NET6;

public partial class RegexHelper
{
  const string
    _regexPhonePattern = @"((\+|\+\s|\d{1}\s?|\()(\d\)?\s?[-\.\s\(]??){8,}\d{1}|\d{3}[-\.\s]??\d{3}[-\.\s]??\d{4}|\(\d{3}\)\s*\d{3}[-\.\s]??\d{4}|\d{3}[-\.\s]??\d{4})",
    _no7digitPhones = @"(?!(\d{7})$)((\+|\+\s|\d{1}\s?|\()(\d\)?\s?[-\.\s\(]??){8,}\d{1}|\d{3}[-\.\s]??\d{3}[-\.\s]??\d{4}|\(\d{3}\)\s*\d{3}[-\.\s]??\d{4}|\d{3}[-\.\s]??\d{4})", // :chatgpt
    _regexEmailPattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"; // //var r = new Regex(@"/\b[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}\b/");         \b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b  <== http://www.regular-expressions.info/email.html

  [GeneratedRegex(_regexEmailPattern, RegexOptions.IgnoreCase, "en-US")] private static partial Regex MyRegex();

  public static string? GetStringBetween(string b, string s1, string s2)
  {
    var i1 = b.IndexOf(s1);
    if (i1 < 0) return null;
    i1 += s1.Length;
    var i2 = b.IndexOf(s2, i1);
    if (i2 < 0) return null;
    if (i2 < i1) return null;
    var em = b[i1..i2];
    return em;
  }

  public static string[] FindEmails__OLD(string body)
  {
    var email = GetStringBetween(body, "Recipient(s):\n\t<", ">\n");
    email ??= GetStringBetween(body, "Recipient(s):\n\t", "\n");
    email ??= GetStringBetween(body, "To: ", "\n");
    email ??= GetStringBetween(body, "<", ">");
    //if (email == null) email = item.SentOnBehalfOfName;
    //if (email == null) continue;
    //if (!email.Contains("@-")) email = item.SenderEmailAddress;

    if (email == null || !email.Contains('@'))
    {
      var matches = MyRegex().Matches(body);
      if (matches.Count > 0)
      {
        var emails = new string[matches.Count];
        for (var i = 0; i < matches.Count; i++) emails[i] = matches[i].Value;
        return emails;
      }
    }

    if (email == null || !email.Contains('@')) return Array.Empty<string>();

    if (!email.Contains('@')) Write("");
    if (email.Contains('<')) email = email.Replace("<", "");
    if (email.Contains('>')) email = email.Replace(">", "");
    if (email.Contains(' ')) email = email.Split(' ')[0];
    if (email.Contains(':')) email = email.Split(':')[1];
    if (email.Contains('"')) email = email.Trim('"');

    email = email.Trim();

    return new string[] { email.Trim() };
  }
  public static string[] FindEmails(string body)
  {
    var matches = MyRegex().Matches(body);
    var emails = new string[matches.Count];
    for (var i = 0; i < matches.Count; i++) emails[i] = matches[i].Value;
    return emails;
  }

  public static void ShowUniqueValidBadPhoneNumbersFromLetter(Ehist ehist, int cur, int ttl, Stopwatch sw, HashSet<string> valids, HashSet<string> bads, string regexPattern = _regexPhonePattern)
  {
    ArgumentNullException.ThrowIfNull(ehist.LetterBody);

    var match = new Regex(regexPattern).Match(ehist.LetterBody);
    while (match.Success)
    {
      foreach (var pnRaw in match.Value.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
      {
        var pnInt = RemoveSpacers(pnRaw);

        Write($"{ehist.Id,5:N0} {ehist.LetterBody?.Length,8:N0}   {ehist.EmailId,40}   ");

        if (pnInt.Length < 10) Write($"< 10 ---");
        else if (pnInt.Length > 11) Write($"> 11 ---");
        else if (pnInt == pnRaw)
          if (
            pnInt.Length == 10 && (pnInt.StartsWith("416") || pnInt.StartsWith("647") || pnInt.StartsWith("905")) ||
            pnInt.Length == 11 && (pnInt.StartsWith("1416") || pnInt.StartsWith("1647") || pnInt.StartsWith("1905")))
          {
            Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write($"{cur,8:N0} / {ttl:N0}  {ehist.EmailId,56}  {pnRaw,16} {pnInt,11}   {(ttl - cur) * sw.Elapsed.TotalSeconds / cur,8:N1} sec left       {ehist.EmailedAt:yyyy-MM}  + + + \n");
          }
          else
          {
            var idx = ehist.LetterBody?.IndexOf(pnRaw) ?? -1;
            if (idx > 10)
            {
              var d = " :+\n\r";
              ArgumentNullException.ThrowIfNull(ehist.LetterBody);
              if (d.Contains(ehist.LetterBody.Substring(idx - 1, 1)))
              {
                Console.ForegroundColor = ConsoleColor.DarkGreen; Console.Write($"{cur,8:N0} / {ttl:N0}  {ehist.EmailId,56}  {pnRaw,16} {pnInt,11}   {(ttl - cur) * sw.Elapsed.TotalSeconds / cur,8:N1} sec left       {ehist.EmailedAt:yyyy-MM}     {ehist.LetterBody?.Substring(idx - 6, 6).Replace("\n", "\\n")}  +++++++++++++++++++\n");
              }
              else
              {

                if (pnInt.Length == 11 && pnInt[0] == '1') pnInt = pnInt[1..];
                if (bads.Add(pnInt))
                {
                  Console.ForegroundColor = ConsoleColor.Magenta; Console.Write($"{cur,8:N0} / {ttl:N0}  {ehist.EmailId,56}  {pnRaw,16} {pnInt,11}   {(ttl - cur) * sw.Elapsed.TotalSeconds / cur,8:N1} sec left       {ehist.EmailedAt:yyyy-MM}     {ehist.LetterBody?.Substring(idx - 16, 16)}  ■ ■ ■ ■ ■  Remove me from DB!!!\n");
                }
              }
            }
            else
            {
              Console.ForegroundColor = ConsoleColor.Magenta;
              Console.BackgroundColor = ConsoleColor.DarkBlue;
              Console.Write($"{cur,8:N0} / {ttl:N0}  {ehist.EmailId,56}  {pnRaw,16} {pnInt,11}   {(ttl - cur) * sw.Elapsed.TotalSeconds / cur,8:N1} sec left       {ehist.EmailedAt:yyyy-MM}  - - -\n");
              Console.ResetColor();
            }
          }
        else
        {
          if (pnInt.Length == 11 && pnInt[0] == '1') pnInt = pnInt[1..];
          if (valids.Add(pnInt))
          {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"{cur,8:N0} / {ttl:N0}  {ehist.EmailId,56}  {pnRaw,16} {pnInt,11}   {(ttl - cur) * sw.Elapsed.TotalSeconds / cur,8:N1} sec left       {ehist.EmailedAt:yyyy-MM}  ++ ++ ++\n");
          }
        }

        WriteLine($"    {pnInt}");
      }

      match = match.NextMatch();
    }
  }
  public static HashSet<string> GetUniquePhoneNumbersFromLetter(Ehist ehist, string regex = _regexPhonePattern)
  {
    Write($"\n");
    var valids = new HashSet<string>();
    ArgumentNullException.ThrowIfNull(ehist.LetterBody);
    var match = new Regex(regex).Match(ehist.LetterBody);
    while (match.Success)
    {
      foreach (var pnRaw in match.Value.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
      {
        var pnInt = RemoveSpacers(pnRaw);

        Write($" {pnRaw} ");

        if (pnInt.Length is < 10 or > 11) continue;
        if (pnInt.Length == 11 && pnInt[0] != '1') continue;

        Write($"\n");

        //todo: figure out what is that and below for: if (pnInt == pnRaw)
        {
          var offset = 15;
          var pnIdx = ehist.LetterBody?.IndexOf(pnRaw) ?? -1;
          if (pnIdx > offset) // JIC not at the very beginning of the letter body.
          {
            var alloweds = "\n: +\r";
            ArgumentNullException.ThrowIfNull(ehist.LetterBody);
            Write($" prefix: '..{ehist.LetterBody.Substring(pnIdx - offset, offset).Replace("\n", "\\n").Replace("\r", "\\r"),20}'   {(alloweds.Contains(ehist.LetterBody.Substring(pnIdx - 1, 1)) ? ":all good  (no action needed)" : "this is bad  ■ ■ ■ ■ ■  Remove me from DB!!! \\n\"")}");
          }
        }

        var pnX = pnInt.Length == 11 ? pnInt[1..] : pnInt;
        if (AreaCodeValidator.Any(pnX))
        {
          var scs = valids.Add(pnX);
          Write($"     {(scs ? "added OK +++" : "a dupe ---")}");
        }
      }

      match = match.NextMatch();
    }

    Write($"\n");

    return valids;
  }

  static string RemoveSpacers(string pnRaw) => pnRaw
            .Replace(" ", "")
            .Replace(" ", "")
            .Replace("+", "")
            .Replace("_", "")
            .Replace("-", "")
            .Replace(".", "")
            .Replace("(", "")
            .Replace(")", "");
  public static bool IsEmail(string txt) => System.Text.RegularExpressions.Regex.IsMatch(txt, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");
}