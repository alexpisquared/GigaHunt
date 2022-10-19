namespace DB.QStats.Std.Models;

public partial class Email
{
  [NotMapped] public int? Ttl_Sent { get; set; }
  [NotMapped] public int? Ttl_Rcvd { get; set; }
  [NotMapped] public DateTime? LastSent { get; set; }
  [NotMapped] public DateTime? LastRcvd { get; set; }
}