namespace DB.QStats.Std.Models;

public partial class Email : INotifyPropertyChanged
{
  [NotMapped] public int? Ttl_Sent { get => ttl_Sent; set { if (value != ttl_Sent) { ttl_Sent = value; OnPropertyChanged(); } } }
  [NotMapped] public int? Ttl_Rcvd { get => ttl_Rcvd; set { if (value != ttl_Rcvd) { ttl_Rcvd = value; OnPropertyChanged(); } } }
  [NotMapped] public DateTime? LastSent { get => lastSent; set { if (value != lastSent) { lastSent = value; OnPropertyChanged(); } } }
  [NotMapped] public DateTime? LastRcvd { get => lastRcvd; set { if (value != lastRcvd) { lastRcvd = value; OnPropertyChanged(); } } }
  //public string? Country { get => _country; set { if (value != _country) { _country = value; OnPropertyChanged(); } } } // now it is real db field (2023-11)

  string? _country;
  int? ttl_Sent;
  int? ttl_Rcvd;
  DateTime? lastSent;
  DateTime? lastRcvd;

  //[ObservableProperty] string? country;

  public event PropertyChangedEventHandler? PropertyChanged;
  protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
