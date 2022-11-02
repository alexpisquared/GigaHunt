﻿namespace DB.QStats.Std.Models;
public partial class QStatsRlsContext
{
  readonly string _sqlConnectionString;

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
}
