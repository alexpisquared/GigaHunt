using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DB.QStats.Std.Models
{
  public partial class QStatsRlsContext : DbContext
  {

    public static QStatsRlsContext Create() => new QStatsRlsContext();
    public string GetDbChangesReport(int v) => ("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
    public async Task<int> TrySaveReportAsync(string v) => 222222;
    public string GetDbChangesReport() => "@@@@@@@@@@@@";
  }
}
