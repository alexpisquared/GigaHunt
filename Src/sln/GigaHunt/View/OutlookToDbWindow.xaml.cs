﻿namespace OutlookToDbWpfApp;
public partial class OutlookToDbWindow : WpfUserControlLib.Base.WindowBase
{
  readonly OutlookHelper6 _oh = new();
  readonly QStatsRlsContext _db = QStatsRlsContext.Create();
  int _newEmailsAdded = 0;

  public OutlookToDbWindow() { InitializeComponent(); themeSelector1.ThemeApplier = ApplyTheme; tbver.Text = DevOps.IsDbg ? @"DBG" : "rls"; }
  protected override void OnClosing(System.ComponentModel.CancelEventArgs e) => base.OnClosing(e); /*DialogResult = _newEmailsAdded > 0;*/
  async void OnLoaded(object s, RoutedEventArgs e)
  {
    var qF = _oh.GetItemsFromFolder(Misc.qFail).Count;
    var qR = _oh.GetItemsFromFolder(Misc.qRcvd).Count;
    var qS = _oh.GetItemsFromFolder(Misc.qSent).Count;
    var qL = _oh.GetItemsFromFolder(Misc.qLate).Count;
    var qSD = _oh.GetItemsFromFolder(Misc.qSentDone).Count;
    var qRD = _oh.GetItemsFromFolder(Misc.qRcvdDone).Count;
    var ttl = qR + qS + qF + qL;

    if (ttl == 0)
    {
      App.Speak(tb1.Text = "Nothing new in Outlook to for DB.");
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
        App.Speak($"Done. {_newEmailsAdded} new emails found.");
        Hide();
        new AgentAdminnWindow().Show();
        Close();
      }
      else
      {
        App.Speak("Done.");
      }
    }
  }
  void OnClose(object s, RoutedEventArgs e) { Close(); Application.Current.Shutdown(); }
  void OnUpdateOutlook(object s, RoutedEventArgs e) => tb1.Text += _oh.SyncDbToOutlook(_db);
  async void OnDoReglr(object s, RoutedEventArgs e) => await OnDoReglr_();
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
      rv += await OutlookFolderToDb_ReglrAsync(Misc.qRcvd);
      rv += await OutlookFolderToDb_ReglrAsync(Misc.qSent);

      var (success, rowsSavedCnt, report) = await _db.TrySaveReportAsync("OutlookToDb.cs");
      tb1.Text += rv;
      WriteLine(rv);
      loadVwSrcs(App.Now);
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
      var rv = await OutlookFolderToDb_FailsAsync(Misc.qFail);
      var (success, rowsSavedCnt, report) = await _db.TrySaveReportAsync("OutlookToDb.cs");
      tb1.Text += rv;
      WriteLine(rv);
      loadVwSrcs(App.Now);
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
      var rv = await OutlookFolderToDb_LaterAsync(Misc.qLate);
      var (success, rowsSavedCnt, report) = await _db.TrySaveReportAsync("OutlookToDb.cs");
      tb1.Text += rv;
      WriteLine(rv);
      loadVwSrcs(App.Now);
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
      var rv = await OutlookFolderToDb_DoneRAsync(Misc.qRcvdDone);
      var (success, rowsSavedCnt, report) = await _db.TrySaveReportAsync("OutlookToDb.cs");
      tb1.Text += rv;
      WriteLine(rv);
      loadVwSrcs(App.Now);
    }
    catch (System.Exception ex) { ex.Pop(); }
    finally { spCtlrPnl.IsEnabled = true; }
  }

  public static async Task<bool> CheckInsert_EMail_EHist_Async(QStatsRlsContext _db, string email, string firstName, string lastName, string? subject, string? body, DateTime? timeRecdSent, string isRcvd, string RS)
  {
    var em = await CheckInsertEMailAsync(_db, email, firstName, lastName, isRcvd);
    if (em == null) return false;

    await CheckInsertEHistAsync(_db, subject, body, timeRecdSent ?? DateTime.Now, RS, em);

    var isNew = em?.AddedAt == App.Now;
    return isNew;
  }
  async Task<TupleSubst> FindInsertEmailsFromBodyAsync(string body, string originalSenderEmail)
  {
    var newEmail = OutlookHelper6.FindEmails(body);
    var isAnyNew = false;

    for (var i = 0; i < newEmail.Length; i++)
    {
      if (!string.IsNullOrEmpty(newEmail[i]))
      {
        var (first, last) = OutlookHelper6.figureOutFLNameFromBody(body, newEmail[i]);
        var em = await CheckInsertEMailAsync(_db, newEmail[i], first, last, $"..from body (sender: {originalSenderEmail}). ");
        if (!isAnyNew) isAnyNew = em?.AddedAt == App.Now;
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
    var rcvdDoneFolder = _oh.GetMapiFOlder(Misc.qRcvdDone);
    var sentDoneFolder = _oh.GetMapiFOlder(Misc.qSentDone);
    var deletedsFolder = _oh.GetMapiFOlder(Misc.qDltd);
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
            if (folderName == Misc.qRcvd)
            {
              var senderEmail = OutlookHelper6.FigureOutSenderEmail(mailItem);
              var isNew = await CheckDbInsertIfMissing_sender(mailItem, senderEmail, "..from  Q  folder. "); // checkInsertInotDbEMailAndEHistAsync(senderEmail, flNme.first, flNme.last, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, $"..was a sender", "R");  //foreach (OL.Recipient r in item.Recipients) ... includes potential CC addresses but appears as NEW and gets added ..probably because of wrong direction recvd/sent.				
              if (isNew) newEmailsAdded++;
              report += OutlookHelper6.reportLine(folderName, senderEmail, isNew);

              if (!string.IsNullOrEmpty(mailItem.Body))
              {
                //await checkInsertEHistAsync(_db, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, "R", em); //2022-10 - added next line, as it was missing functionality of inseing received boides to Hist table:

                var (first, last) = OutlookHelper6.figureOutSenderFLName(mailItem, senderEmail);
                var ii = await FindInsertEmailsFromBodyAsync(mailItem.Body, senderEmail); //if it's via Indeed - name is in the SenderName. Otherwise, it maybe away redirect to a colleague.
                if (ii.HasNewEmails)
                {
                  for (var i = 0; i < ii.NewEmails?.Length; i++) { if (!string.IsNullOrEmpty(ii.NewEmails[i])) { newEmailsAdded++; report += OutlookHelper6.reportLine(folderName, ii.NewEmails[i], isNew); } }
                }
              }

              Write($"\n{ttl,2})  rcvd: {mailItem.ReceivedTime:yyyy-MMM-dd}  {senderEmail,-40}     {mailItem.Recipients.Count} rcpnts: (");
              foreach (OL.Recipient re in mailItem.Recipients)
              {
                var (first, last) = OutlookHelper6.figureOutSenderFLName(re.Name, re.Address);

                var email = await CheckInsertEMailAsync(_db, re.Address, first, last, $"..was a CC of {senderEmail} on {mailItem.SentOn:y-MM-dd HH:mm}. ");
                isNew = email?.AddedAt == App.Now;
                if (isNew) newEmailsAdded++;
                report += OutlookHelper6.reportLine(folderName, re.Address, isNew);
              }

              ArgumentNullException.ThrowIfNull(rcvdDoneFolder, "rcvdDoneFolder is nul @@@@@@@@@@@@@@@");

              OutlookHelper6.moveIt(rcvdDoneFolder, mailItem);
            }
            else if (folderName == Misc.qSent)
            {
              foreach (OL.Recipient re in mailItem.Recipients) // must use ReplyAll for this to work
              {
                var (first, last) = OutlookHelper6.figureOutSenderFLName(re.Name, re.Address);

                var isNew = await CheckInsert_EMail_EHist_Async(_db, re.Address, first, last, mailItem?.Subject, mailItem?.Body, mailItem?.ReceivedTime, $"..from Sent folder. ", "S");
                if (isNew) { newEmailsAdded++; }

                report += OutlookHelper6.reportLine(folderName, re.Address, isNew);
              }

              var trgFolder = (mailItem.Subject ?? "").StartsWith(QStatusBroadcaster.Asu) ? deletedsFolder : sentDoneFolder; // delete Avali-ty broadcasts.

              ArgumentNullException.ThrowIfNull(trgFolder, "MyStore is nul @@@@@@@@@@@@@@@");

              OutlookHelper6.moveIt(trgFolder, mailItem);
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
      report += OutlookHelper6.reportSectionTtl(folderName, cnt, newEmailsAdded);
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
      var failsDoneFolder = _oh.GetMapiFOlder(Misc.qFailsDone);
      var itemsFailes = _oh.GetItemsFromFolder(folderName);
      int prev;
      do
      {
        var cnt = itemsFailes.Count;
        prev = cnt;
#if DEBUG_ // save as then delete - to get the body and other stuff.
        foreach (OL.ReportItem item in itemsFailes)          {            testAllKeys(item);          }
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
                var (first, last) = OutlookHelper6.figureOutSenderFLName(reportItem, senderEmail ?? throw new ArgumentNullException(nameof(folderName), "#########%%%%%%%%"));
                var isNew = await CheckInsert_EMail_EHist_Async(_db, senderEmail, first, last, reportItem.Subject, reportItem.Body, reportItem.CreationTime, "..banned upon delivery fail BUT not existed !!! ", "R");
                if (isNew) { newEmailsAdded++; }
              }
              else
              {
                banPremanentlyInDB(ref report, ref newBansAdded, senderEmail ?? throw new ArgumentNullException(nameof(folderName), "#########%%%%%%%%"), "Delivery failed (a) ");
              }

              ArgumentNullException.ThrowIfNull(failsDoneFolder, "failsdonefolder is nul @@@@@@@@@@@@@@@");
              OutlookHelper6.moveIt(failsDoneFolder, reportItem);
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
                banPremanentlyInDB(ref report, ref newBansAdded, senderEmail, "Delivery failed (b) ");
              }

              foreach (var emailFromBody in OutlookHelper6.FindEmails(mailItem.Body))
              {
                // banPremanentlyInDB(ref report, ref newBansAdded, emailFromBody, "Delivery failed (c) "); <== //todo: restore all %Delivery failed (c)%, since in the body usually alternative contacts are mentioned.

                var emr2 = _db.Emails.Find(emailFromBody);
                if (emr2 == null)
                {
                  var (first, last) = OutlookHelper6.figureOutFLNameFromBody(mailItem.Body, emailFromBody);
                  var isNew = await CheckInsert_EMail_EHist_Async(_db, emailFromBody, first, last, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, "..alt contact from Delvery-Fail body. ", "A");
                  if (isNew) { newEmailsAdded++; }
                }
              }

              ArgumentNullException.ThrowIfNull(failsDoneFolder, "senderEmail is nul @@@@@@@@@@@@@@@");
              OutlookHelper6.moveIt(failsDoneFolder, mailItem);
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
    report += OutlookHelper6.reportSectionTtl(folderName, ttl0, newBansAdded, newEmailsAdded);
    return report;
  }
  async Task<string> OutlookFolderToDb_LaterAsync(string folderName)
  {
    var report = "";
    int ttl0 = 0, newBansAdded = 0, newEmailsAdded = 0;
    try
    {
      var rcvdDoneFolder = _oh.GetMapiFOlder(Misc.qRcvdDone);
      var itemsTempAway = _oh.GetItemsFromFolder(folderName);
      int prev;
      do
      {
        var cnt = itemsTempAway.Count;
        prev = cnt;
#if DEBUG_ // save as then delete - to get the body and other stuff.
        foreach (OL.ReportItem item in itemsFailes)          {            testAllKeys(item);          }
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

              foreach (var emailFromBody in OutlookHelper6.FindEmails(mailItem.Body))
              {
                var emr2 = _db.Emails.Find(emailFromBody);
                if (emr2 == null)
                {
                  var (first, last) = OutlookHelper6.figureOutFLNameFromBody(mailItem.Body, emailFromBody);
                  isNew = await CheckInsert_EMail_EHist_Async(_db, emailFromBody, first, last, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, "..from I'm-Away body as alt contact ", "A");
                  if (isNew) { newEmailsAdded++; }
                }
              }

              if (App.Now > mailItem.ReceivedTime.AddDays(10)) // bad place ... but!
              {
                ArgumentNullException.ThrowIfNull(rcvdDoneFolder, "rcvdDoneFolder is nul @@@@@@@@@@@@@@@");

                var fnm = _db.Emails.Find(mailItem.SenderEmailAddress)?.Fname ?? OutlookHelper6.figureOutSenderFLName(mailItem, mailItem.SenderEmailAddress).first;
                var scs = await QStatusBroadcaster.SendLetter_UpdateDb(true, mailItem.SenderEmailAddress, fnm);
                if (scs)
                  OutlookHelper6.moveIt(rcvdDoneFolder, mailItem);
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
    report += OutlookHelper6.reportSectionTtl(folderName, ttl0, newBansAdded, newEmailsAdded);
    return report;
  }
  async Task<string> OutlookFolderToDb_DoneRAsync(string folderName)
  {
    var report___ = tb1.Text = "";
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
            // testAllKeys(reportItem);
            senderEmail = reportItem.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E04001E") as string; // https://stackoverflow.com/questions/25253442/non-delivery-reports-and-vba-script-in-outlook-2010
            ArgumentNullException.ThrowIfNull(senderEmail, "senderEmail is nul @@@@@@@@@@@@@@@");
            senderEmail = OutlookHelper6.RemoveBadEmailParts(senderEmail);
            if (!OutlookHelper6.ValidEmailAddress(senderEmail)) { tb1.Text += $" ! {senderEmail}  \t <- invalid!!!\r\n"; continue; }

            var emr = _db.Emails.Find(senderEmail);
            if (emr == null)
            {
              var (first, last) = OutlookHelper6.figureOutSenderFLName(reportItem, senderEmail);
              var isNew = await CheckInsert_EMail_EHist_Async(_db, senderEmail, first, last, reportItem.Subject, "under constr-n", reportItem.CreationTime, msg, "R");
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

            foreach (var emailFromBody in OutlookHelper6.FindEmails(mailItem.Body))
            {
              senderEmail = OutlookHelper6.RemoveBadEmailParts(emailFromBody);
              if (!OutlookHelper6.ValidEmailAddress(senderEmail)) { tb1.Text += $" ! {senderEmail}  \t <- invalid!!!\r\n"; continue; }

              var emr2 = _db.Emails.Find(senderEmail);
              if (emr2 == null)
              {
                var (first, last) = OutlookHelper6.figureOutFLNameFromBody(mailItem.Body, senderEmail);
                isNew = await CheckInsert_EMail_EHist_Async(_db, senderEmail, first, last, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, "..from body. ", "R");
                if (isNew) { newEmailsAdded++; tb1.Text += $" * {senderEmail}\r\n"; }
              }

              rptLine += $"body\t{senderEmail,40}  {mailItem.CreationTime:yyyy-MM-dd}  {mailItem.Subject,-80}{OneLineAndTrunkate(mailItem.Body)}   ";
            }

            Write($"\n{ttl,2})  rcvd: {mailItem.ReceivedTime:yyyy-MMM-dd}  {senderEmail,-40}     {mailItem.Recipients.Count} rcpnts: (");
            var cnt = 0;
            foreach (OL.Recipient re in mailItem.Recipients)
            {
              var (first, last) = OutlookHelper6.figureOutSenderFLName(re.Name, re.Address);

              var email = await CheckInsertEMailAsync(_db, re.Address, first, last, $"..CC  {mailItem.SentOn:yyyy-MM-dd}  {++cnt,2}/{mailItem.Recipients.Count,-2}  by {senderEmail}. ");
              isNew = email?.AddedAt == App.Now;
              if (isNew) newEmailsAdded++;
              rptLine += OutlookHelper6.reportLine(folderName, re.Address, isNew);
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
    report___ += OutlookHelper6.reportSectionTtl(folderName, ttlProcessed, 0, newEmailsAdded);
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
    var (first, last) = OutlookHelper6.figureOutSenderFLName(mailItem, senderEmail);
    var isNew = await CheckInsert_EMail_EHist_Async(_db, senderEmail, first, last, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, note, "R");
    return isNew;
  }
  public static async Task<Email?> CheckInsertEMailAsync(QStatsRlsContext _db, string email, string firstName, string lastName, string notes)
  {
    const int maxLen = 256;

    if (email.EndsWith("@msg.monster.com") && email.Length > 46) // ~ 3212846259f94b158701020f5ca8ac4e@msg.monster.com
      return null;

    if (email.Length > maxLen)
      email = email.Substring(email.Length - maxLen, maxLen);

    var em = _db.Emails.Find(email);
    if (em == null)
    {
      var agency = OutlookHelper6.GetCompanyName(email);

      try
      {
        var r2 = _db.Agencies.Any(r => r.Id.Equals(agency.ToLower()));
        var r3 = _db.Agencies.Any(r => r.Id.Equals(agency.ToUpper()));

        if (!_db.Agencies.Any(r => r.Id.Equals(agency))) //i think db is set to be case ignore:  , StringComparison.InvariantCultureIgnoreCase)) )
        {
          _ = _db.Agencies.Add(new Agency
          {
            Id = agency.Length > maxLen ? agency.Substring(agency.Length - maxLen, maxLen) : agency,
            AddedAt = GigaHunt.App.Now
          });
        }
      }
      catch (Exception ex) { ex.Pop("."); }


      em = _db.Emails.Add(new Email
      {
        Id = email.Length > maxLen ? email.Substring(email.Length - maxLen, maxLen) : email,
        Company = agency,
        Fname = firstName,
        Lname = lastName,
        Notes = notes,
        AddedAt = GigaHunt.App.Now,
        ReSendAfter = null,
        NotifyPriority = 99
      }).Entity;

      _ = await _db.TrySaveReportAsync("checkInsertEMail");
    }

    return em;
  }
  public static async Task CheckInsertEHistAsync(QStatsRlsContext _db, string? subject, string? body, DateTime timeRecdSent, string rs, Email em)
  {
    //insertEMailEHistItem(isRcvd, timeRecdSent, em, subject, body);		}		void insertEMailEHistItem(bool isRcvd, DateTime timeRecdSent, Email em, string subject, string body)		{
    try
    {
      var gt = timeRecdSent.AddMinutes(-5);
      var lt = timeRecdSent.AddMinutes(+5);         //var ch = isRcvd ? ctx.EHists.Where(p => p.EmailedAt.HasValue && gt < p.EmailedAt.Value && p.EmailedAt.Value < lt && p.EMailId == id.Id) : ctx.EHists.Where(p => p.EmailedAt.HasValue && gt < p.EmailedAt.Value && p.EmailedAt.Value < lt && p.EMailId == id.Id); if (ch.Count() < 1)
      var eh = _db.Ehists.FirstOrDefault(p => p.RecivedOrSent == rs && p.EmailId == em.Id && gt < p.EmailedAt && p.EmailedAt < lt);
      if (eh == null)
      {
        var newEH = new Ehist
        {
          RecivedOrSent = rs,
          Email = em,
          LetterBody = string.IsNullOrEmpty(body) ? "" : body.Replace("\n\n\n", "\n\n").Replace("\n\n", "\n").Replace("\r\n\r\n\r\n", "\n\n").Replace("\r\n\r\n", "\n"),
          LetterSubject = subject,
          AddedAt = GigaHunt.App.Now,
          Notes = "",
          EmailedAt = timeRecdSent
        };
        var newCH2 = _db.Ehists.Add(newEH);

        _ = await _db.TrySaveReportAsync("checkInsertEHist");
      }
    }
    catch (Exception ex) { ex.Pop(); }
  }

  void banPremanentlyInDB(ref string rv, ref int newBansAdded, string email, string rsn)
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
        emr.PermBanReason += rsn + App.Now.ToString("yyyy-MM-dd");
        emr.ModifiedAt = App.Now;
        rv += $"{Misc.qFail,-15}  {email,-48}banned since: {rsn}\n";
        newBansAdded++;
      }
      else if (emr.PermBanReason.Contains(rsn)) // same reason already there
      {
        rv += $"{Misc.qFail,-15}  {email,-48}banned before with the same reason: {rsn}\n";
      }
      else
      {
        if (Debugger.IsAttached) Debugger.Break(); else throw new Exception("AP: Review this case of missing row: must be something wrong.");
      }
    }
  }
  void loadVwSrcs(DateTime before)
  {
    //((CollectionViewSource)(FindResource("eMailVwSrc"))).Source = ctx.Emails.Where(p => p.AddedAt >= before).ToList();
    //((CollectionViewSource)(FindResource("eMailVwSrc"))).View.MoveCurrentTo(null);
    //((CollectionViewSource)(FindResource("eHistVwSrc"))).Source = ctx.EHists.Where(p => p.AddedAt >= before).ToList();
    //((CollectionViewSource)(FindResource("eHistVwSrc"))).View.MoveCurrentTo(null);
  }

  static void testOneKey(OL.ReportItem item, string key, bool isBinary = false)
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
  static void testAllKeys(OL.ReportItem item) // body for report item ...is hard to find; need more time, but really, who cares. // Aug 2019
  {
    if (item.Body.Length > 0)
    {
      WriteLine($"\n\n@@@ Body: {item.Body}");
      item.SaveAs(@"C:\temp\NDR.txt");
    }

    testOneKey(item, "0x0E04001E"); //tu: email !!!
    testOneKey(item, "0x007D001F");
    testOneKey(item, "0x007D001E");
    testOneKey(item, "0x0065001f");
    testOneKey(item, "0x0078001f");
    testOneKey(item, "0x0076001f");
    testOneKey(item, "0x0C1F001F");
    testOneKey(item, "0x5D01001F");
    testOneKey(item, "0x5D02001F");
    testOneKey(item, "0x0C190102");
    testOneKey(item, "0x4030001F");
    testOneKey(item, "0x003F0102");
    testOneKey(item, "0x0076001F");

    testOneKey(item, "0x001A001E"); // "PR_MESSAGE_CLASS"
    testOneKey(item, "0x0037001E"); // "PR_SUBJECT"
    testOneKey(item, "0x00390040"); // "PR_CLIENT_SUBMIT_TIME"
    testOneKey(item, "0x003D001E"); // "PR_SUBJECT_PREFIX PT_STRING8"
    testOneKey(item, "0x0040001E"); // "PR_RECEIVED_BY_NAME"
    testOneKey(item, "0x0042001E"); // "PR_SENT_REPRESENTING_NAME"
    testOneKey(item, "0x0050001E"); // "PR_REPLY_RECIPIENT_NAMES"

    testOneKey(item, "0x0064001E"); // "PR_SENT_REPRESENTING_ADDRTYPE"
    testOneKey(item, "0x0065001E"); // "PR_SENT_REPRESENTING_EMAIL_ADDRESS"
    testOneKey(item, "0x0070001E"); // "PR_CONVERSATION_TOPIC"
    testOneKey(item, "0x0075001E"); // "PR_RECEIVED_BY_ADDRTYPE"
    testOneKey(item, "0x0076001E"); // "PR_RECEIVED_BY_EMAIL_ADDRESS"
    testOneKey(item, "0x007D001E"); // "PR_TRANSPORT_MESSAGE_HEADERS"
    testOneKey(item, "0x0C1A001E"); // "PR_SENDER_NAME"

    testOneKey(item, "0x0C1E001E"); // "PR_SENDER_ADDRTYPE"
    testOneKey(item, "0x0C1F001E"); // "PR_SENDER_EMAIL_ADDRESS"
    testOneKey(item, "0x0E02001E"); // "PR_DISPLAY_BCC"
    testOneKey(item, "0x0E03001E"); // "PR_DISPLAY_CC"
    testOneKey(item, "0x0E04001E"); // "PR_DISPLAY_TO"
    testOneKey(item, "0x0E060040"); // "PR_MESSAGE_DELIVERY_TIME"
    testOneKey(item, "0x0E070003"); // "PR_MESSAGE_FLAGS"
    testOneKey(item, "0x0E080003"); // "PR_MESSAGE_SIZE"

    testOneKey(item, "0x0E12000D"); // "PR_MESSAGE_RECIPIENTS"
    testOneKey(item, "0x0E13000D"); // "PR_MESSAGE_ATTACHMENTS"
    testOneKey(item, "0x0E1B000B"); // "PR_HASATTACH"
    testOneKey(item, "0x0E1D001E"); // "PR_NORMALIZED_SUBJECT"
    testOneKey(item, "0x0E1F000B"); // "PR_RTF_IN_SYNC"
    testOneKey(item, "0x0E28001E"); // "PR_PRIMARY_SEND_ACCT"
    testOneKey(item, "0x0E29001E"); // "PR_NEXT_SEND_ACCT"
    testOneKey(item, "0x0FF40003"); // "PR_ACCESS"
    testOneKey(item, "0x0FF70003"); // "PR_ACCESS_LEVEL"

    testOneKey(item, "0x0FFE0003"); // "PR_OBJECT_TYPE"
    testOneKey(item, "0x1000001E"); // "PR_BODY"
    testOneKey(item, "0x1035001E"); // "PR_INTERNET_MESSAGE_ID"
    testOneKey(item, "0x1045001E"); // "PR_LIST_UNSUBSCRIBE"

    testOneKey(item, "0x1046001E"); // "N/A"
    testOneKey(item, "0x30070040"); // "PR_CREATION_TIME"
    testOneKey(item, "0x30080040"); // "PR_LAST_MODIFICATION_TIME"
    testOneKey(item, "0x340D0003"); // "PR_STORE_SUPPORT_MASK"
    testOneKey(item, "0x340F0003"); // "N/A"
    testOneKey(item, "0x3FDE0003"); // "PR_INTERNET_CPID"
    testOneKey(item, "0x80050003"); // "SideEffects"
    testOneKey(item, "0x802A001E"); // "InetAcctID"
    testOneKey(item, "0x804F001E"); // "InetAcctName"
    testOneKey(item, "0x80AD001E"); // "x-rcpt-to"

    testOneKey(item, "0x003B0102", false); // "PR_SENT_REPRESENTING_SEARCH_KEY"
    testOneKey(item, "0x003B0102", true); // "PR_SENT_REPRESENTING_SEARCH_KEY"

    testOneKey(item, "0x003F0102", true); // "PR_RECEIVED_BY_ENTRYID"
    testOneKey(item, "0x00410102", true); // "PR_SENT_REPRESENTING_ENTRYID"
    testOneKey(item, "0x004F0102", true); // "PR_REPLY_RECIPIENT_ENTRIES"
    testOneKey(item, "0x00510102", true); // "PR_RECEIVED_BY_SEARCH_KEY"
    testOneKey(item, "0x00710102", true); // "PR_CONVERSATION_INDEX"
    testOneKey(item, "0x0C190102", true); // "PR_SENDER_ENTRYID"
    testOneKey(item, "0x0C1D0102", true); // "PR_SENDER_SEARCH_KEY"
    testOneKey(item, "0x0E090102", true); // "PR_PARENT_ENTRYID"
    testOneKey(item, "0x0FF80102", true); // "PR_MAPPING_SIGNATURE"
    testOneKey(item, "0x0FF90102", true); // "PR_RECORD_KEY"
    testOneKey(item, "0x0FFA0102", true); // "PR_STORE_RECORD_KEY"
    testOneKey(item, "0x0FFB0102", true); // "PR_STORE_ENTRYID"
    testOneKey(item, "0x0FFF0102", true); // "PR_ENTRYID"
    testOneKey(item, "0x10090102", true); // "PR_RTF_COMPRESSED"
    testOneKey(item, "0x10130102", true); // "PR_HTML"
    testOneKey(item, "0x300B0102", true); // "PR_SEARCH_KEY"
    testOneKey(item, "0x34140102", true); // "PR_MDB_PROVIDER"
    testOneKey(item, "0x80660102", true); // "RemoteEID"

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
