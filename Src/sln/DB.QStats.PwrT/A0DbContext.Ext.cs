namespace DB.QStats.PwrT.Models;

public partial class Lead //: ObservableObject
{
  [NotMapped] public double? RealRate => HourlyRate * HourPerDay;
}
public partial class Email //: ObservableObject //, INotifyPropertyChanged
{
  //[ObservableProperty] string? country;               //todo: datagrid column does not get updated until out/in viewport??? Inheriting ObservableObject || INotifyPropertyChanged did not help. Updating the selected row outside the grid WORKS!>!>!>!>?!?!?!?
  [NotMapped] public int? Ttl_Sent { get; set; }        //todo: datagrid column does not get updated until out/in viewport??? Inheriting ObservableObject || INotifyPropertyChanged did not help. Updating the selected row outside the grid WORKS!>!>!>!>?!?!?!?
  [NotMapped] public int? Ttl_Rcvd { get; set; }        //todo: datagrid column does not get updated until out/in viewport??? Inheriting ObservableObject || INotifyPropertyChanged did not help. Updating the selected row outside the grid WORKS!>!>!>!>?!?!?!?
  [NotMapped] public DateTime? LastSent { get; set; }   //todo: datagrid column does not get updated until out/in viewport??? Inheriting ObservableObject || INotifyPropertyChanged did not help. Updating the selected row outside the grid WORKS!>!>!>!>?!?!?!?
  [NotMapped] public DateTime? LastRcvd { get; set; }   //todo: datagrid column does not get updated until out/in viewport??? Inheriting ObservableObject || INotifyPropertyChanged did not help. Updating the selected row outside the grid WORKS!>!>!>!>?!?!?!?

  /*
    [NotMapped] public int? Ttl_Sent { get => ttl_Sent; set { if (value != ttl_Sent) { ttl_Sent = value; OnPropertyChanged(); } } }
    [NotMapped] public int? Ttl_Rcvd { get => ttl_Rcvd; set { if (value != ttl_Rcvd) { ttl_Rcvd = value; OnPropertyChanged(); } } }
    [NotMapped] public DateTime? LastSent { get => lastSent; set { if (value != lastSent) { lastSent = value; OnPropertyChanged(); } } }
    [NotMapped] public DateTime? LastRcvd { get => lastRcvd; set { if (value != lastRcvd) { lastRcvd = value; OnPropertyChanged(); } } }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  */
}

public class MiscEfDb
{
  public static async Task<List<string>> GetBadEmails(string queryString, string connectionString)
  {
    List<string> rv = [];
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
