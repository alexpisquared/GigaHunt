namespace DB.QStats.Std.Models;
public partial class QstatsRlsContext
{
  const string _dbg = """Server=.\SqlExpress;Database=QStatsDBG;Trusted_Connection=True;Encrypt=False;""";
  const string _rls = """Server=.\SqlExpress;Database=QStatsRLS;Trusted_Connection=True;Encrypt=False;""";
  readonly string _sqlConnectionString = _dbg; // legacy clients only.

  public QstatsRlsContext(string sqlConnectionString) => _sqlConnectionString = sqlConnectionString;
  ~QstatsRlsContext() => Trace.WriteLine($"@?@?@:> ~{nameof(QstatsRlsContext)}() called!   {_sqlConnectionString}");

  //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)     => optionsBuilder.UseSqlServer("Server=.\\SqlExpRess;Database=QStatsRls;Trusted_Connection=True;Encrypt=False;");

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      Trace.WriteLine($"@?@?@:> ~{nameof(OnConfiguring)}() called!   {_sqlConnectionString}");
      _ = optionsBuilder.UseSqlServer(_sqlConnectionString, sqlServerOptions => { _ = sqlServerOptions.CommandTimeout(150).EnableRetryOnFailure(10, TimeSpan.FromSeconds(44), null); });
      _ = optionsBuilder.EnableSensitiveDataLogging();  //todo: remove for production.
    }
  }

  [DbFunction(Name = "SOUNDEX", IsBuiltIn = true)] public static string SoundsLike(string sounds) => throw new NotImplementedException(); //tu: SOUNDEX

  public static QstatsRlsContext Create() => //todo: retire; used by old GigaHunter.
#if DEBUG
    new(_dbg); 
#else  
    new(_rls);
#endif
}
