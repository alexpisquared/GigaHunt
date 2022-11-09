namespace DB.QStats.Std.Models;
public partial class QStatsRlsContext
{
  const string _dbg = @"Server=.\SqlExpress;Database=QStatsDBG;Trusted_Connection=True;Encrypt=False;";
  const string _rls = @"Server=.\SqlExpress;Database=QStatsRLS;Trusted_Connection=True;Encrypt=False;";
  readonly string _sqlConnectionString = _dbg; // legacy clients only.

  public QStatsRlsContext(string sqlConnectionString) => _sqlConnectionString = sqlConnectionString;
  ~QStatsRlsContext() => Trace.WriteLine($"@?@?@:> ~{nameof(QStatsRlsContext)}() called!");

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      _ = optionsBuilder.UseSqlServer(_sqlConnectionString, sqlServerOptions => { _ = sqlServerOptions.CommandTimeout(150).EnableRetryOnFailure(10, TimeSpan.FromSeconds(44), null); });
      _ = optionsBuilder.EnableSensitiveDataLogging();  //todo: remove for production.
    }
  }

  public static QStatsRlsContext Create() => //todo: retire; used by old GigaHunter.
#if DEBUG
    new(_dbg); 
#else  
    new(_rls); 
#endif
}
