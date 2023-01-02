using GenderApiLib;
using Microsoft.Extensions.Configuration;

namespace GigaHunt.View;
public partial class AgentAdminnWindow : WpfUserControlLib.Base.WindowBase
{
  static readonly DateTime Now = DateTime.Now;
  readonly QstatsRlsContext _db = QstatsRlsContext.Create();
  readonly CollectionViewSource _cvsEmailsVwSrc;
  IEnumerable<string>? _leadEmails, _leadCompns;
  List<string>? _badEmails;
  bool _isLoaded = false;
  public AgentAdminnWindow()
  {
    InitializeComponent(); themeSelector1.ThemeApplier = ApplyTheme; tbver.Text = DevOps.IsDbg ? @"DBG" : "rls";

    //tbver.Text = $"Db: {QstatsRlsContext.DbNameOnly}        Ver: ???";
    //tbver.Text = $"Db: {_db.ServerDatabase()}        Ver: ???";
    _cvsEmailsVwSrc = (CollectionViewSource)FindResource("eMailViewSource");
    _ = tbFilter.Focus();
    DataContext = this;
  }
  public static readonly DependencyProperty SrchProperty = DependencyProperty.Register("Srch", typeof(string), typeof(AgentAdminnWindow), new PropertyMetadata("", SrchCallback)); public string Srch { get => (string)GetValue(SrchProperty); set => SetValue(SrchProperty, value); }
  static void SrchCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as AgentAdminnWindow)?.SrchFilter();
  public TimeSpan SrchFilter(int topRows = 10)
  {
    if (!_isLoaded)
      return TimeSpan.Zero;

    BPR.BeepShort();
    ctrlPnl.IsEnabled = false;
    var sw = Stopwatch.StartNew();
    var srchToLwr = Srch.ToLower();
    try
    {
#if DEBUG // POC
      var qr0 = _db.Emails.Where(r => r.Id == Srch);
      var qr1 = _db.Emails.Where(r => QstatsRlsContext.SoundsLike(r.Id) == QstatsRlsContext.SoundsLike(Srch));
      WriteLine($">>>{Srch}: {qr0.Count()} < {qr1.Count()}");
      var qry = _db.Emails.Where(r => cbDEX.IsChecked == true ? QstatsRlsContext.SoundsLike(r.Id) == QstatsRlsContext.SoundsLike(Srch) : r.Id.ToLower().Contains(srchToLwr));
      var emc = qry.Count();
#endif

      var emails = _db.Emails.Local.ToBindingList().Where(r =>
        (cbAll.IsChecked == true || string.IsNullOrEmpty(r.PermBanReason) && _badEmails is not null && !_badEmails.Contains(r.Id)) && //todo: PermBanReason == '' still treated as BANNED!!!  HAS BEEN FIXED !!! on Sep 29, 2019.
        (cbxLeadEmails.IsChecked != true || _leadEmails?.Contains(r.Id) == true) &&
        (cbxLeadCompns.IsChecked != true || _leadCompns?.Contains(r.Company) == true) &&
        (
          string.IsNullOrEmpty(srchToLwr) ||

            r.Id.ToLower().Contains(srchToLwr) ||
            r.Company != null && r.Company.ToLower().Contains(srchToLwr) ||
            r.Fname != null && r.Fname.ToLower().Contains(srchToLwr) ||
            r.Lname != null && r.Lname.ToLower().Contains(srchToLwr) ||
            r.Notes != null && r.Notes.ToLower().Contains(srchToLwr)
        )
      ).OrderByDescending(r => r.AddedAt);

      foreach (var em in emails) { FillExtProp(em); if (--topRows < 0) break; }

      _cvsEmailsVwSrc.Source = emails;

      DoInfoTimedFilter(sw, emails);
      sw.Stop();
    }
    catch (Exception ex) { ex.Pop(); }
    finally { ctrlPnl.IsEnabled = true; }

    return sw.Elapsed;
  }

  protected override async void OnClosing(CancelEventArgs ea) { base.OnClosing(ea); ea.Cancel = await AgentAdminnWindowHelpers.CheckAskToSaveDispose_CanditdteForGlobalRepltAsync(_db, true, SaveAndUpdateMetadata); }
  public async void Load()
  {
    themeSelector1.SetCurThemeToMenu(Thm);

    _ = await AgentAdminnWindowHelpers.CheckAskToSaveDispose_CanditdteForGlobalRepltAsync(_db, false, SaveAndUpdateMetadata); // keep it for future misstreatments.
    ctrlPnl.IsEnabled = false;

    var lsw = Stopwatch.StartNew();
    try
    {
      //await _db.Emails.OrderByDescending(r => r.AddedAt).OrderBy(r => r.Notes).LoadAsync(); /**/  WriteLine($">>> Loaded  Emails   {lsw.ElapsedMilliseconds,6:N0} ms    {_db.Database.GetConnectionString()}");
      //await _db.Ehists.OrderByDescending(r => r.EmailedAt).LoadAsync();                     /**/  WriteLine($">>> Loaded  Ehists   {lsw.ElapsedMilliseconds,6:N0} ms"); //tu: that seems to order results in the secondary table where there is no control of roder available. Jul-2019
      await _db.Emails.LoadAsync();                                                         /**/  WriteLine($">>> Loaded  Emails   {lsw.ElapsedMilliseconds,6:N0} ms    {_db.Database.GetConnectionString()}");
      await _db.Ehists.LoadAsync();                                                         /**/  WriteLine($">>> Loaded  Ehists   {lsw.ElapsedMilliseconds,6:N0} ms"); //tu: that seems to order results in the secondary table where there is no control of roder available. Jul-2019
      await _db.Leads.OrderByDescending(r => r.AddedAt).LoadAsync();                        /**/  WriteLine($">>> Loaded   Leads   {lsw.ElapsedMilliseconds,6:N0} ms");
      _leadEmails = _db.Leads.Local.Select(r => r.AgentEmailId ?? "").Distinct();           /**/  WriteLine($">>> Loaded  LeadEm   {lsw.ElapsedMilliseconds,6:N0} ms");
      _leadCompns = _db.Leads.Local.Select(r => r.Agency ?? "").Distinct();                 /**/  WriteLine($">>> Loaded  LeadCo   {lsw.ElapsedMilliseconds,6:N0} ms");
      _badEmails = await DB.QStats.Std.Models.MiscEfDb.GetBadEmails("Select Id from [dbo].[BadEmails]()", _db.Database.GetConnectionString() ?? "??");
      _isLoaded = true;
      var fsw = SrchFilter();                                                               /**/  WriteLine($">>> Loaded  Filter   {lsw.ElapsedMilliseconds,6:N0} ms  ({fsw.TotalMilliseconds:N0})");
    }
    catch (Exception ex) { ex.Pop(); }
    finally { ctrlPnl.IsEnabled = true; _ = tbFilter.Focus(); }
  }
  void DoInfoTimedFilter(Stopwatch sw, IOrderedEnumerable<Email> ems) => tbkTitle.Text = $"{(tbkTitle.ToolTip = $"Total agents/emails  {ems.Count():N0}   filtered in  {sw.Elapsed.TotalSeconds:N2} s.  \n{_db.GetDbChangesReport(8)}").ToString()?.Replace("\n", "")}";
  void DoInfoPendingSave() => tbkTitle.Text = $"{(tbkTitle.ToolTip = $"  {_db.GetDbChangesReport(32)}").ToString()?.Replace("\n", "")}";


  static void FillExtProp(Email em)
  {
    var sends = em.Ehists.Where(r => r.RecivedOrSent == "S");
    var rcvds = em.Ehists.Where(r => r.RecivedOrSent == "R");

    if (sends.Any())
    {
      em.Ttl_Sent = sends.Count();
      em.LastSent = sends.Max(r => r.EmailedAt);
    }

    if (rcvds.Any())
    {
      em.Ttl_Rcvd = rcvds.Count();
      em.LastRcvd = rcvds.Max(r => r.EmailedAt);
    }
  }

  Task<string> SaveAndUpdateMetadata() => AgentAdminnWindowHelpers.SaveAndUpdateMetadata(_db);

  void OnLoaded(object s, RoutedEventArgs e) => Load();
  void OnFilter(object s, RoutedEventArgs e) => SrchFilter();
  void OnMoreHistCounts(object s, RoutedEventArgs e) => SrchFilter(50);
  void OnPBR(object s, RoutedEventArgs e) { tbPbr.Text += $" {((Button)s).Tag} - {DateTime.Today:yyyy-MM-dd}. "; OnNxt(s, e); }
  void OnDNN(object s, RoutedEventArgs e)
  {
    var i = 0;
    var curCampaignID = _db.Campaigns.Max(r => r.Id);
    foreach (Email em in eMailDataGrid.SelectedItems)
    {
      em.DoNotNotifyOnAvailableForCampaignId = curCampaignID;
      em.ModifiedAt = Now;
      i++;
    }

    App.Speak($"{i} records updated");
    OnNxt(s, e);
    DoInfoPendingSave();
  }
  void OnNxt(object s, RoutedEventArgs e) => _cvsEmailsVwSrc.View.MoveCurrentToNext();
  void OnOLk(object s, RoutedEventArgs e)
  {
    BPR.BeepClk();
    if (_cvsEmailsVwSrc.View.CurrentItem is Email em)
      new OutlookHelper6().AddUpdateOutlookContact(em);
    else
      App.Speak("Unable to UpdateContactByEmail since current item is not email.");
  }
  void OnSelectnChgd(object s, SelectionChangedEventArgs e)
  {
    try
    {
      if (e.AddedItems.Count > 0)
        if (e.AddedItems[0] as Email is not null)
          FillExtProp(e.AddedItems[0] as Email ?? throw new ArgumentNullException(nameof(e), "#########%%%%%%%%"));

      //too often: doInfoPendingSave();
    }
    catch (NotSupportedException ex) { ex.Pop("Ignore"); }
    catch (Exception ex) { ex.Pop(); }
  }
  async void OnSave(object s, RoutedEventArgs e) => tbkTitle.Text = await AgentAdminnWindowHelpers.SaveAndUpdateMetadata(_db);
  void OnDel(object s, RoutedEventArgs e)
  {
    App.Speak("Are you sure?");
    if (MessageBox.Show($"Deleting:\r\n\n{eMailDataGrid.SelectedItems.Count}", "Are you sure?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
      try
      {
        BPR.BeepClk();
        foreach (Email em in eMailDataGrid.SelectedItems)
        {
          //foreach (var lead in em.Leads) _db.Leads.Local.Remove(lead); // if there were leads - DO NOT DELETE!!!

          collectionModified_ReEnumerate:
          foreach (var ehst in em.Ehists)
          {
            _ = _db.Ehists.Remove(ehst);
            goto collectionModified_ReEnumerate;
          }

          _ = _db.Emails.Local.Remove(em);
        }

        CollectionViewSource.GetDefaultView(eMailDataGrid.ItemsSource).Refresh(); //tu: refresh bound datagrid
        DoInfoPendingSave();
        App.Speak("Deleted the selected rows and all the foreign keyed records.");
      }
      catch (Exception ex) { ex.Pop(); }
  }
  async void OnCou(object s, RoutedEventArgs e)
  {
    try
    {
      BPR.BeepClk();
      var si = eMailDataGrid.SelectedIndex;
      foreach (Email em in eMailDataGrid.SelectedItems)
        if (em.Fname is not null)
        {
          var (ts, _, root) = await GenderApi.CallOpenAI(new ConfigurationBuilder().AddUserSecrets<App>().Build(), em.Fname);
          em.Country = root?.country_of_origin.FirstOrDefault()?.country_name ?? "??";
        }

      CollectionViewSource.GetDefaultView(eMailDataGrid.ItemsSource).Refresh(); //tu: refresh bound datagrid
      DoInfoPendingSave();
      eMailDataGrid.SelectedIndex = si;

      //BPR___.Speak("Deleted the selected rows and all the foreign keyed records.");
    }
    catch (Exception ex) { ex.Pop(); }
  }

  async void OnClose(object s, RoutedEventArgs e) { Close(); await Task.Delay(11000); Application.Current.Shutdown(); }
}