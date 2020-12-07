using AAV.WPF.Ext;
using AgentFastAdmin;
using AsLink;
using Db.QStats.DbModel;
using OutlookToDbWpfApp;
using System;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AvailStatusEmailer
{
  public partial class LeadManagerWindow : AAV.WPF.Base.WindowBase
  {
    readonly CollectionViewSource _leadViewSource, _leadViewSourcL, _leadViewSourcE;
    readonly A0DbContext _db = A0DbContext.Create();
    readonly int _thisCampaign;
    bool _abandonChanges = false;

    public LeadManagerWindow()
    {
      InitializeComponent(); themeSelector1.ApplyTheme = ApplyTheme;

      _thisCampaign = _db.Campaigns.Max(r => r.Id);

      _leadViewSource = ((CollectionViewSource)(FindResource("leadViewSource")));
      _leadViewSourcL = ((CollectionViewSource)(FindResource("lkuLeadStatuViewSource")));
      _leadViewSourcE = ((CollectionViewSource)(FindResource("eMailViewSource")));      
    }
    void onLoaded(object s, RoutedEventArgs e)
    {
      var sw = Stopwatch.StartNew();
      try
      {
        _db.Leads.Load();
        _db.lkuLeadStatus.Load();
        _db.EMails.Load();

        filterLeads(tbFilter.Text, cbInclAll.IsChecked == true);
        _leadViewSourcL.Source = _db.lkuLeadStatus.Local;
        _leadViewSourcE.Source = _db.EMails.Local;

        tbkTitle.Text = Title = string.Format("Total rows leadds {0} + emails {1} loaded in {2:N1}s.", _db.Leads.Local.Count(), _db.EMails.Local.Count(), sw.Elapsed.TotalSeconds);

        themeSelector1.SetCurTheme(Thm);
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
    void onAgentEmailSelectionChanged(object s, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count != 1) return;

      dynamic lead = _leadViewSource.View.CurrentItem;
      if (lead == null) return;

      //dynamic agent = ((Db.QStats.DbModel.EMail)(((object[])(e.AddedItems))[0]));

      var c = ((Db.QStats.DbModel.EMail)(((object[])(e.AddedItems))[0])).Company;
      if (!string.IsNullOrEmpty(c) && lead?.Agency != c) lead.Agency = c;

      var n = ((Db.QStats.DbModel.EMail)(((object[])(e.AddedItems))[0])).FName;
      if (!string.IsNullOrEmpty(n) && lead.AgentName != n) lead.AgentName = n;
    }
    void onFilter(object s, System.Windows.RoutedEventArgs e) => filterLeads(tbFilter.Text, cbInclAll.IsChecked == true);
    void onCloseTheLead(object s, RoutedEventArgs e)
    {
      Debug.WriteLine("{0} - {1}", ((Db.QStats.DbModel.Lead)(dgLeads.SelectedItem)).AgentName, ((Db.QStats.DbModel.Lead)(dgLeads.SelectedItem)).Status);

      ((Db.QStats.DbModel.Lead)(dgLeads.SelectedItem)).Status = "Closed";

      Debug.WriteLine("{0} - {1} \n", ((Db.QStats.DbModel.Lead)(dgLeads.SelectedItem)).AgentName, ((Db.QStats.DbModel.Lead)(dgLeads.SelectedItem)).Status);
    }
    void onAddNewLead(object s, RoutedEventArgs e)
    {
      try
      {
        var txt = Clipboard.GetText();
        _db.Leads.Local.Add(new Lead { AddedAt = App.Now, CampaignId = _thisCampaign, Note = string.IsNullOrEmpty(txt) ? "\r\n\n\n\t New Lead" : txt });

        CollectionViewSource.GetDefaultView(dgLeads.ItemsSource).Refresh(); //tu: refresh bound datagrid

        _leadViewSource.View.MoveCurrentToLast();
        if (dgLeads.SelectedItem != null)
          dgLeads.ScrollIntoView(dgLeads.SelectedItem);
      }
      catch (Exception ex) { ex.Pop(); }
    }
    void onInitNewItem(object s, InitializingNewItemEventArgs e)
    {
      try
      {
        _leadViewSource.View.MoveCurrentToLast();
        if (dgLeads.SelectedItem != null)
          dgLeads.ScrollIntoView(dgLeads.SelectedItem);

        tbxNote.Focus();
      }
      catch (Exception ex) { ex.Pop(); }
    }

    void filterLeads(string filter, bool includeClosed = false)
    {
      _leadViewSource.Source = _db.Leads.Local.Where(r =>
        (includeClosed || (r.CampaignId == _thisCampaign && r.Status != "Closed" && r.Status != "Dead"))
        &&
        (string.IsNullOrEmpty(filter) || (r.EMail != null && (r.EMail.ID.Contains(filter) || r.EMail.FName.Contains(filter))))
      ).OrderBy(r => r.AddedAt);

      //todo: changes the status to Active: 
      _leadViewSource.View.MoveCurrentToLast();
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
        if (((Lead)(row.Entity)).AddedAt < AvailStatusEmailer.App.Now.AddYears(-99))
          ((Lead)(row.Entity)).AddedAt = AvailStatusEmailer.App.Now;

        ((Lead)(row.Entity)).CampaignId = _thisCampaign;
      }

      foreach (var row in _db.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
      {
        ((Lead)(row.Entity)).ModifiedAt = AvailStatusEmailer.App.Now;
      }
    }
  }
}
