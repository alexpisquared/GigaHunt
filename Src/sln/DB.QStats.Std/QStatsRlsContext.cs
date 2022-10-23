namespace DB.QStats.Std.Models;
public partial class QStatsRlsContext
{
  readonly string _sqlConnectionString = "<Not Initialized!!!>";//todo: if not done: remove warnig and ... in protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)  #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.

  public QStatsRlsContext(string connectoinString) => _sqlConnectionString = connectoinString;

  ~QStatsRlsContext() => Trace.WriteLine($"@?@?@:> ~InventoryContext() called!");

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      //      _ = optionsBuilder.UseSqlServer(_sqlConnectionString, sqlServerOptions => { _ = sqlServerOptions.CommandTimeout(150).EnableRetryOnFailure(10, TimeSpan.FromSeconds(44), null); });

#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
#if DEBUG
      optionsBuilder.UseSqlServer("Server=.\\SqlExpRess;Database=QStatsDbg;Trusted_Connection=True;");
#else
      optionsBuilder.UseSqlServer("Server=.\\SqlExpRess;Database=QStatsRls;Trusted_Connection=True;");
#endif

      _ = optionsBuilder.EnableSensitiveDataLogging();  //todo: remove for production.
    }
  }
  public static QStatsRlsContext Create() => new();
}
