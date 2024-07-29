namespace Emailing.NET6.Helpers;

internal static class EmailerHelpers
{
  public static string GetMicrosoftAccountName() //todo: move it to a proper place.
  {
    using var wi = WindowsIdentity.GetCurrent();

    ArgumentNullException.ThrowIfNull(wi.Groups);
    var groups = from g in wi.Groups
                 select new SecurityIdentifier(g.Value)
                 .Translate(typeof(NTAccount)).Value;

    var msAccount = (from g in groups
                     where g.StartsWith(@"MicrosoftAccount\")
                     select g).FirstOrDefault();

    return msAccount == null ? wi.Name : msAccount[@"MicrosoftAccount\".Length..];
  }
}