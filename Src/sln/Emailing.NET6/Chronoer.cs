namespace GigaHunt.AsLink;

public class Chronoer { static DateTime? _now = null; public static DateTime Now  => _now ?? (_now = DateTime.Now).Value; } 