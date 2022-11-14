using Microsoft.Data.SqlClient;

namespace DB.QStats.Std.Models;

public partial class Email
{
  [NotMapped] public int? Ttl_Sent { get; set; }
  [NotMapped] public int? Ttl_Rcvd { get; set; }
  [NotMapped] public DateTime? LastSent { get; set; }
  [NotMapped] public DateTime? LastRcvd { get; set; }
  [NotMapped] public string? Country { get; set; }
}

public class MiscEfDb
{
  public static async Task<List<string>> GetBadEmails(string queryString, string connectionString)
  {
    List<string> rv = new();
    using (var connection = new SqlConnection(connectionString))
    {
      var command = new SqlCommand(queryString, connection);
      connection.Open();
      var reader = await command.ExecuteReaderAsync();
      while (reader.Read())
        rv.Add(reader[0]?.ToString() ?? "??");
    }

    return rv;
  }

}
