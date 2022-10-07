using AAV.WPF.Ext;
using AgentFastAdmin;
using AsLink;
using AvailStatusEmailer;
using AvailStatusEmailer.View;
using Db.QStats.DbModel;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using OL = Microsoft.Office.Interop.Outlook;

namespace OutlookToDbWpfApp
{
  public partial class OutlookToDbWindow : AAV.WPF.Base.WindowBase
  {
    readonly OutlookHelper _oh = new OutlookHelper();
    readonly A0DbContext _db = A0DbContext.Create();
    int _newEmailsAdded = 0;

    public OutlookToDbWindow() { InitializeComponent(); themeSelector1.ApplyTheme = ApplyTheme; }
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e) => base.OnClosing(e); /*DialogResult = _newEmailsAdded > 0;*/
    async void onLoaded(object s = null, RoutedEventArgs e = null)
    {
#if DEBUG
      await onDoReglr_();      //_oh.FindContactByEmail("jingmei.li@live.com");
#else
      var qF = _oh.GetItemsFromFolder(Misc.qFail).Count;
      var qR = _oh.GetItemsFromFolder(Misc.qRcvd).Count;
      var qS = _oh.GetItemsFromFolder(Misc.qSent).Count;
      var qL = _oh.GetItemsFromFolder(Misc.qLate).Count;
      var qSD = _oh.GetItemsFromFolder(Misc.qSentDone).Count;
      var qRD = _oh.GetItemsFromFolder(Misc.qRcvdDone).Count;
      var ttl = qR + qS + qF + qL;

      if (ttl == 0)
      {
        tb1.Text = "Nothing new in Outlook to for DB. Closing in 2 sec. Not!";
        App.SpeakSynch(tb1.Text);
        //await Task.Delay(2000);        Close();
      }
      else
      {
        tb1.Text = $"Total {ttl} new items found (including {qL} OOF). Total sent/rcvd: {qSD} / {qRD} already.\n\n";
        App.SpeakAsync(tb1.Text);

        await _db.EMails.LoadAsync();
        await _db.EHists.LoadAsync();
        App.SpeakAsync("DB Loaded.");

        await onDoReglr_();
        await onDoFails_();
        await onDoLater_();

        if (_newEmailsAdded > 0)
        {
          App.SpeakAsync($"Finished. Review {_newEmailsAdded} new email addresses found.");
          Hide();
          new AgentAdminnWindow().Show();
          Close();
        }
        else
        {
          App.SpeakAsync("Finished Outlook-to-database processing.");
        }
      }
#endif
    }

    void onClose(object s = null, RoutedEventArgs e = null) => Close();

    void onUpdateOutlook(object s = null, RoutedEventArgs e = null) => tb1.Text += _oh.SyncDbToOutlook(_db, Tag.ToString());

    async void onDoReglr(object s = null, RoutedEventArgs e = null) => await onDoReglr_();
    public async Task onDoReglr_()
    {
      spCtlrPnl.IsEnabled = false;

      try
      {
        var sw = Stopwatch.StartNew();
        var rv = "";
        rv += await outlookFolderToDb_ReglrAsync(Misc.qRcvd);
        rv += await outlookFolderToDb_ReglrAsync(Misc.qSent);

        var rowsAdded = await _db.TrySaveReportAsync("OutlookToDb.cs");
        tb1.Text += rv;
        Debug.WriteLine(rv);
        loadVwSrcs(App.Now);
      }
      catch (Exception ex) { ex.Pop(); }
      finally { spCtlrPnl.IsEnabled = true; }
    }
    async void onDoFails(object s = null, RoutedEventArgs e = null) => await onDoFails_();
    async Task onDoFails_()
    {
      spCtlrPnl.IsEnabled = false;

      try
      {
        var sw = Stopwatch.StartNew();
        var rv = await outlookFolderToDb_FailsAsync(Misc.qFail);
        var rowsAdded = await _db.TrySaveReportAsync("OutlookToDb.cs");
        tb1.Text += rv;
        Debug.WriteLine(rv);
        loadVwSrcs(App.Now);
      }
      catch (Exception ex) { ex.Pop(); }
      finally { spCtlrPnl.IsEnabled = true; }
    }
    async void onDoLater(object s = null, RoutedEventArgs e = null) => await onDoLater_();
    public async Task onDoLater_()
    {
      spCtlrPnl.IsEnabled = false;

      try
      {
        var sw = Stopwatch.StartNew();
        var rv = await outlookFolderToDb_LaterAsync(Misc.qLate);
        var rowsAdded = await _db.TrySaveReportAsync("OutlookToDb.cs");
        tb1.Text += rv;
        Debug.WriteLine(rv);
        loadVwSrcs(App.Now);
      }
      catch (Exception ex) { ex.Pop(); }
      finally { spCtlrPnl.IsEnabled = true; }
    }
    async void onDoDoneR(object s = null, RoutedEventArgs e = null) => await onDoDoneR_();
    public async Task onDoDoneR_()
    {
      spCtlrPnl.IsEnabled = false;

      try
      {
        var sw = Stopwatch.StartNew();
        var rv = await outlookFolderToDb_DoneRAsync(Misc.qRcvdDone);
        var rowsAdded = await _db.TrySaveReportAsync("OutlookToDb.cs");
        tb1.Text += rv;
        Debug.WriteLine(rv);
        loadVwSrcs(App.Now);
      }
      catch (Exception ex) { ex.Pop(); }
      finally { spCtlrPnl.IsEnabled = true; }
    }

    public static async Task<bool> CheckInsertEMailEHistAsync(A0DbContext _db, string email, string firstName, string lastName, string subject, string body, DateTime timeRecdSent, string isRcvd, string RS)
    {
      var em = await checkInsertEMailAsync(_db, email, firstName, lastName, isRcvd);
      if (em == null) return false;

      await checkInsertEHistAsync(_db, subject, body, timeRecdSent, RS, em);
      var isNew = em?.AddedAt == App.Now;
      return isNew;
    }
    async Task<TupleSubst> findInsertEmailsFromBodyAsync(string body, string firstName, string lastName, string originalSenderEmail)
    {
      var newEmail = OutlookHelper.FindEmails(body);
      var isAnyNew = false;

      for (var i = 0; i < newEmail.Length; i++)
      {
        if (!string.IsNullOrEmpty(newEmail[i]))
        {
          var fln = OutlookHelper.figureOutFLNameFromBody(body, newEmail[i]);
          var em = await checkInsertEMailAsync(_db, newEmail[i], fln.Item1, fln.Item2, $"..from body (sender: {originalSenderEmail}). ");
          if (!isAnyNew) isAnyNew = em?.AddedAt == App.Now;
        }
      }

      return new TupleSubst { HasNewEmails = isAnyNew, newEmails = newEmail };
    }
    class TupleSubst { public bool HasNewEmails { get; set; } public string[] newEmails { get; set; } }

    async Task<string> outlookFolderToDb_ReglrAsync(string folderName)
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

        Debug.WriteLine($"\n ****** {items.Count,4}   IPM.Note   items in  {folderName}:");
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
                var senderEmail = OutlookHelper.figureOutSenderEmail(mailItem);
                var senderFLNme = OutlookHelper.figureOutSenderFLName(mailItem, senderEmail);

                var isNew = await checkDbInsertIfMissing_sender(mailItem, senderEmail, "..from  Q  folder. "); // checkInsertInotDbEMailAndEHistAsync(senderEmail, senderFLNme.Item1, senderFLNme.Item2, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, $"..was a sender", "R");  //foreach (OL.Recipient r in item.Recipients) ... includes potential CC addresses but appears as NEW and gets added ..probably because of wrong direction recvd/sent.				
                if (isNew) newEmailsAdded++;
                report += OutlookHelper.reportLine(folderName, senderEmail, isNew);

                if (!string.IsNullOrEmpty(mailItem.Body))
                {
                  var ii = await findInsertEmailsFromBodyAsync(mailItem.Body, senderFLNme.Item1, senderFLNme.Item2, senderEmail); //if it's via Indeed - name is in the SenderName. Otherwise, it maybe away redirect to a colleague.
                  if (ii.HasNewEmails)
                  {
                    for (var i = 0; i < ii.newEmails.Length; i++) { if (!string.IsNullOrEmpty(ii.newEmails[i])) { newEmailsAdded++; report += OutlookHelper.reportLine(folderName, ii.newEmails[i], isNew); } }
                  }
                }

                Debug.Write($"\n{ttl,2})  rcvd: {mailItem.ReceivedTime:yyyy-MMM-dd}  {senderEmail,-40}     {mailItem.Recipients.Count} rcpnts: (");
                foreach (OL.Recipient re in mailItem.Recipients)
                {
                  var ccFLName = OutlookHelper.figureOutSenderFLName(re.Name, re.Address);

                  var email = await checkInsertEMailAsync(_db, re.Address, ccFLName.Item1, ccFLName.Item2, $"..was a CC of {senderEmail} on {mailItem.SentOn:y-MM-dd HH:mm}. ");
                  isNew = email?.AddedAt == App.Now;
                  if (isNew) newEmailsAdded++;
                  report += OutlookHelper.reportLine(folderName, re.Address, isNew);
                }

                OutlookHelper.moveIt(rcvdDoneFolder, mailItem);
              }
              else if (folderName == Misc.qSent)
              {
                foreach (OL.Recipient re in mailItem.Recipients) // must use ReplyAll for this to work
                {
                  var senderFLNme = OutlookHelper.figureOutSenderFLName(re.Name, re.Address);

                  var isNew = await CheckInsertEMailEHistAsync(_db, re.Address, senderFLNme.Item1, senderFLNme.Item2, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, $"..from Sent folder. ", "S");
                  if (isNew) { newEmailsAdded++; }
                  report += OutlookHelper.reportLine(folderName, re.Address, isNew);
                }

                var trgFolder = (mailItem.Subject ?? "").StartsWith(QStatusBroadcaster.Asu) ? deletedsFolder : sentDoneFolder; // delete Avali-ty broadcasts.
                OutlookHelper.moveIt(trgFolder, mailItem);
              }
            }
            catch (Exception ex) { ex.Pop($"senderEmail: {mailItem?.SenderEmailAddress}. Report: {report}."); }
          } // for
#if DEBUG
        } while (false);
#else
        } while ((items = _oh.GetItemsFromFolder(folderName, "IPM.Note")).Count > 0); //not sure why, but it keeps skipping/missing items when  Move to OL folder, thus, this logic.
#endif

        _newEmailsAdded += newEmailsAdded;
        report += OutlookHelper.reportSectionTtl(folderName, cnt, newEmailsAdded);
      }
      catch (Exception ex) { ex.Pop(); }
      finally { Debug.WriteLine(""); }
      return report;
    }
    async Task<string> outlookFolderToDb_FailsAsync(string folderName)
    {
      var report = "";
      int ttl0 = 0, newBansAdded = 0, newEmailsAdded = 0;
      try
      {
        var rcvdDoneFolder = _oh.GetMapiFOlder(Misc.qRcvdDone);
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
                var emr = _db.EMails.Find(senderEmail);
                if (emr == null)
                {
                  var senderFLNme = OutlookHelper.figureOutSenderFLName(reportItem, senderEmail);
                  var isNew = await CheckInsertEMailEHistAsync(_db, senderEmail, senderFLNme.Item1, senderFLNme.Item2, reportItem.Subject, reportItem.Body, reportItem.CreationTime, "..banned upon delivery fail BUT not existed !!! ", "R");
                  if (isNew) { newEmailsAdded++; }
                }
                else
                {
                  banPremanentlyInDB(ref report, ref newBansAdded, senderEmail, "Delivery failed (a) ");
                }

                OutlookHelper.moveIt(rcvdDoneFolder, reportItem);
              }
              else if (item is OL.MailItem mailItem)
              {
                var senderEmail = OutlookHelper.RemoveBadEmailParts(mailItem.SenderEmailAddress);
                var emr = _db.EMails.Find(senderEmail);
                if (emr == null)
                {
                  var isNew = await checkDbInsertIfMissing_sender(mailItem, senderEmail, "..banned upon delivery fail BUT not existed !!! "); // checkInsertInotDbEMailAndEHistAsync(senderEmail, senderFLNme.Item1, senderFLNme.Item2, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, "..banned upon delivery fail BUT not existed !!!", "R");
                  if (isNew) { newEmailsAdded++; }
                }
                else
                {
                  banPremanentlyInDB(ref report, ref newBansAdded, senderEmail, "Delivery failed (b) ");
                }

                foreach (var emailFromBody in OutlookHelper.FindEmails(mailItem.Body))
                {
                  // banPremanentlyInDB(ref report, ref newBansAdded, emailFromBody, "Delivery failed (c) "); <== //todo: restore all %Delivery failed (c)%, since in the body usually alternative contacts are mentioned.

                  var emr2 = _db.EMails.Find(emailFromBody);
                  if (emr2 == null)
                  {
                    var fln = OutlookHelper.figureOutFLNameFromBody(mailItem.Body, emailFromBody);
                    var isNew = await CheckInsertEMailEHistAsync(_db, emailFromBody, fln.Item1, fln.Item2, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, "..alt contact from Delvery-Fail body. ", "A");
                    if (isNew) { newEmailsAdded++; }
                  }
                }

                OutlookHelper.moveIt(rcvdDoneFolder, mailItem);
              }
              else if (Debugger.IsAttached)
                Debugger.Break();
            }
            catch (Exception ex) { ex.Pop($"New  unfinished Aug 2019:{item.GetType().Name}."); }
          }

          itemsFailes = _oh.GetItemsFromFolder(folderName);
        } while (prev != itemsFailes.Count);
      }
      catch (Exception ex) { ex.Pop(); }

      _newEmailsAdded += newEmailsAdded;
      report += OutlookHelper.reportSectionTtl(folderName, ttl0, newBansAdded, newEmailsAdded);
      return report;
    }
    async Task<string> outlookFolderToDb_LaterAsync(string folderName)
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
                var isNew = await checkDbInsertIfMissing_sender(mailItem, OutlookHelper.RemoveBadEmailParts(mailItem.SenderEmailAddress), "..was on vaction && not existed in DB ?!?! ");
                if (isNew) newEmailsAdded++;

                foreach (var emailFromBody in OutlookHelper.FindEmails(mailItem.Body))
                {
                  var emr2 = _db.EMails.Find(emailFromBody);
                  if (emr2 == null)
                  {
                    var fln = OutlookHelper.figureOutFLNameFromBody(mailItem.Body, emailFromBody);
                    isNew = await CheckInsertEMailEHistAsync(_db, emailFromBody, fln.Item1, fln.Item2, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, "..from I'm-Away body as alt contact ", "A");
                    if (isNew) { newEmailsAdded++; }
                  }
                }

                if (App.Now > mailItem.ReceivedTime.AddDays(10)) // bad place ... but!
                {
                  var fnm = _db.EMails.Find(mailItem.SenderEmailAddress)?.FName ?? OutlookHelper.figureOutSenderFLName(mailItem, mailItem.SenderEmailAddress).Item1;
                  var scs = await QStatusBroadcaster.SendLetter_UpdateDb(true, mailItem.SenderEmailAddress, fnm);
                  if (scs)
                    OutlookHelper.moveIt(rcvdDoneFolder, mailItem);
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
      report += OutlookHelper.reportSectionTtl(folderName, ttl0, newBansAdded, newEmailsAdded);
      return report;
    }
    async Task<string> outlookFolderToDb_DoneRAsync(string folderName)
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
              senderEmail = OutlookHelper.RemoveBadEmailParts(senderEmail);
              if (!OutlookHelper.ValidEmailAddress(senderEmail)) { tb1.Text += $" ! {senderEmail}  \t <- invalid!!!\r\n"; continue; }

              var emr = _db.EMails.Find(senderEmail);
              if (emr == null)
              {
                var senderFLNme = OutlookHelper.figureOutSenderFLName(reportItem, senderEmail);
                var isNew = await CheckInsertEMailEHistAsync(_db, senderEmail, senderFLNme.Item1, senderFLNme.Item2, reportItem.Subject, "under constr-n", reportItem.CreationTime, msg, "R");
                if (isNew) { newEmailsAdded++; tb1.Text += $" * {senderEmail}\r\n"; }
              }
              rptLine += $"report\t{senderEmail,40}  {reportItem.CreationTime:yyyy-MM-dd}  {reportItem.Subject,-80} \t [no body - too slow and wrong]";
            }
            else if (item is OL.MailItem mailItem)
            {
              senderEmail = OutlookHelper.RemoveBadEmailParts(mailItem.SenderEmailAddress);
              if (!OutlookHelper.ValidEmailAddress(senderEmail)) { tb1.Text += $" ! {senderEmail}  \t <- invalid!!!\r\n"; continue; }

              var isNew = await checkDbInsertIfMissing_sender(mailItem, senderEmail, msg);
              if (isNew) { newEmailsAdded++; tb1.Text += $" * {senderEmail}\r\n"; }

              foreach (var emailFromBody in OutlookHelper.FindEmails(mailItem.Body))
              {
                senderEmail = OutlookHelper.RemoveBadEmailParts(emailFromBody);
                if (!OutlookHelper.ValidEmailAddress(senderEmail)) { tb1.Text += $" ! {senderEmail}  \t <- invalid!!!\r\n"; continue; }

                var emr2 = _db.EMails.Find(senderEmail);
                if (emr2 == null)
                {
                  var fln = OutlookHelper.figureOutFLNameFromBody(mailItem.Body, senderEmail);
                  isNew = await CheckInsertEMailEHistAsync(_db, senderEmail, fln.Item1, fln.Item2, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, "..from body. ", "R");
                  if (isNew) { newEmailsAdded++; tb1.Text += $" * {senderEmail}\r\n"; }
                }

                rptLine += $"body\t{senderEmail,40}  {mailItem.CreationTime:yyyy-MM-dd}  {mailItem.Subject,-80}{oneLineAndTrunkate(mailItem.Body)}   ";
              }

              Debug.Write($"\n{ttl,2})  rcvd: {mailItem.ReceivedTime:yyyy-MMM-dd}  {senderEmail,-40}     {mailItem.Recipients.Count} rcpnts: (");
              var cnt = 0;
              foreach (OL.Recipient re in mailItem.Recipients)
              {
                var ccFLName = OutlookHelper.figureOutSenderFLName(re.Name, re.Address);

                var email = await checkInsertEMailAsync(_db, re.Address, ccFLName.Item1, ccFLName.Item2, $"..CC  {mailItem.SentOn:yyyy-MM-dd}  {++cnt,2}/{mailItem.Recipients.Count,-2}  by {senderEmail}. ");
                isNew = email?.AddedAt == App.Now;
                if (isNew) newEmailsAdded++;
                rptLine += OutlookHelper.reportLine(folderName, re.Address, isNew);
              }

              rptLine += $"mail\t{senderEmail,40}  {mailItem.CreationTime:yyyy-MM-dd}  {mailItem.Subject,-80}{oneLineAndTrunkate(mailItem.Body)}   ";
            }
            else if (item is OL.AppointmentItem itm0)  /**/ { tb1.Text += $" ? Appointment {itm0.CreationTime:yyyy-MM-dd} {itm0.Subject} \t {oneLineAndTrunkate(itm0.Body)} \r\n"; }
            else if (item is OL.DistListItem itm1)     /**/ { tb1.Text += $" ? DistList    {itm1.CreationTime:yyyy-MM-dd} {itm1.Subject} \t {oneLineAndTrunkate(itm1.Body)} \r\n"; }
            else if (item is OL.DocumentItem itm2)     /**/ { tb1.Text += $" ? Document    {itm2.CreationTime:yyyy-MM-dd} {itm2.Subject} \t {oneLineAndTrunkate(itm2.Body)} \r\n"; }
            else if (item is OL.JournalItem itm3)      /**/ { tb1.Text += $" ? Journal     {itm3.CreationTime:yyyy-MM-dd} {itm3.Subject} \t {oneLineAndTrunkate(itm3.Body)} \r\n"; }
            else if (item is OL.MeetingItem itm4)      /**/ { tb1.Text += $" ? Meeting     {itm4.CreationTime:yyyy-MM-dd} {itm4.Subject} \t {oneLineAndTrunkate(itm4.Body)} \r\n"; }
            else if (item is OL.MobileItem itm5)       /**/ { tb1.Text += $" ? Mobile      {itm5.CreationTime:yyyy-MM-dd} {itm5.Subject} \t {oneLineAndTrunkate(itm5.Body)} \r\n"; }
            else if (item is OL.NoteItem itm6)         /**/ { tb1.Text += $" ? Note        {itm6.CreationTime:yyyy-MM-dd} {itm6.Subject} \t {oneLineAndTrunkate(itm6.Body)} \r\n"; }
            else if (item is OL.TaskItem itm7)         /**/ { tb1.Text += $" ? Task        {itm7.CreationTime:yyyy-MM-dd} {itm7.Subject} \t {oneLineAndTrunkate(itm7.Body)} \r\n"; }
            else if (Debugger.IsAttached) { Debug.WriteLine($"AP: not procesed OL_type: {item.GetType().Name}"); Debugger.Break(); } else throw new Exception("AP: Review this case of missing type: must be something worth processing.");

            Debug.WriteLine($"{rptLine}");
          }
          catch (Exception ex) { ex.Pop($":{senderEmail}."); }

          lblMetaHeader.Content = $" ... found / current / ttl:  {newEmailsAdded} / {++ttlProcessed:N0} / {ttl:N0} ...";

          if (ttlProcessed % 10 == 0) await Task.Delay(10); else await Task.Yield();
        }
      }
      catch (Exception ex) { ex.Pop(); }

      _newEmailsAdded += newEmailsAdded;
      report___ += OutlookHelper.reportSectionTtl(folderName, ttlProcessed, 0, newEmailsAdded);
      return report___;
    }

    string oneLineAndTrunkate(string body, int max = 55)
    {
      if (string.IsNullOrEmpty(body)) return "";

      body = body.Replace("\r", " CR ").Replace("\n", " LF ");

      var len = body.Length;

      return len <= max ? body : (body.Substring(0, max) + "...");
    }

    async Task<bool> checkDbInsertIfMissing_sender(OL.MailItem mailItem, string senderEmail, string note)
    {
      var row = _db.EMails.Find(senderEmail);
      if (row == null)
      {
        var senderFLNme = OutlookHelper.figureOutSenderFLName(mailItem, senderEmail);
        var isNew = await CheckInsertEMailEHistAsync(_db, senderEmail, senderFLNme.Item1, senderFLNme.Item2, mailItem.Subject, mailItem.Body, mailItem.ReceivedTime, note, "R");
        return isNew;
      }

      return false;
    }
    public static async Task<EMail> checkInsertEMailAsync(A0DbContext _db, string email, string firstName, string lastName, string notes)
    {
      const int maxLen = 256;

      if (email.EndsWith("@msg.monster.com")) // ~ 3212846259f94b158701020f5ca8ac4e@msg.monster.com
        return null;

      if (email.Length > maxLen)
        email = email.Substring(email.Length - maxLen, maxLen);

      var em = _db.EMails.Find(email);
      if (em == null)
      {
        var agency = OutlookHelper.GetCompanyName(email);

        if (_db.Agencies.FirstOrDefault(r => string.Compare(r.ID, agency, true) == 0) == null)
        {
          _db.Agencies.Add(new Agency
          {
            ID = agency.Length > maxLen ? agency.Substring(agency.Length - maxLen, maxLen) : agency,
            AddedAt = AvailStatusEmailer.App.Now
          });
        }

        em = _db.EMails.Add(new EMail
        {
          ID = email.Length > maxLen ? email.Substring(email.Length - maxLen, maxLen) : email,
          Company = agency,
          FName = firstName,
          LName = lastName,
          Notes = notes,
          AddedAt = AvailStatusEmailer.App.Now,
          ReSendAfter = null,
          NotifyPriority = 99
        });

        await _db.TrySaveReportAsync("checkInsertEMail");
      }

      return em;
    }
    public static async Task checkInsertEHistAsync(A0DbContext _db, string subject, string body, DateTime timeRecdSent, string rs, EMail em)
    {
      //insertEMailEHistItem(isRcvd, timeRecdSent, em, subject, body);		}		void insertEMailEHistItem(bool isRcvd, DateTime timeRecdSent, EMail em, string subject, string body)		{
      try
      {
        var gt = timeRecdSent.AddMinutes(-5);
        var lt = timeRecdSent.AddMinutes(+5);         //var ch = isRcvd ? ctx.EHists.Where(p => p.EmailedAt.HasValue && gt < p.EmailedAt.Value && p.EmailedAt.Value < lt && p.EMailId == id.ID) : ctx.EHists.Where(p => p.EmailedAt.HasValue && gt < p.EmailedAt.Value && p.EmailedAt.Value < lt && p.EMailId == id.ID); if (ch.Count() < 1)
        var eh = _db.EHists./*Local.*/FirstOrDefault(p => p.RecivedOrSent == rs && p.EMailID == em.ID && gt < p.EmailedAt && p.EmailedAt < lt);
        if (eh == null)
        {
          var newEH = new EHist
          {
            RecivedOrSent = rs,
            EMail = em,
            LetterBody = string.IsNullOrEmpty(body) ? "" : body.Replace("\n\n\n", "\n\n").Replace("\n\n", "\n").Replace("\r\n\r\n\r\n", "\n\n").Replace("\r\n\r\n", "\n"),
            LetterSubject = subject,
            AddedAt = AvailStatusEmailer.App.Now,
            Notes = "",
            EmailedAt = timeRecdSent
          };
          var newCH2 = _db.EHists.Add(newEH);

          await _db.TrySaveReportAsync("checkInsertEHist");
        }
      }
      catch (Exception ex) { ex.Pop(); }
    }

    void banPremanentlyInDB(ref string rv, ref int newBansAdded, string email, string rsn)
    {
      var emr = _db.EMails.Find(email);
      if (emr == null)
      {
        if (Debugger.IsAttached) Debugger.Break(); else throw new Exception("AP: Review this case of missing row: must be something wrong.");
      }
      else
      {
        if ((emr.PermBanReason == null || !emr.PermBanReason.Contains(rsn))) // if new reason
        {
          emr.PermBanReason += (rsn + App.Now.ToString("yyyy-MM-dd"));
          emr.ModifiedAt = App.Now;
          rv += ($"{Misc.qFail,-15}  {email,-60}\tnew reason: {rsn}\n");
          newBansAdded++;
        }
        else if (emr.PermBanReason.Contains(rsn)) // same reason already there
        {
          rv += ($"{Misc.qFail,-15}  {email,-60}\talready banned with the same reason: {rsn}\n");
        }
        else
        {
          if (Debugger.IsAttached) Debugger.Break(); else throw new Exception("AP: Review this case of missing row: must be something wrong.");
        }
      }
    }
    void loadVwSrcs(DateTime before)
    {
      //((CollectionViewSource)(FindResource("eMailVwSrc"))).Source = ctx.EMails.Where(p => p.AddedAt >= before).ToList();
      //((CollectionViewSource)(FindResource("eMailVwSrc"))).View.MoveCurrentTo(null);
      //((CollectionViewSource)(FindResource("eHistVwSrc"))).Source = ctx.EHists.Where(p => p.AddedAt >= before).ToList();
      //((CollectionViewSource)(FindResource("eHistVwSrc"))).View.MoveCurrentTo(null);
    }

    static void testOneKey(OL.ReportItem item, string key, bool isBinary = false)
    {
      try
      {
        Debug.Write($"==> {key}: ");

        var pa = item.PropertyAccessor;
        var url = $"http://schemas.microsoft.com/mapi/proptag/{key}";

        object prop;
        prop = pa.GetProperty(url);

        if (isBinary)
          prop = pa.BinaryToString(pa.GetProperty(url));

        if (prop is byte[])
        {
          var snd = pa.BinaryToString(prop);
          Debug.Write($"  === snd: {snd.Replace("\n", "-LF-").Replace("\r", "-CR-")}");

          var s = item.Session.GetAddressEntryFromID(snd);
          Debug.Write($"  *** GetExchangeDistributionList(): {s.GetExchangeDistributionList().ToString().Replace("\n", "-LF-").Replace("\r", "-CR-")}"); //.PrimarySmtpAddress;
          Debug.Write($"  *** Address: {s.Address}"); //.PrimarySmtpAddress;
        }
        else
        {
          Debug.Write($"==> {prop.GetType().Name}  {prop.ToString().Replace("\n", "-LF-").Replace("\r", "-CR-")}");
        }
      }
      catch (Exception ex) { Debug.Write($"  !!! ex: {ex.Message.Replace("\n", "-LF-").Replace("\r", "-CR-")}"); }

      Debug.WriteLine($"^^^^^^^^^^^^^^^^^^^^^^^^^");
    }
    static void testAllKeys(OL.ReportItem item) // body for report item ...is hard to find; need more time, but really, who cares. // Aug 2019
    {
      if (item.Body.Length > 0)
      {
        Debug.WriteLine($"\n\n@@@ Body: {item.Body}");
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



      Debug.WriteLine($"++++++++++++++++++++++++++++++++++++++ --------------------\n");
    }
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
