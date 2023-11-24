using Emailing.NET6;
using Emailing.NET6.Helpers;

namespace OutlookToDbWpfApp;
public partial class OutlookToDbWindow : WpfUserControlLib.Base.WindowBase
{
  static readonly DateTime Now = DateTime.Now;
  readonly OutlookHelper6 _oh = new();
  readonly QstatsRlsContext _db = QstatsRlsContext.Create();
  int _newEmailsAdded = 0;

  public OutlookToDbWindow() { InitializeComponent(); themeSelector1.ThemeApplier = ApplyTheme; tbver.Text = $".NET 8    Db: ~QStats        Ver: {VersionHelper.CurVer}  {(DevOps.IsDbg ? @"DBG" : "rls")}"; }
  protected override void OnClosing(System.ComponentModel.CancelEventArgs e) => base.OnClosing(e); /*DialogResult = _newEmailsAdded > 0;*/
  async void OnLoaded(object s, RoutedEventArgs e)
  {
    var qF = _oh.GetItemsFromFolder(OuFolder.qFail).Count;
    var qR = _oh.GetItemsFromFolder(OuFolder.qRcvd).Count;
    var qS = _oh.GetItemsFromFolder(OuFolder.qSent).Count;
    var qL = _oh.GetItemsFromFolder(OuFolder.qLate).Count;
    var qSD = _oh.GetItemsFromFolder(OuFolder.qSentDone).Count;
    var qRD = _oh.GetItemsFromFolder(OuFolder.qRcvdDone).Count;
    var ttl = qR + qS + qF + qL;

    if (ttl == 0)
    {
      await SpeechSynth.SpeakFreeAsync(tb1.Text = "Nothing new in Outlook to for DB.");
    }
    else
    {
      tb1.Text = $"Total {ttl} new items found (including {qL} OOF). Total sent/rcvd: {qSD} / {qRD} already.\n\n";

      await _db.Emails.LoadAsync();

      await OnDoReglr_();
      await OnDoFails_();
      await OnDoLater_();

      if (_newEmailsAdded > 0)
      {
        await SpeechSynth.SpeakFreeAsync($"Done. {_newEmailsAdded} new emails found.");
        Hide();
        new AgentAdminnWindow().Show();
        Close();
      }
      else
      {
        await SpeechSynth.SpeakFreeAsync("Done.");
      }
    }
  }
  void OnClose(object s, RoutedEventArgs e) { Close(); Application.Current.Shutdown(); }
  void OnUpdateOutlook(object s, RoutedEventArgs e) => tb1.Text += _oh.SyncDbToOutlook(_db);
  async void OnDoReglr(object s, RoutedEventArgs e) => await OnDoReglr_();
  async void OnDoJunkM(object s, RoutedEventArgs e) => await OnDoJunkM_();
  async void OnDoFails(object s, RoutedEventArgs e) => await OnDoFails_();
  async void OnDoLater(object s, RoutedEventArgs e) => await OnDoLater_();
  async void OnDoDoneR(object s, RoutedEventArgs e) => await OnDoDoneR_();

  async Task OnDoReglr_()
  {
    spCtlrPnl.IsEnabled = false;

    try
    {
      var sw = Stopwatch.StartNew();
      var rv = "";
      rv += await OutlookFolderToDb_ReglrAsync(OuFolder.qRcvd);
      rv += await OutlookFolderToDb_ReglrAsync(OuFolder.qSent);

      var (success, rowsSavedCnt, report) = await _db.TrySaveReportAsync("OutlookToDb.cs");
      tb1.Text += rv;
      WriteLine(rv);
      LoadVwSrcs();
    }
    catch (System.Exception ex) { ex.Pop(); }
    finally { spCtlrPnl.IsEnabled = true; }
  }
  async Task OnDoJunkM_()
  {
    spCtlrPnl.IsEnabled = false;

    try
    {
      var sw = Stopwatch.StartNew();
      var rv = "";
      rv += await OutlookFolderToDb_ReglrAsync(OuFolder.qJunkMail);

      var (success, rowsSavedCnt, report) = await _db.TrySaveReportAsync("OutlookToDb.cs");
      tb1.Text += rv;
      WriteLine(rv);
      LoadVwSrcs();
    }
    catch (System.Exception ex) { ex.Pop(); }
    finally { spCtlrPnl.IsEnabled = true; }
  }
  async Task OnDoFails_()
  {
    spCtlrPnl.IsEnabled = false;

    try
    {
      var sw = Stopwatch.StartNew();
      var rv = await OutlookFolderToDb_FailsAsync(OuFolder.qFail);
      var (success, rowsSavedCnt, report) = await _db.TrySaveReportAsync("OutlookToDb.cs");
      tb1.Text += rv;
      WriteLine(rv);
      LoadVwSrcs();
    }
    catch (Exception ex) { ex.Pop(); }
    finally { spCtlrPnl.IsEnabled = true; }
  }
  async Task OnDoLater_()
  {
    spCtlrPnl.IsEnabled = false;

    try
    {
      var sw = Stopwatch.StartNew();
      var rv = await OutlookFolderToDb_LaterAsync(OuFolder.qLate);
      var (success, rowsSavedCnt, report) = await _db.TrySaveReportAsync("OutlookToDb.cs");
      tb1.Text += rv;
      WriteLine(rv);
      LoadVwSrcs();
    }
    catch (System.Exception ex) { ex.Pop(); }
    finally { spCtlrPnl.IsEnabled = true; }
  }
  async Task OnDoDoneR_()
  {
    spCtlrPnl.IsEnabled = false;

    try
    {
      var sw = Stopwatch.StartNew();
      var rv = await OutlookFolderToDb_DoneRAsync(OuFolder.qRcvdDone);
      var (success, rowsSavedCnt, report) = await _db.TrySaveReportAsync("OutlookToDb.cs");
      tb1.Text += rv;
      WriteLine(rv);
      LoadVwSrcs();
    }
    catch (System.Exception ex) { ex.Pop(); }
    finally { spCtlrPnl.IsEnabled = true; }
  }

  async Task<TupleSubst> FindInsertEmailsFromBodyAsync(string body, string originalSenderEmail)
  {
    var newEmail = RegexHelper.FindEmails(body);
    var isAnyNew = false;

    for (var i = 0; i < newEmail.Length; i++)
    {
      if (!string.IsNullOrEmpty(newEmail[i]))
      {
        var (first, last) = OutlookHelper6.FigureOutFLNameFromBody(body, newEmail[i]);
        var em = await OutlookToDbWindowHelpers.CheckInsertEMailAsync(_db, newEmail[i], first, last, $"..from body (sender: {originalSenderEmail}). ");
        if (!isAnyNew) isAnyNew = em?.AddedAt == Now;
      }
    }

    return new TupleSubst { HasNewEmails = isAnyNew, NewEmails = newEmail };
  }
  class TupleSubst
  {
    public bool HasNewEmails { get; set; }
    public string[]? NewEmails { get; set; }
  }

  async Task<string> OutlookFolderToDb_ReglrAsync(string folderName)
  {
    int cnt = 0, ttl = 0, newEmailsAdded = 0;
    var rcvdDoneFolder = _oh.GetMapiFOlder(OuFolder.qRcvdDone);
    var sentDoneFolder = _oh.GetMapiFOlder(OuFolder.qSentDone);
    var deletedsFolder = _oh.GetMapiFOlder(OuFolder.qDltd);
    var report = "";

    try
    {
      var items = _oh.GetItemsFromFolder(folderName, "IPM.Note");
      ttl = items.Count;

      WriteLine($"\n ****** {items.Count,4}   IPM.Note   items in  {folderName}:");
      do
      {
        foreach (OL.MailItem mailItem in items)
        {
          ttl--;
          cnt++;
          try
          {
            if (folderName == OuFolder.qRcvd || folderName == OuFolder.qJunkMail)
            {
              var senderEmail = OutlookHelper6.FigureOutSenderEmail(mailItem);
              var isNew = await CheckDbInsertIfMissing_sender(mailItem, senderEmail, $"..from  {folderName}  folder. "); // checkInsertInotDbEMailAndEHistAsync(senderEmail, flNme.first, flNme.last, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, $"..was a sender", "R");  //foreach (OL.Recipient r in item.Recipients) ... includes potential CC addresses but appears as NEW and gets added ..probably because of wrong direction recvd/sent.				
              if (isNew) newEmailsAdded++;
              report += OutlookHelper6.ReportLine(folderName, senderEmail, isNew);

              if (!string.IsNullOrEmpty(mailItem.Body))
              {
                //await checkInsertEHistAsync(_db, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, "R", em); //2022-10 - added next line, as it was missing functionality of inseing received boides to Hist table:

                var (first, last) = OutlookHelper6.FigureOutSenderFLName(mailItem, senderEmail);
                var ii = await FindInsertEmailsFromBodyAsync(mailItem.Body, senderEmail); //if it's via Indeed - name is in the SenderName. Otherwise, it maybe away redirect to a colleague.
                if (ii.HasNewEmails)
                {
                  for (var i = 0; i < ii.NewEmails?.Length; i++) { if (!string.IsNullOrEmpty(ii.NewEmails[i])) { newEmailsAdded++; report += OutlookHelper6.ReportLine(folderName, ii.NewEmails[i], isNew); } }
                }
              }

              Write($"\n{ttl,2})  rcvd: {mailItem.ReceivedTime:yyyy-MMM-dd}  {senderEmail,-40}     {mailItem.Recipients.Count} rcpnts: (");
              foreach (OL.Recipient re in mailItem.Recipients)
              {
                var (first, last) = OutlookHelper6.FigureOutSenderFLName(re.Name, re.Address);

                var email = await OutlookToDbWindowHelpers.CheckInsertEMailAsync(_db, re.Address, first, last, $"..was a CC of {senderEmail} on {mailItem.SentOn:y-MM-dd HH:mm}. ");
                isNew = email?.AddedAt == Now;
                if (isNew) newEmailsAdded++;
                report += OutlookHelper6.ReportLine(folderName, re.Address, isNew);
              }

              ArgumentNullException.ThrowIfNull(rcvdDoneFolder, "rcvdDoneFolder is nul @@@@@@@@@@@@@@@");

              OutlookHelper6.MoveIt(rcvdDoneFolder, mailItem);
            }
            else if (folderName == OuFolder.qSent)
            {
              foreach (OL.Recipient re in mailItem.Recipients) // must use ReplyAll for this to work
              {
                var (first, last) = OutlookHelper6.FigureOutSenderFLName(re.Name, re.Address);

                var isNew = await OutlookToDbWindowHelpers.CheckInsert_EMail_EHist_Async(_db, re.Address, first, last, mailItem?.Subject, mailItem?.Body, mailItem?.SentOn, mailItem?.ReceivedTime, $"..from Sent folder. ", "S");
                if (isNew) { newEmailsAdded++; }

                report += OutlookHelper6.ReportLine(folderName, re.Address, isNew);
              }

              var trgFolder = (mailItem.Subject ?? "").StartsWith(QStatusBroadcaster.Asu) ? deletedsFolder : sentDoneFolder; // delete Avali-ty broadcasts.

              ArgumentNullException.ThrowIfNull(trgFolder, "MyStore is nul @@@@@@@@@@@@@@@");

              OutlookHelper6.MoveIt(trgFolder, mailItem);
            }
          }
          catch (System.Exception ex) { ex.Pop($"senderEmail: {mailItem?.SenderEmailAddress}. Report: {report}."); }
        } // for
#if DEBUG
      } while (false);
#else
      } while ((items = _oh.GetItemsFromFolder(folderName, "IPM.Note")).Count > 0); //not sure why, but it keeps skipping/missing items when  Move to OL folder, thus, this logic.
#endif

      _newEmailsAdded += newEmailsAdded;
      report += OutlookHelper6.ReportSectionTtl(folderName, cnt, newEmailsAdded);
    }
    catch (System.Exception ex) { ex.Pop(); }
    finally { WriteLine(""); }

    return report;
  }
  async Task<string> OutlookFolderToDb_FailsAsync(string folderName)
  {
    var report = "";
    int ttl0 = 0, newBansAdded = 0, newEmailsAdded = 0;
    try
    {
      var failsDoneFolder = _oh.GetMapiFOlder(OuFolder.qFailsDone);
      var itemsFailes = _oh.GetItemsFromFolder(folderName);
      int prev;
      do
      {
        var cnt = itemsFailes.Count;
        prev = cnt;
#if !DEBUG_ // save as then delete - to get the body and other stuff.
        if (DateTime.Now == DateTime.MinValue) foreach (OL.ReportItem item in itemsFailes)          {            TestAllKeys(item);          }
#endif
        foreach (var item in itemsFailes)
        {
          ttl0++;
          try
          {
            if (item is OL.ReportItem reportItem)
            {
              var senderEmail = reportItem.PropertyAccessor.GetProperty($"http://schemas.microsoft.com/mapi/proptag/0x0E04001E") as string; // https://stackoverflow.com/questions/25253442/non-delivery-reports-and-vba-script-in-outlook-2010
              var emr = _db.Emails.Find(senderEmail);
              if (emr == null)
              {
                var (first, last) = OutlookHelper6.FigureOutSenderFLName(reportItem, senderEmail ?? throw new ArgumentNullException(nameof(folderName), "#########%%%%%%%%"));
                var isNew = await OutlookToDbWindowHelpers.CheckInsert_EMail_EHist_Async(_db, senderEmail, first, last, reportItem.Subject, reportItem.Body, null, reportItem.CreationTime, "..banned upon delivery fail BUT not existed !!! ", "R");
                if (isNew) { newEmailsAdded++; }
              }
              else
              {
                BanPremanentlyInDB(ref report, ref newBansAdded, senderEmail ?? throw new ArgumentNullException(nameof(folderName), "#########%%%%%%%%"), "Delivery failed (a) ");
              }

              ArgumentNullException.ThrowIfNull(failsDoneFolder, "failsdonefolder is nul @@@@@@@@@@@@@@@");
              OutlookHelper6.MoveIt(failsDoneFolder, reportItem);
            }
            else if (item is OL.MailItem mailItem)
            {
              var senderEmail = OutlookHelper6.RemoveBadEmailParts(mailItem.SenderEmailAddress);
              var emr = _db.Emails.Find(senderEmail);
              if (emr == null)
              {
                var isNew = await CheckDbInsertIfMissing_sender(mailItem, senderEmail, "..banned upon delivery fail BUT not existed !!! "); // checkInsertInotDbEMailAndEHistAsync(senderEmail, flNme.first, flNme.last, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, "..banned upon delivery fail BUT not existed !!!", "R");
                if (isNew) { newEmailsAdded++; }
              }
              else
              {
                BanPremanentlyInDB(ref report, ref newBansAdded, senderEmail, "Delivery failed (b) ");
              }

              foreach (var emailFromBody in RegexHelper.FindEmails(mailItem.Body))
              {
                // banPremanentlyInDB(ref report, ref newBansAdded, emailFromBody, "Delivery failed (c) "); <== //todo: restore all %Delivery failed (c)%, since in the body usually alternative contacts are mentioned.

                if (_db.Emails.Find(emailFromBody) == null)
                {
                  var (first, last) = OutlookHelper6.FigureOutFLNameFromBody(mailItem.Body, emailFromBody);
                  var isNew = await OutlookToDbWindowHelpers.CheckInsert_EMail_EHist_Async(_db, emailFromBody, first, last, mailItem.Subject, mailItem.Body, mailItem.SentOn, mailItem.ReceivedTime, "..alt contact from Delvery-Fail body. ", "A");
                  if (isNew) { newEmailsAdded++; }
                }
              }

              ArgumentNullException.ThrowIfNull(failsDoneFolder, "senderEmail is nul @@@@@@@@@@@@@@@");
              OutlookHelper6.MoveIt(failsDoneFolder, mailItem);
            }
            else if (Debugger.IsAttached)
              Debugger.Break();
          }
          catch (Exception ex) { ex.Pop($"New  unfinished Aug 2019:{item.GetType().Name}."); }
        }

        itemsFailes = _oh.GetItemsFromFolder(folderName);
      } while (prev != itemsFailes.Count);
    }
    catch (System.Exception ex) { ex.Pop(); }

    _newEmailsAdded += newEmailsAdded;
    report += OutlookHelper6.ReportSectionTtl(folderName, ttl0, newBansAdded, newEmailsAdded);
    return report;
  }
  async Task<string> OutlookFolderToDb_LaterAsync(string folderName)
  {
    var report = "";
    int ttl0 = 0, newBansAdded = 0, newEmailsAdded = 0;
    try
    {
      var rcvdDoneFolder = _oh.GetMapiFOlder(OuFolder.qRcvdDone);
      var itemsTempAway = _oh.GetItemsFromFolder(folderName);
      int prev;
      do
      {
        var cnt = itemsTempAway.Count;
        prev = cnt;
#if !DEBUG_ // save as then delete - to get the body and other stuff.
        if (DateTime.Now == DateTime.MinValue) foreach (OL.ReportItem item in itemsTempAway)          {            TestAllKeys(item);          }
#endif
        foreach (var item in itemsTempAway)
        {
          ttl0++;
          try
          {
            if (item is OL.ReportItem reportItem)
            {
              if (Debugger.IsAttached) Debugger.Break(); else throw new Exception("AP: //todo: ReportItem in Q\\ToResend !!!");
            }
            else if (item is OL.MailItem mailItem)
            {
              var isNew = await CheckDbInsertIfMissing_sender(mailItem, OutlookHelper6.RemoveBadEmailParts(mailItem.SenderEmailAddress), "..was on vaction && not existed in DB ?!?! ");
              if (isNew) newEmailsAdded++;

              foreach (var emailFromBody in RegexHelper.FindEmails(mailItem.Body))
              {
                if (_db.Emails.Find(emailFromBody) == null)
                {
                  var (first, last) = OutlookHelper6.FigureOutFLNameFromBody(mailItem.Body, emailFromBody);
                  isNew = await OutlookToDbWindowHelpers.CheckInsert_EMail_EHist_Async(_db, emailFromBody, first, last, mailItem.Subject, mailItem.Body,mailItem.SentOn, mailItem.ReceivedTime, "..from I'm-Away body as alt contact ", "A");
                  if (isNew) { newEmailsAdded++; }
                }
              }

              if (Now > mailItem.ReceivedTime.AddDays(10)) // bad place ... but!
              {
                ArgumentNullException.ThrowIfNull(rcvdDoneFolder, "rcvdDoneFolder is nul @@@@@@@@@@@@@@@");

                var fnm = _db.Emails.Find(mailItem.SenderEmailAddress)?.Fname ?? OutlookHelper6.FigureOutSenderFLName(mailItem, mailItem.SenderEmailAddress).first;
                var scs = await QStatusBroadcasterProxy.SendLetter_UpdateDb(true, mailItem.SenderEmailAddress, fnm);
                if (scs)
                  OutlookHelper6.MoveIt(rcvdDoneFolder, mailItem);
              }
            }
            else
            if (Debugger.IsAttached) Debugger.Break(); else throw new Exception("AP: Review this case of missing row: must be something wrong.");
          }
          catch (Exception ex) { ex.Pop($"New  unfinished Aug 2019:{item.GetType().Name}."); }
        }

        itemsTempAway = _oh.GetItemsFromFolder(folderName);
      } while (prev != itemsTempAway.Count);
    }
    catch (Exception ex) { ex.Pop(); }

    _newEmailsAdded += newEmailsAdded;
    report += OutlookHelper6.ReportSectionTtl(folderName, ttl0, newBansAdded, newEmailsAdded);
    return report;
  }
  async Task<string> OutlookFolderToDb_DoneRAsync(string folderName)
  {
    string report___ = tb1.Text = "";
    var msg = "..was done before && not existed in DB ?!?! ";
    int ttlProcessed = 0, newEmailsAdded = 0;
    try
    {
      var itemsRcvdDone = _oh.GetItemsFromFolder(folderName);
      var ttl = itemsRcvdDone.Count;
      foreach (var item in itemsRcvdDone)
      {
        var senderEmail = "?";
        var rptLine = "";
        try
        {
          if (item is OL.ReportItem reportItem)
          {
            if (DateTime.Now == DateTime.MinValue) TestAllKeys(reportItem);

            senderEmail = reportItem.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E04001E") as string; // https://stackoverflow.com/questions/25253442/non-delivery-reports-and-vba-script-in-outlook-2010
            ArgumentNullException.ThrowIfNull(senderEmail, "senderEmail is nul @@@@@@@@@@@@@@@");
            senderEmail = OutlookHelper6.RemoveBadEmailParts(senderEmail);
            if (!OutlookHelper6.ValidEmailAddress(senderEmail)) { tb1.Text += $" ! {senderEmail}  \t <- invalid!!!\r\n"; continue; }

            var emr = _db.Emails.Find(senderEmail);
            if (emr == null)
            {
              var (first, last) = OutlookHelper6.FigureOutSenderFLName(reportItem, senderEmail);
              var isNew = await OutlookToDbWindowHelpers.CheckInsert_EMail_EHist_Async(_db, senderEmail, first, last, reportItem.Subject, "under constr-n", null, reportItem.CreationTime, msg, "R");
              if (isNew) { newEmailsAdded++; tb1.Text += $" * {senderEmail}\r\n"; }
            }

            rptLine += $"report\t{senderEmail,40}  {reportItem.CreationTime:yyyy-MM-dd}  {reportItem.Subject,-80} \t [no body - too slow and wrong]";
          }
          else if (item is OL.MailItem mailItem)
          {
            senderEmail = OutlookHelper6.RemoveBadEmailParts(mailItem.SenderEmailAddress);
            if (!OutlookHelper6.ValidEmailAddress(senderEmail)) { tb1.Text += $" ! {senderEmail}  \t <- invalid!!!\r\n"; continue; }

            var isNew = await CheckDbInsertIfMissing_sender(mailItem, senderEmail, msg);
            if (isNew) { newEmailsAdded++; tb1.Text += $" * {senderEmail}\r\n"; }

            foreach (var emailFromBody in RegexHelper.FindEmails(mailItem.Body))
            {
              senderEmail = OutlookHelper6.RemoveBadEmailParts(emailFromBody);
              if (!OutlookHelper6.ValidEmailAddress(senderEmail)) { tb1.Text += $" ! {senderEmail}  \t <- invalid!!!\r\n"; continue; }

              if (_db.Emails.Find(senderEmail) == null)
              {
                var (first, last) = OutlookHelper6.FigureOutFLNameFromBody(mailItem.Body, senderEmail);
                isNew = await OutlookToDbWindowHelpers.CheckInsert_EMail_EHist_Async(_db, senderEmail, first, last, mailItem.Subject, mailItem.Body,mailItem.SentOn, mailItem.ReceivedTime, "..from body. ", "R");
                if (isNew) { newEmailsAdded++; tb1.Text += $" * {senderEmail}\r\n"; }
              }

              rptLine += $"body\t{senderEmail,40}  {mailItem.CreationTime:yyyy-MM-dd}  {mailItem.Subject,-80}{OneLineAndTrunkate(mailItem.Body)}   ";
            }

            Write($"\n{ttl,2})  rcvd: {mailItem.ReceivedTime:yyyy-MMM-dd}  {senderEmail,-40}     {mailItem.Recipients.Count} rcpnts: (");
            var cnt = 0;
            foreach (OL.Recipient re in mailItem.Recipients)
            {
              var (first, last) = OutlookHelper6.FigureOutSenderFLName(re.Name, re.Address);

              var email = await OutlookToDbWindowHelpers.CheckInsertEMailAsync(_db, re.Address, first, last, $"..CC  {mailItem.SentOn:yyyy-MM-dd}  {++cnt,2}/{mailItem.Recipients.Count,-2}  by {senderEmail}. ");
              isNew = email?.AddedAt == Now;
              if (isNew) newEmailsAdded++;
              rptLine += OutlookHelper6.ReportLine(folderName, re.Address, isNew);
            }

            rptLine += $"mail\t{senderEmail,40}  {mailItem.CreationTime:yyyy-MM-dd}  {mailItem.Subject,-80}{OneLineAndTrunkate(mailItem.Body)}   ";
          }
          else if (item is OL.AppointmentItem itm0)  /**/ { tb1.Text += $" ? Appointment {itm0.CreationTime:yyyy-MM-dd} {itm0.Subject} \t {OneLineAndTrunkate(itm0.Body)} \r\n"; }
          else if (item is OL.DistListItem itm1)     /**/ { tb1.Text += $" ? DistList    {itm1.CreationTime:yyyy-MM-dd} {itm1.Subject} \t {OneLineAndTrunkate(itm1.Body)} \r\n"; }
          else if (item is OL.DocumentItem itm2)     /**/ { tb1.Text += $" ? Document    {itm2.CreationTime:yyyy-MM-dd} {itm2.Subject} \t {OneLineAndTrunkate(itm2.Body)} \r\n"; }
          else if (item is OL.JournalItem itm3)      /**/ { tb1.Text += $" ? Journal     {itm3.CreationTime:yyyy-MM-dd} {itm3.Subject} \t {OneLineAndTrunkate(itm3.Body)} \r\n"; }
          else if (item is OL.MeetingItem itm4)      /**/ { tb1.Text += $" ? Meeting     {itm4.CreationTime:yyyy-MM-dd} {itm4.Subject} \t {OneLineAndTrunkate(itm4.Body)} \r\n"; }
          else if (item is OL.MobileItem itm5)       /**/ { tb1.Text += $" ? Mobile      {itm5.CreationTime:yyyy-MM-dd} {itm5.Subject} \t {OneLineAndTrunkate(itm5.Body)} \r\n"; }
          else if (item is OL.NoteItem itm6)         /**/ { tb1.Text += $" ? Note        {itm6.CreationTime:yyyy-MM-dd} {itm6.Subject} \t {OneLineAndTrunkate(itm6.Body)} \r\n"; }
          else if (item is OL.TaskItem itm7)         /**/ { tb1.Text += $" ? Task        {itm7.CreationTime:yyyy-MM-dd} {itm7.Subject} \t {OneLineAndTrunkate(itm7.Body)} \r\n"; }
          else if (Debugger.IsAttached) { WriteLine($"AP: not procesed OL_type: {item.GetType().Name}"); Debugger.Break(); } else throw new Exception("AP: Review this case of missing type: must be something worth processing.");

          WriteLine($"{rptLine}");
        }
        catch (Exception ex) { ex.Pop($":{senderEmail}."); }

        lblMetaHeader.Content = $" ... found / current / ttl:  {newEmailsAdded} / {++ttlProcessed:N0} / {ttl:N0} ... ";

        if (ttlProcessed % 10 == 0) await Task.Delay(10); else await Task.Yield();
      }
    }
    catch (Exception ex) { ex.Pop(); }

    _newEmailsAdded += newEmailsAdded;
    report___ += OutlookHelper6.ReportSectionTtl(folderName, ttlProcessed, 0, newEmailsAdded);
    return report___;
  }

  static string OneLineAndTrunkate(string body, int max = 55)
  {
    if (string.IsNullOrEmpty(body)) return "";

    body = body.Replace("\r", " CR ").Replace("\n", " LF ");

    var len = body.Length;

    return len <= max ? body : (body[..max] + "...");
  }

  async Task<bool> CheckDbInsertIfMissing_sender(OL.MailItem mailItem, string senderEmail, string note)
  {
    //if (_db.Emails.Find(senderEmail) == null)
    var (first, last) = OutlookHelper6.FigureOutSenderFLName(mailItem, senderEmail);
    var isNew = await OutlookToDbWindowHelpers.CheckInsert_EMail_EHist_Async(_db, senderEmail, first, last, mailItem.Subject, mailItem.Body, mailItem.SentOn, mailItem.ReceivedTime, note, "R");
    return isNew;
  }

  void BanPremanentlyInDB(ref string rv, ref int newBansAdded, string email, string rsn)
  {
    var emr = _db.Emails.Find(email);
    if (emr == null)
    {
      if (Debugger.IsAttached) Debugger.Break(); else throw new Exception("AP: Review this case of missing row: must be something wrong.");
    }
    else
    {
      if (emr.PermBanReason == null || !emr.PermBanReason.Contains(rsn)) // if new reason
      {
        emr.PermBanReason += rsn + Now.ToString("yyyy-MM-dd");
        emr.ModifiedAt = Now;
        rv += $"{OuFolder.qFail,-15}  {email,-48}banned since: {rsn}\n";
        newBansAdded++;
      }
      else if (emr.PermBanReason.Contains(rsn)) // same reason already there
      {
        rv += $"{OuFolder.qFail,-15}  {email,-48}banned before with the same reason: {rsn}\n";
      }
      else
      {
        if (Debugger.IsAttached) Debugger.Break(); else throw new Exception("AP: Review this case of missing row: must be something wrong.");
      }
    }
  }

  static void LoadVwSrcs()
  {
    //((CollectionViewSource)(FindResource("eMailVwSrc"))).Source = ctx.Emails.Where(p => p.AddedAt >= before).ToList();
    //((CollectionViewSource)(FindResource("eMailVwSrc"))).View.MoveCurrentTo(null);
    //((CollectionViewSource)(FindResource("eHistVwSrc"))).Source = ctx.EHists.Where(p => p.AddedAt >= before).ToList();
    //((CollectionViewSource)(FindResource("eHistVwSrc"))).View.MoveCurrentTo(null);
  }

  static void TestOneKey(OL.ReportItem item, string key, bool isBinary = false)
  {
    try
    {
      Write($"==> {key}: ");

      var pa = item.PropertyAccessor;
      var url = $"http://schemas.microsoft.com/mapi/proptag/{key}";

      object prop;
      prop = pa.GetProperty(url);

      if (isBinary)
        prop = pa.BinaryToString(pa.GetProperty(url));

      if (prop is byte[])
      {
        var snd = pa.BinaryToString(prop);
        Write($"  === snd: {snd.Replace("\n", "-LF-").Replace("\r", "-CR-")}");

        var s = item.Session.GetAddressEntryFromID(snd);
        Write($"  *** GetExchangeDistributionList(): {s.GetExchangeDistributionList().ToString()?.Replace("\n", "-LF-").Replace("\r", "-CR-")}"); //.PrimarySmtpAddress;
        Write($"  *** Address: {s.Address}"); //.PrimarySmtpAddress;
      }
      else
      {
        Write($"==> {prop.GetType().Name}  {prop.ToString()?.Replace("\n", "-LF-").Replace("\r", "-CR-")}");
      }
    }
    catch (Exception ex) { Write($"  !!! ex: {ex.Message.Replace("\n", "-LF-").Replace("\r", "-CR-")}"); }

    WriteLine($"^^^^^^^^^^^^^^^^^^^^^^^^^");
  }
  static void TestAllKeys(OL.ReportItem item) // body for report item ...is hard to find; need more time, but really, who cares. // Aug 2019
  {
    if (item.Body.Length > 0)
    {
      WriteLine($"\n\n@@@ Body: {item.Body}");
      item.SaveAs(@"C:\temp\NDR.txt");
    }

    TestOneKey(item, "0x0E04001E"); //tu: email !!!
    TestOneKey(item, "0x007D001F");
    TestOneKey(item, "0x007D001E");
    TestOneKey(item, "0x0065001f");
    TestOneKey(item, "0x0078001f");
    TestOneKey(item, "0x0076001f");
    TestOneKey(item, "0x0C1F001F");
    TestOneKey(item, "0x5D01001F");
    TestOneKey(item, "0x5D02001F");
    TestOneKey(item, "0x0C190102");
    TestOneKey(item, "0x4030001F");
    TestOneKey(item, "0x003F0102");
    TestOneKey(item, "0x0076001F");

    TestOneKey(item, "0x001A001E"); // "PR_MESSAGE_CLASS"
    TestOneKey(item, "0x0037001E"); // "PR_SUBJECT"
    TestOneKey(item, "0x00390040"); // "PR_CLIENT_SUBMIT_TIME"
    TestOneKey(item, "0x003D001E"); // "PR_SUBJECT_PREFIX PT_STRING8"
    TestOneKey(item, "0x0040001E"); // "PR_RECEIVED_BY_NAME"
    TestOneKey(item, "0x0042001E"); // "PR_SENT_REPRESENTING_NAME"
    TestOneKey(item, "0x0050001E"); // "PR_REPLY_RECIPIENT_NAMES"

    TestOneKey(item, "0x0064001E"); // "PR_SENT_REPRESENTING_ADDRTYPE"
    TestOneKey(item, "0x0065001E"); // "PR_SENT_REPRESENTING_EMAIL_ADDRESS"
    TestOneKey(item, "0x0070001E"); // "PR_CONVERSATION_TOPIC"
    TestOneKey(item, "0x0075001E"); // "PR_RECEIVED_BY_ADDRTYPE"
    TestOneKey(item, "0x0076001E"); // "PR_RECEIVED_BY_EMAIL_ADDRESS"
    TestOneKey(item, "0x007D001E"); // "PR_TRANSPORT_MESSAGE_HEADERS"
    TestOneKey(item, "0x0C1A001E"); // "PR_SENDER_NAME"

    TestOneKey(item, "0x0C1E001E"); // "PR_SENDER_ADDRTYPE"
    TestOneKey(item, "0x0C1F001E"); // "PR_SENDER_EMAIL_ADDRESS"
    TestOneKey(item, "0x0E02001E"); // "PR_DISPLAY_BCC"
    TestOneKey(item, "0x0E03001E"); // "PR_DISPLAY_CC"
    TestOneKey(item, "0x0E04001E"); // "PR_DISPLAY_TO"
    TestOneKey(item, "0x0E060040"); // "PR_MESSAGE_DELIVERY_TIME"
    TestOneKey(item, "0x0E070003"); // "PR_MESSAGE_FLAGS"
    TestOneKey(item, "0x0E080003"); // "PR_MESSAGE_SIZE"

    TestOneKey(item, "0x0E12000D"); // "PR_MESSAGE_RECIPIENTS"
    TestOneKey(item, "0x0E13000D"); // "PR_MESSAGE_ATTACHMENTS"
    TestOneKey(item, "0x0E1B000B"); // "PR_HASATTACH"
    TestOneKey(item, "0x0E1D001E"); // "PR_NORMALIZED_SUBJECT"
    TestOneKey(item, "0x0E1F000B"); // "PR_RTF_IN_SYNC"
    TestOneKey(item, "0x0E28001E"); // "PR_PRIMARY_SEND_ACCT"
    TestOneKey(item, "0x0E29001E"); // "PR_NEXT_SEND_ACCT"
    TestOneKey(item, "0x0FF40003"); // "PR_ACCESS"
    TestOneKey(item, "0x0FF70003"); // "PR_ACCESS_LEVEL"

    TestOneKey(item, "0x0FFE0003"); // "PR_OBJECT_TYPE"
    TestOneKey(item, "0x1000001E"); // "PR_BODY"
    TestOneKey(item, "0x1035001E"); // "PR_INTERNET_MESSAGE_ID"
    TestOneKey(item, "0x1045001E"); // "PR_LIST_UNSUBSCRIBE"

    TestOneKey(item, "0x1046001E"); // "N/A"
    TestOneKey(item, "0x30070040"); // "PR_CREATION_TIME"
    TestOneKey(item, "0x30080040"); // "PR_LAST_MODIFICATION_TIME"
    TestOneKey(item, "0x340D0003"); // "PR_STORE_SUPPORT_MASK"
    TestOneKey(item, "0x340F0003"); // "N/A"
    TestOneKey(item, "0x3FDE0003"); // "PR_INTERNET_CPID"
    TestOneKey(item, "0x80050003"); // "SideEffects"
    TestOneKey(item, "0x802A001E"); // "InetAcctID"
    TestOneKey(item, "0x804F001E"); // "InetAcctName"
    TestOneKey(item, "0x80AD001E"); // "x-rcpt-to"

    TestOneKey(item, "0x003B0102", false); // "PR_SENT_REPRESENTING_SEARCH_KEY"
    TestOneKey(item, "0x003B0102", true); // "PR_SENT_REPRESENTING_SEARCH_KEY"

    TestOneKey(item, "0x003F0102", true); // "PR_RECEIVED_BY_ENTRYID"
    TestOneKey(item, "0x00410102", true); // "PR_SENT_REPRESENTING_ENTRYID"
    TestOneKey(item, "0x004F0102", true); // "PR_REPLY_RECIPIENT_ENTRIES"
    TestOneKey(item, "0x00510102", true); // "PR_RECEIVED_BY_SEARCH_KEY"
    TestOneKey(item, "0x00710102", true); // "PR_CONVERSATION_INDEX"
    TestOneKey(item, "0x0C190102", true); // "PR_SENDER_ENTRYID"
    TestOneKey(item, "0x0C1D0102", true); // "PR_SENDER_SEARCH_KEY"
    TestOneKey(item, "0x0E090102", true); // "PR_PARENT_ENTRYID"
    TestOneKey(item, "0x0FF80102", true); // "PR_MAPPING_SIGNATURE"
    TestOneKey(item, "0x0FF90102", true); // "PR_RECORD_KEY"
    TestOneKey(item, "0x0FFA0102", true); // "PR_STORE_RECORD_KEY"
    TestOneKey(item, "0x0FFB0102", true); // "PR_STORE_ENTRYID"
    TestOneKey(item, "0x0FFF0102", true); // "PR_ENTRYID"
    TestOneKey(item, "0x10090102", true); // "PR_RTF_COMPRESSED"
    TestOneKey(item, "0x10130102", true); // "PR_HTML"
    TestOneKey(item, "0x300B0102", true); // "PR_SEARCH_KEY"
    TestOneKey(item, "0x34140102", true); // "PR_MDB_PROVIDER"
    TestOneKey(item, "0x80660102", true); // "RemoteEID"

    WriteLine($"++++++++++++++++++++++++++++++++++++++ --------------------\n");
  }
}
/* Cache of https://stackoverflow.com/questions/25253442/non-delivery-reports-and-vba-script-in-outlook-2010

I've been dealing with a very similar issue myself and can offer some insight into my discoveries, which I hope might be helpful in your situation.

If .Body for NDR messages are showing up as questions marks or Chinese characters then that's because the NDR is actually made by Outlook 'on the fly' using 'Properties' and by using certain methods inaccessible to VBA.

You can use an add-in called Redemption to gain access to all the information that normal VBA doesn't permit, but you need to install and register it on every PC you need the code to work with (which is OK if only YOU need to use it) but for me this wasn't an option.

The easiest alternative to what you were trying to achieve is to save the body using .SaveAs first and then read the contents back. I've made some functions that might make it easier.

//usage example:
theBody = GetNDRBody(MailItem)

Function GetNDRBody(rItm As Object) As String
  Dim TheBody, TempFilePath As String
  If (LCase(rItm.MessageClass) = "report.ipm.note.ndr") Then
      TheBody = rItm.Body
      If Len(TheBody) > 0 Then
          If Chr(Asc(Left(TheBody, 1))) = "?" Then
              TempFilePath = AppDataDirectory & "\temp.txt"
              rItm.SaveAs TempFilePath, olTXT
              GetNDRBody = ReadFileContents(TempFilePath, True)
          End If
      End If
  End If
End Function

Function ReadFileContents(filePath As String, Optional DeleteWhenFinished As Boolean = False) As String
  Dim fso As Object: Set fso = CreateObject("scripting.filesystemobject")
  If fso.FileExists(filePath) Then
      Dim FileStream As Object: Set FileStream = fso.OpenTextFile(filePath, 1)
      ReadFileContents = FileStream.ReadAll
      FileStream.Close
      If DeleteWhenFinished = True Then fso.DeleteFile (filePath)
  End If
End Function
Function AppDataDirectory() As String
  Dim fso As Object: Set fso = CreateObject("scripting.filesystemobject")
  AppDataDirectory = fso.GetSpecialFolder(2)
  Set fso = Nothing
End Function


HOWEVER - I'm not sure what exact information your scanning NDRs for, but it may also be possible to find an alternative way using a Property. For example, here is a snippet I used to fetch the failed email list from an NDR:

(it only works if they are displayed as an email in the NDR immediately below the title 'Delivery has failed to these recipients or distribution lists:'. If it instead shows as a contact name then only the name will be in that 'property'. In my case, when they showed as a contact name then I would use the GetNDRBody function I made)

Dim objItem As Object

If (objItem.MessageClass = "REPORT.IPM.Note.NDR") Then
  Dim propertyAccessor As propertyAccessor
  Set propertyAccessor = objItem.propertyAccessor

  FailEmail = propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E04001E")
Sometimes there is a list of emails separated by "; " so I split it into an array and did a 'for each'

I also managed to get the email list from 'Mail Delivery Failed' emails this way, then spliting them into an array by ", " (this is just a snippet again)

If objItem.Subject = "Mail delivery failed: returning message to sender" Then
  Set propertyAccessor = objItem.propertyAccessor
  FailEmail = propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/x-failed-recipients/0x0000001F")
  FailEmail = Replace(FailEmail, ", ", vbNewLine)
...
FailEmails = Split(FailEmail, vbNewLine)
For Each FailedEmail in FailEmails
You can also try the below code to see if what you're looking for comes up as a common property (and you can also try installing OutlookSpy and see if there is a different property not listed here):

Set propertyAccessor = objItem.propertyAccessor
GetPropertyAccessorInfo propertyAccessor



Sub GetPropertyAccessorInfo(propertyAccessor As propertyAccessor)
 On Error Resume Next
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x001A001E"), , "PR_MESSAGE_CLASS"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0037001E"), , "PR_SUBJECT"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x00390040"), , "PR_CLIENT_SUBMIT_TIME"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x003D001E"), , "PR_SUBJECT_PREFIX PT_STRING8"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0040001E"), , "PR_RECEIVED_BY_NAME"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0042001E"), , "PR_SENT_REPRESENTING_NAME"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0050001E"), , "PR_REPLY_RECIPIENT_NAMES"

 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0064001E"), , "PR_SENT_REPRESENTING_ADDRTYPE"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0065001E"), , "PR_SENT_REPRESENTING_EMAIL_ADDRESS"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0070001E"), , "PR_CONVERSATION_TOPIC"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0075001E"), , "PR_RECEIVED_BY_ADDRTYPE"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0076001E"), , "PR_RECEIVED_BY_EMAIL_ADDRESS"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x007D001E"), , "PR_TRANSPORT_MESSAGE_HEADERS"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0C1A001E"), , "PR_SENDER_NAME"

 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0C1E001E"), , "PR_SENDER_ADDRTYPE"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0C1F001E"), , "PR_SENDER_EMAIL_ADDRESS"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E02001E"), , "PR_DISPLAY_BCC"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E03001E"), , "PR_DISPLAY_CC"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E04001E"), , "PR_DISPLAY_TO"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E060040"), , "PR_MESSAGE_DELIVERY_TIME"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E070003"), , "PR_MESSAGE_FLAGS"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E080003"), , "PR_MESSAGE_SIZE"

 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E12000D"), , "PR_MESSAGE_RECIPIENTS"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E13000D"), , "PR_MESSAGE_ATTACHMENTS"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E1B000B"), , "PR_HASATTACH"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E1D001E"), , "PR_NORMALIZED_SUBJECT"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E1F000B"), , "PR_RTF_IN_SYNC"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E28001E"), , "PR_PRIMARY_SEND_ACCT"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E29001E"), , "PR_NEXT_SEND_ACCT"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0FF40003"), , "PR_ACCESS"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0FF70003"), , "PR_ACCESS_LEVEL"

 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0FFE0003"), , "PR_OBJECT_TYPE"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x1000001E"), , "PR_BODY"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x1035001E"), , "PR_INTERNET_MESSAGE_ID"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x1045001E"), , "PR_LIST_UNSUBSCRIBE"

 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x1046001E"), , "N/A"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x30070040"), , "PR_CREATION_TIME"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x30080040"), , "PR_LAST_MODIFICATION_TIME"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x340D0003"), , "PR_STORE_SUPPORT_MASK"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x340F0003"), , "N/A"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x3FDE0003"), , "PR_INTERNET_CPID"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x80050003"), , "SideEffects"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x802A001E"), , "InetAcctID"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x804F001E"), , "InetAcctName"
 MsgBox propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x80AD001E"), , "x-rcpt-to"

 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x003B0102")), , "PR_SENT_REPRESENTING_SEARCH_KEY"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x003F0102")), , "PR_RECEIVED_BY_ENTRYID"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x00410102")), , "PR_SENT_REPRESENTING_ENTRYID"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x004F0102")), , "PR_REPLY_RECIPIENT_ENTRIES"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x00510102")), , "PR_RECEIVED_BY_SEARCH_KEY"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x00710102")), , "PR_CONVERSATION_INDEX"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0C190102")), , "PR_SENDER_ENTRYID"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0C1D0102")), , "PR_SENDER_SEARCH_KEY"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E090102")), , "PR_PARENT_ENTRYID"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0FF80102")), , "PR_MAPPING_SIGNATURE"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0FF90102")), , "PR_RECORD_KEY"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0FFA0102")), , "PR_STORE_RECORD_KEY"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0FFB0102")), , "PR_STORE_ENTRYID"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0FFF0102")), , "PR_ENTRYID"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x10090102")), , "PR_RTF_COMPRESSED"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x10130102")), , "PR_HTML"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x300B0102")), , "PR_SEARCH_KEY"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x34140102")), , "PR_MDB_PROVIDER"
 MsgBox propertyAccessor.BinaryToString(propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x80660102")), , "RemoteEID"
End Sub
shareeditflag
edited Aug 19 '15 at 16:33
answered Aug 18 '15 at 16:51

Sidupac
42849
add a comment
Your Answer
Links Images Styling/Headers Lists Blockquotes Code HTML advanced help »

community wiki
Post Your Answer
Not the answer you're looking for? Browse other questions tagged regex vba email outlook-vba outlook-2010 or ask your own question.
asked

4 years, 6 months ago

viewed

770 times

active

3 years, 3 months ago

*/
