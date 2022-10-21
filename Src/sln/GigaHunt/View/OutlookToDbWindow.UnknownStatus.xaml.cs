using Outlook = Microsoft.Office.Interop.Outlook;

namespace GigaHunt.View
{
  public partial class OutlookToDbWindowUnkn : WpfUserControlLib.Base.WindowBase
  {
    readonly QStatsRlsContext _db = QStatsRlsContext.Create();
    const string note = "note3";
    readonly DateTime _now = GigaHunt.App.Now;

    public OutlookToDbWindowUnkn() { InitializeComponent(); themeSelector1.ThemeApplier = ApplyTheme; }

    async void onLoadedAsync(object s, RoutedEventArgs e) => await btnRefreshDbFromOutlookAsync();

    async System.Threading.Tasks.Task checkInsertEMailAndEHistAsync(string email, string flName, string subject, string body, DateTime timeRecdSent, bool isRcvd)
    {
      //var v1 = ctx.Emails.ToList().Find(p => p.Id == email);
      //var v2 = ctx.Emails.ToList().FirstOrDefault(p => p.Id == email);
      //var v4 = ctx.Emails.Where(p => p.Id == email);
      var em = _db.Emails.Find(email);
      if (em == null)
      {
        em = _db.Emails.Add(new Email { Id = email, Company = getCompanyName(email), Fname = new FirstLastNameParser(flName).FirstName, Lname = new FirstLastNameParser(flName).LastName, Notes = note, AddedAt = _now }).Entity;
        await _db.TrySaveReportAsync("OutlookToDb.cs");
      }
      //insertEMailEHistItem(isRcvd, timeRecdSent, em, subject, body);		}		void insertEMailEHistItem(bool isRcvd, DateTime timeRecdSent, Email em, string subject, string body)		{
      try
      {
        var gt = timeRecdSent.AddMinutes(-5);
        var lt = timeRecdSent.AddMinutes(+5);         //var ch = isRcvd ? ctx.EHists.Where(p => p.EmailedAt.HasValue && gt < p.EmailedAt.Value && p.EmailedAt.Value < lt && p.EMailId == id.Id) : ctx.EHists.Where(p => p.EmailedAt.HasValue && gt < p.EmailedAt.Value && p.EmailedAt.Value < lt && p.EMailId == id.Id); if (ch.Count() < 1)
        var eh = _db.Ehists./*Local.*/FirstOrDefault(p => p.RecivedOrSent == (isRcvd ? "R" : "S") && p.EmailId == em.Id && gt < p.EmailedAt && p.EmailedAt < lt);
        if (eh == null)
        {
          var newEH = new Ehist { RecivedOrSent = (isRcvd ? "R" : "S"), Email = em, LetterBody = body, LetterSubject = subject, AddedAt = _now, Notes = note, EmailedAt = timeRecdSent };
          var newCH2 = _db.Ehists.Add(newEH);
        }
      }
      catch (Exception ex) { ex.Pop(); ; }
    }
    string getCompanyName(string email)
    {
      var cmpny = email.IndexOf('@') < 0 ? email : email.Split('@')[1].Split('.')[0];
      return cmpny;
    }
    async Task outlookFolderToDbAsync(string folderName)
    {
      try
      {
        var outlkFolder = new Outlook.Application().Session.Stores["pigida@hotmail.com"].GetRootFolder().Folders[folderName] as Outlook.Folder;
        var items = outlkFolder.Items.Restrict("[MessageClass] = 'IPM.Note'");
        var cnt = items.Count;
        foreach (Outlook.MailItem item in items)
        {
          try
          {
            var email = folderName != Misc.qRcvd && item.Recipients.Count > 0 ? item.Recipients[item.Recipients.Count].Address : item.SenderEmailAddress;
            var ename = folderName != Misc.qRcvd && item.Recipients.Count > 0 ? item.Recipients[item.Recipients.Count].Name : item.SenderName;
            Write(string.Format("\n{0,4})  {1}  {2,-32}{3,-32}{4,-32}{5}", cnt--, item.ReceivedTime.ToString("yyyy-MMM-dd"), ename, email, item.SentOnBehalfOfName, item.Recipients.Count));  //
            foreach (Outlook.Recipient re in item.Recipients) Write(string.Format("\n{0,52}{1}", "", re.Address));

            if (cnt == 55) Write("");

            if (folderName == Misc.qRcvd)
            {
              var senderEmail = item.SentOnBehalfOfName.Contains("@") ? item.SentOnBehalfOfName : item.SenderEmailAddress;
              await checkInsertEMailAndEHistAsync(senderEmail, item.SenderName, item.Subject, item.Body, item.ReceivedTime, folderName == Misc.qRcvd);
              //foreach (Outlook.Recipient r in item.Recipients) ... includes potential CC addresses but appears as NEW and gets added ..probably because of wrong direction recvd/sent.
              //			checkInsertEMailAndEHist(r.Address, r.Name, item.Subject, item.Body, item.ReceivedTime, folder == Misc.qRcvd);
            }
            else
              foreach (Outlook.Recipient r in item.Recipients) // must use ReplyAll for this to work
                await checkInsertEMailAndEHistAsync(r.Address, r.Name, item.Subject, item.Body, item.ReceivedTime, folderName == Misc.qRcvd);
          }
          catch (Exception ex) { ex.Pop(); ; }
        }
      }
      catch (Exception ex) { ex.Pop(); ; }
      finally { WriteLine(""); }
    }
    void loadVwSrcs(System.DateTime before)
    {
      //((CollectionViewSource)(FindResource("eMailVwSrc"))).Source = ctx.Emails.Where(p => p.AddedAt >= before).ToList();
      //((CollectionViewSource)(FindResource("eMailVwSrc"))).View.MoveCurrentTo(null);
      //((CollectionViewSource)(FindResource("eHistVwSrc"))).Source = ctx.EHists.Where(p => p.AddedAt >= before).ToList();
      //((CollectionViewSource)(FindResource("eHistVwSrc"))).View.MoveCurrentTo(null);
    }
    async Task btnRefreshDbFromOutlookAsync()
    {
      try
      {
        var sw = Stopwatch.StartNew();

        _db.Emails.Load();
        _db.Ehists.Load();

        await outlookFolderToDbAsync(Misc.qRcvd);
        await outlookFolderToDbAsync(Misc.qSent);

        var rowsAdded = await _db.TrySaveReportAsync("OutlookToDb.cs");
        tb1.Text = tbkTitle.Text = Title = string.Format("{0} rows added to EHist in {1:N1} sec (~15)", rowsAdded, sw.ElapsedMilliseconds * .001);
        WriteLine(Title);
        loadVwSrcs(_now);
      }
      catch (Exception ex) { ex.Pop(); ; }
    }
  }
}
