using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using AAV.Sys.Helpers;
using AAV.WPF.Ext;
using AgentFastAdmin;
using AsLink;
using AvailStatusEmailer.View;
using Db.QStats.DbModel;
using OutlookToDbWpfApp;

namespace AvailStatusEmailer
{
  public partial class EmailersendWindow : AAV.WPF.Base.WindowBase
  {
    const double _fractionToSend = .025, _absoluteMax = 25;
    A0DbContext _db;
    CollectionViewSource _cvsEmails;
    ObservableCollection<vEMail_Avail_Prod> _obsColAvlbl;
    IEnumerable<string> _leadEmails, _leadCompns;
    string _firstName = "Sirs";

    public EmailersendWindow()
    {
      InitializeComponent(); themeSelector1.ApplyTheme = ApplyTheme;
      tbver.Text = $"Db: {A0DbContext.DbNameOnly}        Ver: {AAV.Sys.Helpers.VerHelper.CurVerStr(".net 4.8")}";
      _ = tbFilter.Focus();

      Loaded += async (s, e) => { await Task.Yield(); themeSelector1.SetCurTheme(Thm); Bpr.BeepBgn3(); };
    }
    async Task<string> reLoad()
    {
      var lswTtl = Stopwatch.StartNew();
      var lsw = Stopwatch.StartNew();
      try
      {
        _db = A0DbContext.Create();                                                   /**/  Debug.WriteLine($">>> A0DbContext.Create(){lsw.ElapsedMilliseconds,6:N0} ms"); lsw = Stopwatch.StartNew();
        tbver.Text = $"Db: {_db.ServerDatabase()}        Ver: {AAV.Sys.Helpers.VerHelper.CurVerStr(".net 4.8")}";
        if (chkIsAvailable.IsChecked == true)
        {
          await _db.vEMail_Avail_Prod.LoadAsync();                                    /**/  Debug.WriteLine($">>>    Loaded   EmlVw   {lsw.ElapsedMilliseconds,6:N0} ms"); lsw = Stopwatch.StartNew();
          await _db.Leads.OrderByDescending(r => r.AddedAt).LoadAsync();              /**/  Debug.WriteLine($">>>    Loaded   Leads   {lsw.ElapsedMilliseconds,6:N0} ms"); lsw = Stopwatch.StartNew();
          _leadEmails = _db.Leads.Local.Select(r => r.AgentEmailId).Distinct();       /**/  Debug.WriteLine($">>>    Loaded  LeadEm   {lsw.ElapsedMilliseconds,6:N0} ms"); lsw = Stopwatch.StartNew();
          _leadCompns = _db.Leads.Local.Select(r => r.Agency).Distinct();             /**/  Debug.WriteLine($">>>    Loaded  LeadCo   {lsw.ElapsedMilliseconds,6:N0} ms"); lsw = Stopwatch.StartNew();
          _obsColAvlbl = _db.vEMail_Avail_Prod.Local;

          _cvsEmails = (CollectionViewSource)FindResource("vsEMail_Avail");
          _cvsEmails.Source = null;
          populateWithSorting(_obsColAvlbl);
        }
        else
        {
          await _db.vEMail_UnAvl_Prod.OrderByDescending(r => r.AddedAt).LoadAsync();

          _cvsEmails = (CollectionViewSource)FindResource("vsEMail_UnAvl");
          _cvsEmails.Source = null;
          _cvsEmails.Source = _db.vEMail_UnAvl_Prod.Local.OrderByDescending(r => r.AddedAt);
        }

        var ttl = chkIsAvailable.IsChecked == true ? _obsColAvlbl.Count : _db.vEMail_UnAvl_Prod.Local.Count;

        btMax.Content = $"Top {tbMax.Text = $"{(int)Math.Min(ttl * _fractionToSend, _absoluteMax)}"} rows";
        return string.Format("Total {0} unused records loaded in {1:N1}", ttl, lswTtl.Elapsed.TotalSeconds);
      }
      catch (Exception ex) { ex.Pop(); return ex.Message; }
    }
    void filter()
    {
      var sw = Stopwatch.StartNew();
      var srchToLwr = tbFilter.Text.ToLower();
      try
      {
        IEnumerable<vEMail_Avail_Prod> rv;

        if (cbx_ALL_Emails.IsChecked == true)
        {
          var ls = new List<vEMail_Avail_Prod>();

          foreach (var r in _db.EMails.Where(r => r.ID.Contains(srchToLwr)))
          {
            ls.Add(new vEMail_Avail_Prod
            {
              ID = r.ID,
              Company = r.Company,
              FName = r.FName,
              LName = r.LName,
              Notes = r.Notes,
              DoNotNotifyForCampaignID = r.DoNotNotifyOnAvailableForCampaignID,
              LastSentAt = r.LastSent,
              LastRepliedAt = r.LastRcvd,
              AddedAt = r.AddedAt,
              PermBanReason = r.PermBanReason
            });
          }

          rv = ls;
        }
        else
        {
          rv = _obsColAvlbl.Where(r =>
                      (cbxLeadEmails.IsChecked != true || _leadEmails.Contains(r.ID)) &&
                      (cbxLeadCompns.IsChecked != true || _leadCompns.Contains(r.Company)) &&
                      (
                        string.IsNullOrEmpty(srchToLwr) ||

                          r.ID.ToLower().Contains(srchToLwr) ||
                          (r.Company != null && r.Company.ToLower().Contains(srchToLwr)) ||
                          (r.FName != null && r.FName.ToLower().Contains(srchToLwr)) ||
                          (r.LName != null && r.LName.ToLower().Contains(srchToLwr)) ||
                          (r.Notes != null && r.Notes.ToLower().Contains(srchToLwr))

                      )
                    );

        }

        populateWithSorting(rv);
        tbkTitle.Text = $"Total agents/emails  {rv.Count()} / {_db.EHists.Local.Count()}  filtered in  {sw.Elapsed.TotalSeconds:N2} s.  {_db.GetDbChangesReport(3).Replace("\n", "")}";
        sw.Stop();
      }
      catch (Exception ex) { ex.Pop(); }
    }
    void populateWithSorting(IEnumerable<vEMail_Avail_Prod> rv) => _cvsEmails.Source = rv.OrderBy(r => r.LastSentAt).ThenBy(r => r.AddedAt);// <= for restarting the failed campaingn | for starting brand new campagn after a contract => .OrderByDescending(r => r.TtlSends).ThenByDescending(r => r.TtlRcvds); 

    void Save()
    {
      try
      {
        // When you delete an object from the related entities collection (in this case Products), the Entity Framework doesn’t mark these child entities as deleted.
        // Instead, it removes the relationship between the parent and the child by setting the parent reference to null.
        // So we manually have to delete the products that have a Category reference set to null.

        // The following code uses LINQ to Objects against the Local collection of Products.
        // The ToList call is required because otherwise the collection will be modified by the Remove call while it is being enumerated.
        // In most other situations you can use LINQ to Objects directly against the Local property without using ToList first.
        //foreach (var product in _context.vEMail_Avail_Prod.Local.ToList())			{				//if (product.Category == null)_context.vEMail_Avail_Prod.Remove(product);			}

        //tbkTitle.Text = Title =   _context.SaveChanges().ToString() + " rows saved";
      }
      catch (Exception ex) { ex.Pop(); }
    }
    void ReFresh()
    {
      try
      {
        //old:
        vEMail_Avail_DevDataGrid.Items.Refresh();// Refresh the grids so the database generated values show up.		
        vEMail_Avail_DevDataGrid.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();

        //new:
        CollectionViewSource.GetDefaultView(vEMail_Avail_DevDataGrid.ItemsSource).Refresh(); //tu: refresh bound datagrid
      }
      catch (Exception ex) { ex.Pop(); }
    }
    void EnableControls(bool b) => ZommablePanel.IsEnabled = ctrlPanelOnMarket.IsEnabled = ctrlPanelOffMarket.IsEnabled = b;
    async void onLoaded(object s, RoutedEventArgs e)
    {
      tbkTitle.Text = Title = await reLoad();

      if (Environment.GetCommandLineArgs().Length > 1 && Environment.GetCommandLineArgs()[1] == "Broad")
      {
        btnBroadcastTopN_Click(s, e);
      }

      if (!Clipboard.ContainsText()) return;

      try
      {
        /*        
                var text = Clipboard.GetText();
                if (string.IsNullOrEmpty(text)) return;

                var mail = OutlookHelper.FindEmails(text)?.FirstOrDefault();
                if (string.IsNullOrEmpty(mail)) return;

                var t = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (t.Length > 1)
                  _firstName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(t[1].ToLower()); // ALEX will be ALEX without .ToLower() (2020-12-03)

                var idx = cbMail.Items.Add(mail);
                cbMail.SelectedIndex = idx; */
      }
      catch (Exception ex) { ex.Pop(); }
    }
    void onFilter(object s, EventArgs e) => filter();
    async void onSendFromCbx(object s, RoutedEventArgs e)
    {
      var scs = false;
      try
      {
        Bpr.Beep1of2();

        EnableControls(false);

        var text = tbMail.Text;
        if (string.IsNullOrEmpty(text)) return;

        var mail = OutlookHelper.FindEmails(text)?.FirstOrDefault();
        if (string.IsNullOrEmpty(mail)) return;

        _firstName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((tbName.Text ?? "Sirs").ToLower()); // ALEX will be ALEX without .ToLower() (2020-12-03)

        scs = await QStatusBroadcaster.SendLetter_UpdateDb(chkIsAvailable.IsChecked == true, mail, _firstName);
      }
      finally
      {
        tbkTitle.Text = $"{(scs ? "Success" : "Failure")}  sending to  {tbMail.Text}";
        EnableControls(true);
        Bpr.Beep2of2();
      }
    }
    void btnSave_Click(object s, RoutedEventArgs e) => Save();
    void onRefresh(object s, RoutedEventArgs e) => ReFresh();
    async void onReLoad(object s, RoutedEventArgs e) => tbkTitle.Text = Title = await reLoad();
    void onClear(object s, RoutedEventArgs e) => _obsColAvlbl.Clear();
    void btnBroadcastTopN_Click(object s, RoutedEventArgs e)
    {
      try
      {
        if (!int.TryParse(tbMax.Text, out var cnt))
          App.SpeakAsync($"Unable to parse {tbMax.Text}. Aborting broadcast.");
        else
        {
          for (var i = 0; i < cnt && i < vEMail_Avail_DevDataGrid.Items.Count; i++)
            _ = vEMail_Avail_DevDataGrid.SelectedItems.Add(vEMail_Avail_DevDataGrid.Items[i]);

          onBroadcast_Avail(s, e);
        }
      }
      catch (Exception ex) { ex.Pop(); }
    }
    async void onBroadcast_Avail(object s, RoutedEventArgs e)
    {
      Bpr.BeepClk();
      try
      {
        var cnt = vEMail_Avail_DevDataGrid.SelectedItems.Count;
        var cntRO = cnt;
        var msg = "Failes: ";
        var antiSpamBlockListPauseInMs = 1000 + (cnt < 12 ? cnt * cnt * cnt * cnt : 14000);

        App.SpeakAsync(tbkTitle.Text = $"Sending {cnt} letters. Anti Spam delay set to {antiSpamBlockListPauseInMs * .001:N0} sec. ETA {cnt * antiSpamBlockListPauseInMs * .001 / 60.0:N0} minutes.");

        EnableControls(false);
        var sw = Stopwatch.StartNew();
        foreach (var em in vEMail_Avail_DevDataGrid.SelectedItems)
        {
          var scs = await QStatusBroadcaster.SendLetter_UpdateDb(true, ((vEMail_Avail_Prod)em).ID, ((vEMail_Avail_Prod)em).FName);
          if (!scs)
            msg += $"\n  {((vEMail_Avail_Prod)em).ID}";

          tbkTitle.Text = $"Done/Todo: {cntRO - cnt} / {--cnt}      msg/min so far: {(cntRO - cnt) / sw.Elapsed.TotalMinutes:N1}      Last one is:  {(scs ? "Success" : "Failure")}  sending to  {((vEMail_Avail_Prod)em).ID}";

          await Task.Delay(antiSpamBlockListPauseInMs);
        }

        if (msg.Length > 12)
        {
          tbkTitle.Text = msg;
          App.SpeakAsync($"Apparently, some letters have failed being sent. ");
        }
        else
        {
          Hide();
          var prompt = $"Must run Outlook-to-DB now, to avoid double-sending!!!\n\n Review mailbox for unprocessed letters ... or just refer to *Done folder.";
          App.SpeakAsync(prompt);
          // _ = MessageBox.Show(prompt, "SUCCESS sending all letters", MessageBoxButton.OK, MessageBoxImage.Information);
          new OutlookToDbWindow().Show();

          Close(); // better close-reopen for a cleaner reload: //var rl = await reLoad();          reFresh();
        }
      }
      catch (Exception ex) { ex.Pop(); }
      finally { WindowState = System.Windows.WindowState.Normal; }
    }
    async void onBroadcast_UnAvl(object s, RoutedEventArgs e)
    {
      try
      {
        var cnt = vEMail_UnAvl_DevDataGrid.SelectedItems.Count;
        var sw = Stopwatch.StartNew();
        var lst = ""; foreach (var em in vEMail_UnAvl_DevDataGrid.SelectedItems) lst += ((vEMail_UnAvl_Prod)em).ID + Environment.NewLine;
        var qsn = string.Format("Send letter to these {0} selected addresses?", vEMail_UnAvl_DevDataGrid.SelectedItems.Count);
        //if (MessageBox.Show(lst, qsn, MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
          EnableControls(false);
          //WindowState = System.Windows.WindowState.Minimized;
          foreach (var em in vEMail_UnAvl_DevDataGrid.SelectedItems) _ = await QStatusBroadcaster.SendLetter_UpdateDb(false, ((vEMail_UnAvl_Prod)em).ID, ((vEMail_UnAvl_Prod)em).FName);
          Close();
        }
      }
      catch (Exception ex) { ex.Pop(); }
      finally { WindowState = System.Windows.WindowState.Normal; Bpr.BeepDone(); }
    }
    void chkIsAvailable_Checked(object s, RoutedEventArgs e)
    {
      //load(chkIsAvailable.IsChecked == true);
    }
    void onTglTest(object s, RoutedEventArgs e) => EnableControls(((ToggleButton)s).IsChecked == false);
    void onAgentsEdit(object s, RoutedEventArgs e) => new AgentAdminnWindow().Show(); // a special case for paralel acces to agents
    void tbMax_TextChanged(object s, TextChangedEventArgs e)
    {
      if (btMax != null && int.TryParse(tbMax?.Text, out _))
        btMax.Content = $"Top {tbMax.Text} rows";
    }

    void OnGetNameFromEmail(object s, RoutedEventArgs e) => tbName.Text = new Helpers.FirstLastNameParser(((TextBox)s).Text).FirstName;

    void cbMail_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
      if (tbMail != null)
        tbMail.Text = ((ContentControl)e.AddedItems[0])?.Content?.ToString();
    }
    void onSelectnChgd(object s, SelectionChangedEventArgs e)
    {
      var dg = (DataGrid)s;
      tbkTitle.Text = Title = $"{dg.SelectedItems.Count} / {dg.Items.Count} selected";
      btMax.Visibility = dg.SelectedItems.Count < 3 ? Visibility.Visible : Visibility.Collapsed;
      btSel.Visibility = dg.SelectedItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}
///todo: convey the automatinnes of the message:
///		...
///		As soon as my current contract comes closer to finish, expect another notification with my roll-off schedule.
///		Until then I may not have an opportunity to answer intermittent inquiries. 
///		IF you want to unsubscribe from recieving this messages, please ...
///		
/*
Aug 2019: Cleaned up Company names to better match the reality of name@CO.NAME.123.com => CO.NAME.123

INSERT INTO Agency                (AddedAt, ID)
SELECT   GETDATE() AS Expr2, SUBSTRING(ID, CHARINDEX('@', ID) + 1, LEN(ID) - CHARINDEX('@', ID) - CHARINDEX('.', REVERSE(ID))) AS Expr1
FROM      EMail
GROUP BY SUBSTRING(ID, CHARINDEX('@', ID) + 1, LEN(ID) - CHARINDEX('@', ID) - CHARINDEX('.', REVERSE(ID)))
HAVING   (SUBSTRING(ID, CHARINDEX('@', ID) + 1, LEN(ID) - CHARINDEX('@', ID) - CHARINDEX('.', REVERSE(ID))) NOT IN (SELECT   ID FROM      Agency AS Agency_1))

UPDATE  EMail
SET     Company  = SUBSTRING(ID, CHARINDEX('@', ID) + 1, LEN(ID) - CHARINDEX('@', ID) - CHARINDEX('.', REVERSE(ID))) ,ModifiedAt = GETDATE()
WHERE   Company <> SUBSTRING(ID, CHARINDEX('@', ID) + 1, LEN(ID) - CHARINDEX('@', ID) - CHARINDEX('.', REVERSE(ID)))











Backup copy of vEMail_Avail_Prod as of AUg 2019

SELECT   TOP (100) PERCENT ID, FName, LName, Company, Phone, PermBanReason, Notes, AddedAt, DoNotNotifyOnAvailableForCampaignID AS DoNotNotifyForCampaignID,
                  (SELECT   MAX(CampaignStart) AS Expr1
                   FROM      dbo.Campaign) AS CurrentCampaignStart,
                  (SELECT   Id
                   FROM      dbo.Campaign
                   WHERE   (CampaignStart =
                                       (SELECT   MAX(CampaignStart) AS Expr1
                                        FROM      dbo.Campaign AS Campaign_6))) AS LastCampaignID,
                  (SELECT   COUNT(*) AS Expr1
                   FROM      dbo.EHist
                   WHERE   (RecivedOrSent = 'S') AND (LetterBody NOT LIKE 'std%') AND (EMailID = em.ID)) AS MyReplies,
                  (SELECT   MAX(EmailedAt) AS Expr1
                   FROM      dbo.EHist
                   WHERE   (RecivedOrSent = 'S') AND (LetterBody NOT LIKE 'std%') AND (EMailID = em.ID)) AS LastSentAt,
                  (SELECT   MAX(EmailedAt) AS Expr1
                   FROM      dbo.EHist
                   WHERE   (RecivedOrSent <> 'S') AND (LetterBody NOT LIKE 'std%') AND (EMailID = em.ID)) AS LastRepliedAt
FROM      dbo.EMail AS em
WHERE   (ID NOT IN
                  (SELECT   ID
                   FROM      dbo.BadEmails())) AND (dbo.CurrentCampaignID() <> ISNULL(DoNotNotifyOnAvailableForCampaignID, - 1)) AND
                  ((SELECT   MAX(EmailedAt) AS LastSent
                    FROM      dbo.EHist
                    WHERE   (RecivedOrSent = 'S') AND (EMailID = em.ID)) IS NULL) OR
              (ID NOT IN
                  (SELECT   ID
                   FROM      dbo.BadEmails())) AND (dbo.CurrentCampaignID() <> ISNULL(DoNotNotifyOnAvailableForCampaignID, - 1)) AND
                  ((SELECT   MAX(EmailedAt) AS LastSent
                    FROM      dbo.EHist
                    WHERE   (RecivedOrSent = 'S') AND (EMailID = em.ID)) < dbo.CurrentCampaignStart())
ORDER BY AddedAt DESC
*/
