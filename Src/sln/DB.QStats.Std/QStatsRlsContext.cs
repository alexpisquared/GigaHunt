namespace DB.QStats.Std.Models;
public partial class QStatsRlsContext : DbContext
{
  public static QStatsRlsContext Create() => new();
  public string GetDbChangesReport(int t = 0) => $"todo {t}";
  public async Task<int> TrySaveReportAsync(string v) => 222222;
}
