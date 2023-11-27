namespace GigaHunt.AsLink;

public partial class OutlookHelper6
{
  readonly OL.Application? _olApp;
  readonly OL.MAPIFolder? _contactsFolder;
  readonly int _customLetersSentThreshold = 3; // to become an Outlook contact, must have at least 3 letters sent.
  static readonly char[] _delim = new[] { ' ', '.', ',', ':', ';', '\r', '\n', '\'', '"', '_' };
  int _updatedCount, _addedCount;

  public OutlookHelper6()
  {
    try
    {
      _olApp = new OL.Application();

      MyStore = _olApp.Session.Stores["alex.pigida@outlook.com"];
      _contactsFolder = MyStore.GetDefaultFolder(OL.OlDefaultFolders.olFolderContacts);        // this.Application.GetNamespace("MAPI").GetDefaultFolder(OL.OlDefaultFolders.olFolderContacts);                //_deletedsFolder = _store.GetDefaultFolder(OL.OlDefaultFolders.olFolderDeletedItems);
    }
    catch (COMException ex) { ex.Log("I think this is it... (ap: Jun`20)"); throw; }
    catch (Exception ex) { ex.Log(); throw; }
  }

  public OL.Store? MyStore { get; }
  public OL.Items? GetItemsFromFolder(string folder, int old)
  {
    try
    {
      var folder0 = MyStore?.GetRootFolder().Folders[folder] as OL.Folder;
      var items = folder0?.Items.Restrict("[MessageClass] = 'IPM.Note'");
      return items;
    }
    catch (Exception ex) { ex.Log(folder); throw; }
  }
  public OL.Items? GetDeliveryFailedItems()
  {
    try
    {
      var folder = MyStore?.GetRootFolder().Folders[OuFolder.qRcvd].Folders["Fails"] as OL.Folder;
      var itemss = folder?.Items.Restrict("[MessageClass] = 'REPORT.IPM.Note.NDR'");
      WriteLine($"***        Fails: {itemss?.Count}");
      return itemss;
    }
    catch (Exception ex) { ex.Log(@"Q\Fails"); throw; }
  }
  public OL.Items GetItemsFromFolder(string folderPath, string? messageClass = null) // IPM.Note, REPORT.IPM.Note.NDR
  {
    try
    {
      var folder = GetMapiFOlder(folderPath);
      ArgumentNullException.ThrowIfNull(folder, "folder is nul @@@@@@@@@@@@@@@-");

      var itemss = messageClass == null ? folder.Items : folder?.Items.Restrict($"[MessageClass] = '{messageClass}'");      //...WriteLine($" *** {folderPath,24}: {itemss.Count}");
      ArgumentNullException.ThrowIfNull(itemss, "itemss is nul @@@@@@@@@@@@@@@-");

      return itemss;
    }
    catch (Exception ex) { ex.Log(@"Q\Fails"); throw; }
  }
  public OL.Items? GetToResendItems()
  {
    try
    {
      var folder = MyStore?.GetRootFolder().Folders[OuFolder.qRcvd].Folders["ToReSend"] as OL.Folder;
      var itemss = folder?.Items.Restrict("[MessageClass] = 'IPM.Note'");
      return itemss;
    }
    catch (Exception ex) { ex.Log(@"Q\ToReSend"); throw; }
  }
  public OL.MAPIFolder? GetMapiFOlder(string folderPath)
  {
    var folderParts = folderPath.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
    var folder = MyStore?.GetRootFolder();
    for (var i = 0; i < folderParts.Length; i++)
      folder = folder?.Folders[folderParts[i]] as OL.Folder;

    return folder;
  }

  public async Task<string> OutlookUndeleteContactsAsync(QstatsRlsContext db)
  {
    Trace.WriteLine("Synchronous action... usually takes 5 minutes.");

    var sw = Stopwatch.StartNew();

    db.Emails.Load();
    db.Agencies.Load();

    ArgumentNullException.ThrowIfNull(MyStore, "MyStore is nul @@@@@@@@@@@@@@@-");

    UndeleteContacts(MyStore.GetDefaultFolder(OL.OlDefaultFolders.olFolderContacts), db, "Contacts");
    UndeleteContacts(MyStore.GetDefaultFolder(OL.OlDefaultFolders.olFolderDeletedItems), db, "Deleted Items");

    Trace.WriteLine($"All done. Took {sw.Elapsed.TotalMinutes:N1} minutes.");

    var rep =      //db.TrySaveReport(); // 
      db.GetDbChangesReport();
    WriteLine($"{rep}");

    Task<string> saveAndUpdateMetadata()
    {
      return AgentAdminnWindowHelpers.SaveAndUpdateMetadata(db);
    }

    _ = await AgentAdminnWindowHelpers.CheckAskToSaveDispose_CanditdteForGlobalRepltAsync(db, false, saveAndUpdateMetadata);

    return rep;
  }

  public void FindContactByName(string lastName)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(_contactsFolder, "_contactsFolder is nul @@@@@@@@@@@@@@@-");
      var i = (OL.ContactItem)_contactsFolder.Items.Find(string.Format("[LastName]='{0}'", lastName)); // ..Format("[FirstName]='{0}' and [LastName]='{1}'", firstName, lastName));
      while (i != null)
      {
        i.Display(true);
        i = (OL.ContactItem)_contactsFolder.Items.FindNext();
      }
    }
    catch (Exception ex) { ex.Log(lastName); throw; }
  }
  public void FindContactByEmail(string email)
  {
    try
    {
      //var contact = (OL.ContactItem)contactsFolder.Items.Find(String.Format("[Email1Address]='{0}'", email.ToUpper())); // ..Format("[FirstName]='{0}' and [LastName]='{1}'", firstName, lastName));
      //while (contact != null)
      //{
      //  contact.Display(true);
      //  contact = contactsFolder.Items.FindNext();
      //}

      ArgumentNullException.ThrowIfNull(MyStore, "MyStore is nul @@@@@@@@@@@@@@@-");

      DbgListAllCOntacts(MyStore.GetDefaultFolder(OL.OlDefaultFolders.olFolderContacts));
      DbgListAllCOntacts(MyStore.GetDefaultFolder(OL.OlDefaultFolders.olFolderDeletedItems));
    }
    catch (Exception ex) { ex.Log("!!! MUST RUN OUTLOOK TO WORK !!!"); throw; }
  }
  public string SyncDbToOutlook(QstatsRlsContext db)
  {
    //AAV.Sys.Helpers.Chronoer.Start();
    var q = db.Emails.Where(r => string.IsNullOrEmpty(r.PermBanReason)
                            && r.Id.Contains('@')
                            && !r.Id.Contains('=')
                            && !r.Id.Contains("reply")
                            && !r.Id.Contains('\'')
                            && !r.Id.Contains('+')
                            && r.Ehists.Count(e => string.Compare(e.RecivedOrSent, "S", true) == 0 && !string.IsNullOrEmpty(e.LetterBody) && e.LetterBody.Length > 96) > _customLetersSentThreshold); //at least 2 letters sent (1 could be just an unanswered reply on their broadcast)

    var ttl = q.Count();
    WriteLine($"\r\n{ttl} eligible email contacts found");
    _addedCount = _updatedCount = 0;
    foreach (var em in q)
      AddUpdateOutlookContact(em);

    //Chronoer.Finish();
    return string.Format("\r\nTotal Outlook: {0} added, {1} updated / out of {2} eligibles. \r\n", _addedCount, _updatedCount, ttl);
  }
  public void AddUpdateOutlookContact(Email em)
  {
    try
    {
      var i = (OL.ContactItem?)_contactsFolder?.Items.Find(string.Format("[Email1Address]='{0}' or [Email2Address]='{0}' or [Email3Address]='{0}' ", em.Id));
      if (i != null)
      {
        MergeDbDataToOutlookContact(em, ref i, "by Email");
        return;
      }

      if (!string.IsNullOrWhiteSpace(em.Fname) && !string.IsNullOrWhiteSpace(em.Lname) && _contactsFolder?.Items.Find(string.Format("[FirstName]='{0}' and [LastName]='{1}'", em.Fname, em.Lname)) != null)
      {
        i = (OL.ContactItem)_contactsFolder.Items.Find(string.Format("[FirstName]='{0}' and [LastName]='{1}'", em.Fname, em.Lname));
        if (i != null)
        {
          MergeDbDataToOutlookContact(em, ref i, "by Name");
          return;
        }
      }

      CreateFromDbOutlookContact(em);
    }
    catch (Exception ex) { ex.Log("!!! MUST RUN OUTLOOK TO WORK !!!"); throw; }
  }

  void MergeDbDataToOutlookContact(Email em, ref OL.ContactItem i, string msg)
  {
    bool changed;
    while (i != null)
    {
      Debug.WriteLine("\r\nUpdating outlook: {0,12} {1,-12} - {2,32} - Body: {3} \r\n    with DB data: {4,12} {5,-12} - {6,32} - Body: {7}  ", i.FirstName, i.LastName, i.Email1Address, i.Body, em.Fname, em.Lname, em.Id, em.Notes);
      changed = false;

      if (!string.IsNullOrWhiteSpace(em.Notes))
        if (string.IsNullOrWhiteSpace(i.Body))
        {
          i.Body = em.Notes;
          changed = true;
        }
        else if (!i.Body.Contains(em.Notes))
        {
          i.Body += Environment.NewLine + Environment.NewLine + " -=-=- From QStats DB -=-=- [[" + Environment.NewLine + em.Notes + Environment.NewLine + "]] -=-=- From QStats DB -=-=-";
          changed = true;
        }

      if (string.IsNullOrWhiteSpace(i.Email1Address))                                                                                                                  /**/{ i.Email1Address = em.Id; changed = true; }
      else if (string.Compare(i.Email1Address, em.Id, true) != 0 && string.IsNullOrWhiteSpace(i.Email2Address))                                                       /**/{ i.Email2Address = em.Id; changed = true; }
      else if (string.Compare(i.Email1Address, em.Id, true) != 0 && string.Compare(i.Email2Address, em.Id, true) != 0 && string.IsNullOrWhiteSpace(i.Email3Address)) /**/{ i.Email3Address = em.Id; changed = true; }
      else if (string.Compare(i.Email1Address, em.Id, true) != 0 && string.Compare(i.Email2Address, em.Id, true) != 0 && string.Compare(i.Email3Address, em.Id, true) != 0)
      { i.Body += Environment.NewLine + Environment.NewLine + em.Id; changed = true; }

      if (string.IsNullOrWhiteSpace(i.FirstName) && !string.IsNullOrWhiteSpace(em.Fname))                  /**/ { changed = true; i.FirstName = em.Fname; }

      if (string.IsNullOrWhiteSpace(i.LastName) && !string.IsNullOrWhiteSpace(em.Lname))                   /**/ { changed = true; i.LastName = em.Lname; }

      if (string.IsNullOrWhiteSpace(i.CompanyName) && !string.IsNullOrWhiteSpace(em.Company))              /**/ { changed = true; i.CompanyName = em.Company; }

      if (string.IsNullOrWhiteSpace(i.BusinessTelephoneNumber) && !string.IsNullOrWhiteSpace(em.Phone))    /**/ { changed = true; i.BusinessTelephoneNumber = em.Phone; }

      if (changed)
      {
        if (string.IsNullOrWhiteSpace(i.Categories)) i.Categories = "AppAdded";
        i.User2 = msg;
        i.Save();
        _updatedCount++;
      }

      ArgumentNullException.ThrowIfNull(_contactsFolder, "contactsFolder is nul @@@@@@@@@@@@@@@-");

      i = (OL.ContactItem)_contactsFolder.Items.FindNext();
    }
  }
  void CreateFromDbOutlookContact(Email em)
  {
    ArgumentNullException.ThrowIfNull(_olApp, "olApp is nul @@@@@@@@@@@@@@@-");
    var i = (OL.ContactItem)_olApp.CreateItem(OL.OlItemType.olContactItem);
    i.Email1Address = em.Id;
    if (!string.IsNullOrWhiteSpace(em.Fname))    /**/ i.FirstName = em.Fname;
    if (!string.IsNullOrWhiteSpace(em.Lname))    /**/ i.LastName = em.Lname;
    if (!string.IsNullOrWhiteSpace(em.Company))  /**/ i.CompanyName = em.Company;
    if (!string.IsNullOrWhiteSpace(em.Phone))    /**/ i.BusinessTelephoneNumber = em.Phone;

    var q = em.Ehists.Where(e => string.Compare(e.RecivedOrSent, "S", true) == 0 && !string.IsNullOrWhiteSpace(e.LetterBody) && e.LetterBody.Length > 96);
    var m = q.Max(e => e.EmailedAt);
    var t = q.FirstOrDefault(r => r.EmailedAt == m);
    i.Body = " -=-=- [[From QStats DB (brandNew): -=-=- " + Environment.NewLine + (!string.IsNullOrWhiteSpace(em.Notes) ? em.Notes : "") +
      string.Format("{0}Added: {1:yyyy-MM-dd},  as of {2:yyyy-MM-dd} hand-written letters sent: {3},  {0}  first: {4:yyyy-MM-dd}  - last: {5:yyyy-MM-dd}:{0}{0}{6}{0} -=-=- EOFrom QStasts DB]] -=-=- {0}",
      Environment.NewLine,
      em.AddedAt,
      DateTime.Today,
      q.Count(),
      q.Min(e => e.EmailedAt),
      q.Max(e => e.EmailedAt),
      t == null ? "" : t.LetterBody?.Length > 222 ? t.LetterBody[..222] + " ..." : t.LetterBody);

    //i.Display(true);

    i.Categories = "AppAdded";
    i.User1 = "New from QStats DB";
    //i.CreationTime = GigaHunt.Chronoer._now;

    i.Save();
    _addedCount++;
  }

  static void DbgListAllCOntacts(OL.MAPIFolder folder)
  {
    WriteLine($"Folder {folder.Name} has total {folder.Items.Count} items: ");

    foreach (var o in folder.Items) //.Where(r => r==r))
      if (o is OL.ContactItem)          /**/{ var i = o as OL.ContactItem;    /**/ WriteLine($"C {i?.FirstName,16}\t{i?.LastName,-16}\t{i?.Email1Address,24}\t{i?.Subject,-48}\t{(string.IsNullOrWhiteSpace(i?.Body) ? "·" : i?.Body.Length > 50 ? i?.Body[..50] : i?.Body)}\t{i?.Account}"); }
      else if (o is OL.MailItem)        /**/{ var i = o as OL.MailItem;       /**/ WriteLine($"M {i?.To,-32}\t{i?.Subject,-48}\t"); }
      else if (o is OL.AppointmentItem) /**/{ var i = o as OL.AppointmentItem;/**/ WriteLine($"M {i?.Subject,-48}\t"); }
      else if (o is OL.MeetingItem)     /**/{ var i = o as OL.MeetingItem;    /**/ WriteLine($"M {i?.Subject,-48}\t"); }
      else if (o is OL.TaskItem)        /**/{ var i = o as OL.TaskItem;       /**/ WriteLine($"M {i?.Body,-48}\t"); }
      else
      {
        foreach (PropertyDescriptor descrip in TypeDescriptor.GetProperties(o))
          Write($" {descrip.Name}"); // if (descrip.Name == "Subject") { foreach (PropertyDescriptor descrip2 in TypeDescriptor.GetProperties(descrip)) { if (descrip2.Name == "sub attribute Name") { } } }

        Write($"\n");
      }
  }

  static void UndeleteContacts(OL.MAPIFolder folder, QstatsRlsContext QstatsRlsContext, string srcFolder)
  {
    WriteLine($"Folder {folder.Name} has total {folder.Items.Count} items: ");

    foreach (var o in folder.Items)
      if (o is OL.ContactItem item)
        AddUpdateToDb(item, QstatsRlsContext, srcFolder);
  }

  static void AddUpdateToDb(OL.ContactItem ci, QstatsRlsContext db, string srcFolder)
  {
    const int maxLen = 256;

    var emailId = "";
    if (!string.IsNullOrWhiteSpace(ci.Email1Address))                                               /**/emailId = ci.Email1Address;
    else if (!string.IsNullOrWhiteSpace(ci.Email2Address))                                          /**/emailId = ci.Email2Address;
    else if (!string.IsNullOrWhiteSpace(ci.Email3Address))                                          /**/emailId = ci.Email3Address;
    else if (!string.IsNullOrWhiteSpace(ci.FirstName) && !string.IsNullOrWhiteSpace(ci.LastName))   /**/emailId = $"{ci.FirstName}.{ci.LastName}@__UnKnwn__.com";
    else if (!string.IsNullOrWhiteSpace(ci.FirstName))                                              /**/emailId = $"{ci.FirstName}.__UnKnwn__@__UnKnwn__.com";
    else if (!string.IsNullOrWhiteSpace(ci.LastName))                                               /**/emailId = $"__UnKnwn__.{ci.LastName}@__UnKnwn__.com";
    else
    {
      WriteLine($"******************");
      return;
    }

    if (!string.IsNullOrWhiteSpace(ci.Body)) WriteLine($"{(string.IsNullOrWhiteSpace(ci.Body) ? "·" : ci.Body.Length > 50 ? ci.Body[..50] : ci.Body)}"); // <= strange thing: all bodies are empty.

    var phone = $"{ci.HomeTelephoneNumber} {ci.PrimaryTelephoneNumber} {ci.BusinessTelephoneNumber} {ci.Business2TelephoneNumber} {ci.MobileTelephoneNumber}".Replace("(", "").Replace(")", "-").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
    var agency = /*!string.IsNullOrWhiteSpace(ci.CompanyName) ? ci.CompanyName : */GetCompanyName(emailId);

    if (!db.Agencies.Local.Any(r => agency.Equals(r.Id, StringComparison.OrdinalIgnoreCase)))
      db.Agencies.Local.Add(new Agency
      {
        Id = agency.Length > maxLen ? agency.Substring(agency.Length - maxLen, maxLen) : agency,
        AddedAt = Chronoer.Now
      });

    WriteLine($"{emailId,32}\t{ci.FirstName,17} {ci.LastName,-21}\t{ci.JobTitle,-80}\t{phone}\t{(string.IsNullOrWhiteSpace(ci.Body) ? "·" : ci.Body.Length > 50 ? ci.Body[..50] : ci.Body)}");

    AddUpdateBassedOnGoodEmailId(ci, db, emailId, phone, agency, srcFolder);
  }
  public static string GetCompanyName(string email)
  {
    var indexofAt = email.IndexOf('@') + 1;
    var indexofEx = email.LastIndexOf('.');
    var len = indexofEx - indexofAt;

    if (len < 1)
      return email;

    var cmpny = email.Substring(indexofAt, len);
    return cmpny;
  }

  static void AddUpdateBassedOnGoodEmailId(OL.ContactItem ci, QstatsRlsContext db, string emailId, string phone, string agency, string srcFolder)
  {
    var an = $"-={srcFolder}-Add=-{ci.JobTitle}·{ci.Body}¦";
    var em = db.Emails.Local.FirstOrDefault(r => emailId.Equals(r.Id, StringComparison.OrdinalIgnoreCase));
    if (em == null)
    {
      var e0 = new Email
      {
        Id = emailId, // $"__UnKnwn__:{Guid.NewGuid()}",
        Fname = ci.FirstName,
        Lname = ci.LastName,
        //Company = agency,
        Phone = phone,
        Notes = an,
        AddedAt = Chronoer.Now
      };
      db.Emails.Local.Add(e0);
    }
    else
    {
      var note = $"-={srcFolder}-Upd=-{ci.JobTitle}·{ci.Email2Address} {ci.Email3Address}·{phone}·{ci.Body}¦";

      if (string.IsNullOrWhiteSpace(em.Phone) && !string.IsNullOrWhiteSpace(phone))
      {
        em.Phone = phone;
        em.ModifiedAt = Chronoer.Now;
      }

      if (string.IsNullOrWhiteSpace(em.Notes))
      {
        em.Notes = note;
        em.ModifiedAt = Chronoer.Now;
      }
      else if (!string.IsNullOrWhiteSpace(em.Notes))
      {
        if (!string.IsNullOrWhiteSpace(ci.JobTitle      /**/) && !em.Notes.Contains(ci.JobTitle      /**/)) { em.ModifiedAt = Chronoer.Now; em.Notes += $" + {ci.JobTitle}"; }

        if (!string.IsNullOrWhiteSpace(ci.Body          /**/) && !em.Notes.Contains(ci.Body          /**/)) { em.ModifiedAt = Chronoer.Now; em.Notes += $" + {ci.Body}"; }
      }
      else
        return;
    }
  }

  public static bool ValidEmailAddress(string emailaddress)
  {
    try
    {
      if (
        emailaddress.Length > 48 ||
        emailaddress.Contains('\\') ||
        emailaddress.Contains('/') ||
        emailaddress.Contains('=')
        ) return false;

      var m = new MailAddress(emailaddress);
      return true;
    }
    catch (FormatException)
    {
      return false;
    }
  }
  public static string FigureOutSenderEmail(OL.MailItem mailItem) => !string.IsNullOrEmpty(mailItem.Sender?.Address) && mailItem.Sender.Address.Contains('@') ? mailItem.Sender.Address :
                      !string.IsNullOrEmpty(mailItem.SenderEmailAddress) && mailItem.SenderEmailAddress.Contains('=') && mailItem.SenderEmailAddress.Contains('@') ? RemoveBadEmailParts(mailItem.SenderEmailAddress) :
                      !string.IsNullOrEmpty(mailItem.SenderEmailAddress) && mailItem.SenderEmailAddress.Contains('@') ? mailItem.SenderEmailAddress :
                      !string.IsNullOrEmpty(mailItem.SentOnBehalfOfName) && mailItem.SentOnBehalfOfName.Contains('@') ? mailItem.SentOnBehalfOfName :
                      mailItem.SenderEmailAddress;
  public static (string first, string last) FigureOutSenderFLName(OL.MailItem mailItem, string email)
  {
    var fln =
      !string.IsNullOrEmpty(mailItem.Sender?.Name) && mailItem.Sender.Name.Contains(' ') ? mailItem.Sender.Name :
      !string.IsNullOrEmpty(mailItem.SentOnBehalfOfName) && mailItem.SentOnBehalfOfName.Contains(' ') ? mailItem.SentOnBehalfOfName :
      !string.IsNullOrEmpty(mailItem.Sender?.Name) ? mailItem.Sender.Name :
      !string.IsNullOrEmpty(mailItem.SentOnBehalfOfName) ? mailItem.SentOnBehalfOfName :
      null;

    return FigureOutSenderFLName(fln, email);
  }
  public static (string first, string last) FigureOutSenderFLName(string? fln, string email)
  {
    if (fln is null or
      "Marketing- SharedMB" // randstad on behalf of case
      )
    {
      var hlp = new FirstLastNameParser(email);
      return (hlp.FirstName, hlp.LastName);
    }

    if (fln.Contains("via Indeed"))
      return ("Sirs", "");

    fln = fln.Trim(_delim);

    var idx = fln.IndexOf('@');
    if (idx > 0)
      fln = fln[..idx];

    var flnArray = fln.Split(_delim, StringSplitOptions.RemoveEmptyEntries);
    if (flnArray.Length == 1)
      return (flnArray[0], "");

    if (flnArray.Length >= 2)
      if (fln.Contains(','))
        return (flnArray[1], flnArray[0]);
      else
        return (flnArray[0], flnArray[1]);

    return ("Sirs", "");
  }
  public static (string first, string last) FigureOutFLNameFromBody(string body, string email)
  {
    body = body.ToLower();
    email = email.ToLower();
    var idx = body.IndexOf(email);
    var words = body[..idx].Split(_delim, StringSplitOptions.RemoveEmptyEntries);
    var len = words.Length;
    if (len > 5)
    {
      Write($">>> {words[len - 5]} {words[len - 4]}    {words[len - 3]} {words[len - 2]}    {words[len - 1]}    <= '{email}' ");

      if (new[] { "at", "-", "@", "<" }.Contains(words[len - 1]))
        if (new[] { "contact", "manager", "email", "to", "from", "com>", "ca>", "ca", "com" }.Contains(words[len - 4]))
          return (words[len - 3], words[len - 2]);           // Thank you for your email. Liam Tang is no longer with Experis-Veritaaq, please contact Michael Baraban at michaelb@experis-veritaaq.ca.
        else
      if (words[len - 1] == "(")
          if (words[len - 4] == "contact") return (words[len - 3], words[len - 2]);            // ... contact Gary Shearer (gary.shearer@appcentrica.com <mailto:gary.shearer@appcentrica.com> ). 
          else
        if (new[] { "<mailto", "addresses" }.Contains(words[len - 1]))
            WriteLine($" ignore this !!!");
          else
        if (Debugger.IsAttached)
            WriteLine($" ignore this ???");
    }

    var hlp = new FirstLastNameParser(email);
    return (hlp.FirstName, hlp.LastName);
  }
  public static (string first, string last) FigureOutSenderFLName(OL.ReportItem reportItem, string email)
  {
    var pc = reportItem.Body.ToLower().IndexOf("please contact");
    if (pc > 0)
    {
      var words = reportItem.Body[pc..].Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
      return (words[2], words[3]);
    }

    var hlp = new FirstLastNameParser(email);
    return (hlp.FirstName, hlp.LastName);
  }
  public static string ReportLine(string folder, string senderEmail, bool isNew) => $"{folder,-15}{(isNew ? "*" : " ")} {senderEmail,-48}{GetCompanyName(senderEmail),-48}\n";
  public static string ReportSectionTtl(string folder, int ttls, int news)      /**/ => $"{folder,-13}=>  total/new:        {ttls,3} / {news} \n\n";
  public static string ReportSectionTtl(string folder, int ttls, int bans, int news) => $"{folder,-13}=>  total/new/banned: {ttls,3} / {news} / {bans} \n\n";
  public static string RemoveBadEmailParts(string emailAddress)
  {
    emailAddress = emailAddress.Trim(_delim); //  new[] { ' ', '\'', '`', ';', ':' });

    foreach (var delim in new char[] { '=', '?' }) emailAddress = RemoveBadEmailParts(emailAddress, delim);

    return emailAddress;
  }
  static string RemoveBadEmailParts(string emailAddress, char delim)
  {
    var a = emailAddress.IndexOf(delim);
    if (a < 0) return emailAddress;

    var b = emailAddress.IndexOf('@');
    if (b > 0 && a < b)
    {
      var c = emailAddress[a..b];
      var d = emailAddress.Replace(c, "");
      return d;
    }
    else
    {
      var c = emailAddress[..a];
      return c;
    }
  }
  public static void MoveIt(OL.MAPIFolder targetFolder, OL.MailItem ol_item)
  {
#if DEBUG
#else
      ol_item.Move(targetFolder); ;
#endif
  }
  public static void MoveIt(OL.MAPIFolder targetFolder, OL.ReportItem ol_item)
  {
#if DEBUG
#else
      ol_item.Move(targetFolder); ;
#endif
  }
  public static void Test(OL.ReportItem item, string ww)
  {
    try
    {
      Write($"==> {ww}: ");

      var pa = item.PropertyAccessor;
      var prop = pa.GetProperty($"http://schemas.microsoft.com/mapi/proptag/{ww}");

      if (prop is byte[])
      {
        var snd = pa.BinaryToString(prop);
        Write($"=== snd: {snd}");

        var s = item.Session.GetAddressEntryFromID(snd);
        Write($"*** GetExchangeDistributionList(): {s.GetExchangeDistributionList()}"); //.PrimarySmtpAddress;
        Write($"*** Address: {s.Address}"); //.PrimarySmtpAddress;
      }
      //else if (sndr is byte[]) { }
      else
        Write($"==> {prop.GetType().Name}  {prop}");
    }
    catch (Exception ex) { Write($"!!! {ex.Message}"); }

    WriteLine($"^^^^^^^^^^^^^^^^^^^^^^^^^");
  }
}
