﻿namespace GigaHunt.View;
public partial class LeadManagerWindow : WpfUserControlLib.Base.WindowBase
{
  readonly CollectionViewSource _leadViewSource, _leadViewSourcL, _leadViewSourcE;
  readonly QStatsRlsContext _db = new();
  readonly int _thisCampaign;
  bool _abandonChanges = false;

  public LeadManagerWindow()
  {
    InitializeComponent(); themeSelector1.ThemeApplier = ApplyTheme;

    _thisCampaign = _db.Campaigns.Max(r => r.Id);

    _leadViewSource = (CollectionViewSource)FindResource("leadViewSource");
    _leadViewSourcL = (CollectionViewSource)FindResource("lkuLeadStatuViewSource");
    _leadViewSourcE = (CollectionViewSource)FindResource("eMailViewSource");
  }
  void onLoaded(object s, RoutedEventArgs e)
  {
    var sw = Stopwatch.StartNew();
    try
    {
      _db.Leads.Load();
      _db.LkuLeadStatuses.Load();
      _db.Emails.Load();

      filterLeads(tbFilter.Text, cbInclAll.IsChecked == true);
      _leadViewSourcL.Source = _db.LkuLeadStatuses.Local.ToBindingList();
      _leadViewSourcE.Source = _db.Emails.Local.ToBindingList();

      tbkTitle.Text = Title = string.Format("Total rows leadds {0} + emails {1} loaded in {2:N1}s.", _db.Leads.Local.Count(), _db.Emails.Local.Count(), sw.Elapsed.TotalSeconds);

      themeSelector1.SetCurThemeToMenu(Thm);
    }
    catch (Exception ex) { ex.Pop(); }

    dgLeads.Focus();
  }
  protected override void OnClosing(System.ComponentModel.CancelEventArgs ea)
  {
    base.OnClosing(ea);
    if (_abandonChanges)
      return;

    setProperValuesToNewRecords();

    Task.Run(async () => ea.Cancel = await AgentAdminnWindow.CheckAskToSaveDispose_CanditdteForGlobalRepltAsync(_db, true, saveAndUpdateMetadata)).Wait(); //tu: waiting await from synch!!!!!!!
  }
  void onQuit(object s, RoutedEventArgs e) { _abandonChanges = true; Close(); }
  async void onSave(object s, RoutedEventArgs e) => await saveAsync();
  void onAgentEmailSelectionChanged(object s, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.Count != 1) return;

    dynamic lead = _leadViewSource.View.CurrentItem;
    if (lead == null) return;

    //dynamic agent = ((DB.QStats.Std.Models.Email)(((object[])(e.AddedItems))[0]));

    var c = ((DB.QStats.Std.Models.Email)((object[])e.AddedItems)[0]).Company;
    if (!string.IsNullOrEmpty(c) && lead.Agency != c) (lead ?? throw new ArgumentNullException("@@@####@@@")).Agency = c;

    var n = ((DB.QStats.Std.Models.Email)((object[])e.AddedItems)[0]).Fname;
    if (!string.IsNullOrEmpty(n) && lead.AgentName != n) (lead ?? throw new ArgumentNullException("@@@####@@@")).AgentName = n;
  }
  void onFilter(object s, RoutedEventArgs e) => filterLeads(tbFilter.Text, cbInclAll.IsChecked == true);
  void onCloseTheLead(object s, RoutedEventArgs e)
  {
    WriteLine($"{((DB.QStats.Std.Models.Lead)dgLeads.SelectedItem).AgentName} _{((DB.QStats.Std.Models.Lead)dgLeads.SelectedItem).Status}" );

    ((DB.QStats.Std.Models.Lead)dgLeads.SelectedItem).Status = "Closed";

    WriteLine($"{((DB.QStats.Std.Models.Lead)dgLeads.SelectedItem).AgentName} _{((DB.QStats.Std.Models.Lead)dgLeads.SelectedItem).Status} \n" );
  }
  void onAddNewLead(object s, RoutedEventArgs e)
  {
    try
    {
      var txt = Clipboard.GetText();
      _db.Leads.Local.Add(new Lead { AddedAt = App.Now, CampaignId = _thisCampaign, Note = string.IsNullOrEmpty(txt) ? "\r\n\n\n\t New Lead" : txt });

      CollectionViewSource.GetDefaultView(dgLeads.ItemsSource).Refresh(); //tu: refresh bound datagrid

      _ = _leadViewSource.View.MoveCurrentToLast();
      if (dgLeads.SelectedItem != null)
        dgLeads.ScrollIntoView(dgLeads.SelectedItem);
    }
    catch (Exception ex) { ex.Pop(); }
  }
  void onInitNewItem(object s, InitializingNewItemEventArgs e)
  {
    try
    {
      _ = _leadViewSource.View.MoveCurrentToLast();
      if (dgLeads.SelectedItem != null)
        dgLeads.ScrollIntoView(dgLeads.SelectedItem);

      tbxNote.Focus();
    }
    catch (Exception ex) { ex.Pop(); }
  }

  void filterLeads(string filter, bool includeClosed = false)
  {
    _leadViewSource.Source = _db.Leads.Local.ToBindingList().Where(r =>
      (includeClosed || r.CampaignId == _thisCampaign && r.Status != "Closed" && r.Status != "Dead")
      &&
      (string.IsNullOrEmpty(filter) || r.AgentEmail != null && (r.AgentEmail.Id.Contains(filter) || r.AgentEmail.Fname?.Contains(filter) == true))
    ).OrderBy(r => r.AddedAt);

    //todo: changes the status to Active: 
    _ = _leadViewSource.View.MoveCurrentToLast();
    if (dgLeads.SelectedItem != null)
      dgLeads.ScrollIntoView(dgLeads.SelectedItem);
  }
  Task<string> saveAndUpdateMetadata() => AgentAdminnWindow.SaveAndUpdateMetadata(_db);
  async Task<string> saveAsync()
  {
    setProperValuesToNewRecords();

    var rowsSawed = await _db.TrySaveReportAsync("LeadManager.cs");
    tbkTitle.Text = Title = string.Format("{0} rows saved", rowsSawed);
    Thread.Sleep(111);
    return Title;
  }
  void setProperValuesToNewRecords()
  {
    foreach (var row in _db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
    {
      if (((Lead)row.Entity).AddedAt < GigaHunt.App.Now.AddYears(-99))
        ((Lead)row.Entity).AddedAt = GigaHunt.App.Now;

      ((Lead)row.Entity).CampaignId = _thisCampaign;
    }

    foreach (var row in _db.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
      ((Lead)row.Entity).ModifiedAt = GigaHunt.App.Now;
  }
  void OnClose(object s, RoutedEventArgs e) { Close(); Application.Current.Shutdown(); }
}
