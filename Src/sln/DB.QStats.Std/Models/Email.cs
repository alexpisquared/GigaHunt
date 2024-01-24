namespace DB.QStats.Std.Models;

public partial class Email : ObservableValidator
{
  public string Id { get; set; } = null!;

  public string? Fname { get; set; }

  public string? Lname { get; set; }

  public string? Company { get; set; }

  [ObservableProperty] string? country; // poc 2024-01-24 //todo: datagrid column does not get updated until out/in viewport???

  public string? Phone { get; set; }

  public string? PermBanReason { get; set; }

  public int? DoNotNotifyOnAvailableForCampaignId { get; set; }

  public int? DoNotNotifyOnOffMarketForCampaignId { get; set; }

  public string? Notes { get; set; }

  public int NotifyPriority { get; set; }

  public DateTime? ReSendAfter { get; set; }

  public DateTime? LastAction { get; set; }

  public DateTime AddedAt { get; set; }

  public DateTime? ModifiedAt { get; set; }

  public virtual Agency? CompanyNavigation { get; set; }

  public virtual Campaign? DoNotNotifyOnOffMarketForCampaign { get; set; }

  public virtual ICollection<Ehist> Ehists { get; } = [];

  public virtual ICollection<LeadEmail> LeadEmails { get; } = [];

  public virtual ICollection<Lead> Leads { get; } = [];

  public virtual ICollection<PhoneEmailXref> PhoneEmailXrefs { get; } = [];
}
