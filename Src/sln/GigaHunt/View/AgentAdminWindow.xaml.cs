namespace AgentFastAdmin;

public partial class AgentAdminnWindow : WpfUserControlLib.Base.WindowBase
{
  readonly QStatsRlsContext _db = QStatsRlsContext.Create();
  readonly CollectionViewSource _cvsEmailsVwSrc;
  IEnumerable<string> _leadEmails, _leadCompns;
  List<string> _badEmails;
  bool _isLoaded = false;
  public AgentAdminnWindow()
  {
    InitializeComponent(); themeSelector1.ThemeApplier = ApplyTheme;
    //tbver.Text = $"Db: {QStatsRlsContext.DbNameOnly}        Ver: ???";
    //tbver.Text = $"Db: {_db.ServerDatabase()}        Ver: ???";
    _cvsEmailsVwSrc = (CollectionViewSource)FindResource("eMailViewSource");
    _ = tbFilter.Focus();
    DataContext = this;
  }
  public static readonly DependencyProperty SrchProperty = DependencyProperty.Register("Srch", typeof(string), typeof(AgentAdminnWindow), new PropertyMetadata("", SrchCallback)); public string Srch { get => (string)GetValue(SrchProperty); set => SetValue(SrchProperty, value); }
  static void SrchCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as AgentAdminnWindow)?.SrchFilter();
  public TimeSpan SrchFilter(int max = 10)
  {
    if (!_isLoaded)
      return TimeSpan.Zero;

    BPR.BeepShort();
    ctrlPnl.IsEnabled = false;
    var sw = Stopwatch.StartNew();
    var srchToLwr = Srch.ToLower();
    try
    {
      var emails = _db.Emails.Local.ToBindingList().Where(r =>
        (cbAll.IsChecked == true || (string.IsNullOrEmpty(r.PermBanReason) && !_badEmails.Contains(r.Id))) && //todo: PermBanReason == '' still treated as BANNED!!!  HAS BEEN FIXED !!! on Sep 29, 2019.
        (cbxLeadEmails.IsChecked != true || _leadEmails.Contains(r.Id)) &&
        (cbxLeadCompns.IsChecked != true || _leadCompns.Contains(r.Company)) &&
        (
          string.IsNullOrEmpty(srchToLwr) ||

            r.Id.ToLower().Contains(srchToLwr) ||
            (r.Company != null && r.Company.ToLower().Contains(srchToLwr)) ||
            (r.Fname != null && r.Fname.ToLower().Contains(srchToLwr)) ||
            (r.Lname != null && r.Lname.ToLower().Contains(srchToLwr)) ||
            (r.Notes != null && r.Notes.ToLower().Contains(srchToLwr))

        )
      ).OrderByDescending(r => r.AddedAt);

      foreach (var em in emails) { fillExtProp(em); if (--max < 0) break; }

      _cvsEmailsVwSrc.Source = emails;

      doInfoTimedFilter(sw, emails);
      sw.Stop();
    }
    catch (Exception ex) { ex.Pop(); }
    finally { ctrlPnl.IsEnabled = true; }

    return sw.Elapsed;
  }

  protected override async void OnClosing(System.ComponentModel.CancelEventArgs ea)
  {
    base.OnClosing(ea);

    ea.Cancel = await CheckAskToSaveDispose_CanditdteForGlobalRepltAsync(_db, true, saveAndUpdateMetadata); // sync did NOT save !!! Oct2019

    //todo: Async gives the error below:
    //      Task.Run(async () => ea.Cancel = await CheckAskToSaveDispose_CanditdteForGlobalRepltAsync(_db, true, saveAndUpdateMetadata)).Wait(); //tu: waiting await from synch!!!!!!!
    //      --Logging only:
    //      System.Windows.Data Error: 2 : Cannot find governing FrameworkElement or FrameworkContentElement for target element. BindingExpression:Path = EmailedAt; DataItem = null; target element is 'DataGridTextColumn'(HashCode = 20852350); target property is 'ToolTip'(type 'Object')
    //
    //19.08.06 16:01:56 > InvalidOperationException at C:\C\AAV\WpfUserControlLib\Extension\Ext.Exception.Pop.cs(11): Pop()
    //  - Collection was modified; enumeration operation may not execute.
    //
    //19.08.06 16:01:59 > CurrentDispatcherUnhandledException: s: Dispatcher. - One or more errors occurred.
    //-The calling thread must be STA, because many UI components require this.
  }
  public async void load()
  {
    _ = await CheckAskToSaveDispose_CanditdteForGlobalRepltAsync(_db, false, saveAndUpdateMetadata); // keep it for future misstreatments.
    //ctrlPnl.IsEnabled = false;

    //AAV.Sys.Helpers.BPR.Beep1of2();

    var lsw = Stopwatch.StartNew();
    try
    {
      await _db.Emails.OrderByDescending(r => r.AddedAt).OrderBy(r => r.Notes).LoadAsync(); /**/  WriteLine($">>> Loaded  Emails   {lsw.ElapsedMilliseconds,6:N0} ms");
      await _db.Ehists.OrderByDescending(r => r.EmailedAt).LoadAsync();                     /**/  WriteLine($">>> Loaded  Ehists   {lsw.ElapsedMilliseconds,6:N0} ms"); //tu: that seems to order results in the secondary table where there is no control of roder available. Jul-2019
      await _db.Leads.OrderByDescending(r => r.AddedAt).LoadAsync();                        /**/  WriteLine($">>> Loaded   Leads   {lsw.ElapsedMilliseconds,6:N0} ms");
      _leadEmails = _db.Leads.Local.Select(r => r.AgentEmailId ?? "").Distinct();           /**/  WriteLine($">>> Loaded  LeadEm   {lsw.ElapsedMilliseconds,6:N0} ms");
      _leadCompns = _db.Leads.Local.Select(r => r.Agency ?? "").Distinct();                 /**/  WriteLine($">>> Loaded  LeadCo   {lsw.ElapsedMilliseconds,6:N0} ms");
      _badEmails = await CreateCommand("Select Id from [dbo].[BadEmails]()", _db.Database.GetConnectionString() ?? "??");

      static async Task<List<string>> CreateCommand(string queryString, string connectionString)
      {
        List<string> rv = new();
        using (var connection = new SqlConnection(connectionString))
        {
          var command = new SqlCommand(queryString, connection);
          connection.Open();
          SqlDataReader reader = await command.ExecuteReaderAsync();
          while (reader.Read())
          {
            rv.Add(reader[0]?.ToString() ?? "??");
          }
        }

        return rv;
      }

      _isLoaded = true;
      var fsw = SrchFilter();                                                               /**/  WriteLine($">>> Loaded  Filter   {lsw.ElapsedMilliseconds,6:N0} ms  ({fsw.TotalMilliseconds:N0})");

      //tu: sub table  ...maybe: .Include(c => c.Ehists).Load();

      themeSelector1.SetCurThemeToMenu(Thm);
      App.SpeakAsync("Loaded.");
    }
    catch (Exception ex) { ex.Pop(); }
    finally { ctrlPnl.IsEnabled = true; BPR.Beep2of2(); _ = tbFilter.Focus(); }
  }
  void doInfoTimedFilter(Stopwatch sw, IOrderedEnumerable<Email> ems) => tbkTitle.Text = $"{(tbkTitle.ToolTip = $"Total agents/emails  {ems.Count():N0}   filtered in  {sw.Elapsed.TotalSeconds:N2} s.  \n{_db.GetDbChangesReport(8)}").ToString().Replace("\n", "")}";
  void doInfoPendingSave() => tbkTitle.Text = $"{(tbkTitle.ToolTip = $"  {_db.GetDbChangesReport(32)}").ToString().Replace("\n", "")}";
  void fillExtProp(Email em)
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

  Task<string> saveAndUpdateMetadata() => SaveAndUpdateMetadata(_db);
  public static async Task<string> SaveAndUpdateMetadata(QStatsRlsContext db)
  {
    BPR.Beep1of2();
    var now = GigaHunt.App.Now;

    while (true)
    {
      try
      {
        foreach (var row in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
        {
          if (row.Entity is Email) // sanity check
          {
            var agencyCompany = ((Email)row.Entity).Company;

            if (db.Agencies.FirstOrDefault(r => string.Compare(r.Id, agencyCompany, true) == 0) == null)
            {
              _ = db.Agencies.Add(new Agency { Id = agencyCompany, AddedAt = now });
            }

            ((Email)row.Entity).AddedAt = now;
            ((Email)row.Entity).NotifyPriority = 99;
          }
        }

        foreach (var row in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
        {
          if (row.Entity is Email) // sanity check
          {
            ((Email)row.Entity).ModifiedAt = now;
          }
        }

        var rowsSawed = await db.TrySaveReportAsync("OutlookToDb.cs");

        return $"{rowsSawed} rows saved";
      }
      catch (Exception ex)
      {
        //if (bKeepShowingMoveError)
        switch (MessageBox.Show(string.Format("Error in {0}.{1}():\n\n{2}\n\nRetry?\nYes - keep showing\nNo - skip showing\nCancel - Cancel operation", MethodInfo.GetCurrentMethod().DeclaringType.Name, MethodInfo.GetCurrentMethod().Name,
          ex.InnerException == null ? ex.Message :
          ex.InnerException.InnerException == null ? ex.InnerException.Message :
          ex.InnerException.InnerException.Message), "Exception ", MessageBoxButton.YesNoCancel, MessageBoxImage.Error))
        {
          case MessageBoxResult.Yes: break;
          //	case MessageBoxResult.No: bKeepShowingMoveError = false; break;
          case MessageBoxResult.Cancel: return "Cancelled";// tbkTitle.Text;
        }
      }
      finally { BPR.Beep2of2(); }

      return "*******------------********+++++++++++";
    }
  }
  public static async Task<bool> CheckAskToSaveDispose_CanditdteForGlobalRepltAsync(QStatsRlsContext dbx, bool dispose, Func<Task> savePlusMetadata)
  {
    try
    {
      if (dbx.ChangeTracker.Entries().Any(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
      {
        App.SpeakAsync($"Would you like to save the changes? \r\n\n{dbx.GetDbChangesReport()}");

        switch (MessageBox.Show($"Would you like to save the changes? \r\n\n{dbx.GetDbChangesReport()}", "Unsaved changes detected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
        {
          default:
          case MessageBoxResult.Yes: await savePlusMetadata(); if (dispose) dbx.Dispose(); return false;
          case MessageBoxResult.No: if (dispose) dbx.Dispose(); return false;
          case MessageBoxResult.Cancel: return true;
        }
      }
      else
        return false;
    }
    catch (Exception ex) { ex.Pop(); return true; }
  }
  [Obsolete("This one does not save if app is closing immediately")]
  public static bool CheckAskToSaveDispose_CanditdteForGlobalRepltSynch(QStatsRlsContext dbx, bool dispose, Func<Task> savePlusMetadata)
  {
    try
    {
      if (dbx.ChangeTracker.Entries().Any(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
      {
        App.SpeakAsync($"Would you like to save the changes? \r\n\n{dbx.GetDbChangesReport()}");

        switch (MessageBox.Show($"Would you like to save the changes? \r\n\n{dbx.GetDbChangesReport()}", "Unsaved changes detected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
        {
          default:
          case MessageBoxResult.Yes: _ = savePlusMetadata(); if (dispose) dbx.Dispose(); return false;
          case MessageBoxResult.No: if (dispose) dbx.Dispose(); return false;
          case MessageBoxResult.Cancel: return true;
        }
      }
      else
        return false;
    }
    catch (Exception ex) { ex.Pop(); return true; }
  }

  void onLoaded(object s, RoutedEventArgs e) => load();
  void onFilter(object s, RoutedEventArgs e) => SrchFilter();
  void onMoreHistCounts(object s, RoutedEventArgs e) => SrchFilter(50);
  void onPBR(object s, RoutedEventArgs e) { tbPbr.Text += $" {((Button)s).Tag} - {DateTime.Today:yyyy-MM-dd}. "; onNxt(s, e); }
  void onDNN(object s, RoutedEventArgs e)
  {
    var i = 0;
    var curCampaignID = _db.Campaigns.Max(r => r.Id);
    foreach (Email em in eMailDataGrid.SelectedItems)
    {
      em.DoNotNotifyOnAvailableForCampaignId = curCampaignID;
      em.ModifiedAt = App.Now;
      i++;
    }

    App.SpeakAsync($"{i} records updated");
    onNxt(s, e);
    doInfoPendingSave();
  }
  void onNxt(object s, RoutedEventArgs e) => _cvsEmailsVwSrc.View.MoveCurrentToNext();
  void onOLk(object s, RoutedEventArgs e)
  {
    BPR.BeepClk();
    if (_cvsEmailsVwSrc.View.CurrentItem is Email em)
      new OutlookHelper6().AddUpdateOutlookContact(em);
    else
      App.SpeakAsync("Unable to UpdateContactByEmail since current item is not email.");
  }
  void onDel(object s, RoutedEventArgs e)
  {
    App.SpeakAsync("Are you sure?");

    if (MessageBox.Show($"Deleting:\r\n\n{eMailDataGrid.SelectedItems.Count}", "Are you sure?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
    {
      try
      {
        BPR.BeepClk();
        foreach (Email em in eMailDataGrid.SelectedItems)
        {
          //foreach (var lead in em.Leads) _db.Leads.Local.Remove(lead); // if there were leads - DO NOT DELETE!!!

          collectionModified_ReEnumerate:
          foreach (var ehst in em.Ehists)
          {
            _ = _db.Ehists.Local.Remove(ehst);
            goto collectionModified_ReEnumerate;
          }

          _ = _db.Emails.Local.Remove(em);
        }

        CollectionViewSource.GetDefaultView(eMailDataGrid.ItemsSource).Refresh(); //tu: refresh bound datagrid
        doInfoPendingSave();
        App.SpeakAsync("Deleted the selected rows and all the foreign keyed records.");
      }
      catch (Exception ex) { ex.Pop(); }
    }
  }
  void onSelectnChgd(object s, SelectionChangedEventArgs e)
  {
    try
    {
      if (e.AddedItems.Count > 0)
      {
        if (e.AddedItems[0] as Email is not null)
          fillExtProp(e.AddedItems[0] as Email ?? throw new ArgumentNullException(nameof(e), "#########%%%%%%%%"));
      }

      //too often: doInfoPendingSave();
    }
    catch (NotSupportedException ex) { ex.Pop("Ignore"); }
    catch (Exception ex) { ex.Pop(); }
  }
  async void onSave(object s, RoutedEventArgs e) => tbkTitle.Text = await SaveAndUpdateMetadata(_db);
}
