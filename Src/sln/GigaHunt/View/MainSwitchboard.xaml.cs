using GigaHunt.AsLink;

namespace GigaHunt.View
{
  public partial class MainSwitchboard : WpfUserControlLib.Base.WindowBase
  {
    public MainSwitchboard()
    {
      InitializeComponent();
      themeSelector1.ThemeApplier = ApplyTheme;

      tbver.Text = $"Db: {DB.QStats.Std.Models.QStatsRlsContext.DbNameOnly}        Ver: ???";


#if DEBUG_
      if (checkNewEmail)
      {
        var hasNewAgens = new OutlookToDbWindow().ShowDialog();
        if (hasNewAgens == true)
          new AgentAdminnWindow().ShowDialog();
      }
#endif
    }

    void OnLoaded(object sender, RoutedEventArgs e)
    {
      try
      {
        var d = QStatsRlsContext.Create();
        d.LkuLeadStatuses.Load();
      }
      catch (Exception ex) { ex.Pop(); }

      Task.Run(() =>
      {
        try { QStatsRlsContext.Create().LkuLeadStatuses.Load(); } catch (Exception ex) { ex.Pop(); }
      }); // preload to ini the EF for faster loads in views.


      themeSelector1.SetCurThemeToMenu(Thm);
      Bpr.BeepBgn3();
    }
    void onClose(object s, RoutedEventArgs e) { Close(); Application.Current.Shutdown(); }

    void onDb2Ou(object s, RoutedEventArgs e) => tbkTitle.Text = Title = tbRep.Text = new OutlookHelper().SyncDbToOutlook(QStatsRlsContext.Create());
    async void onUndel(object s, RoutedEventArgs e) => tbkTitle.Text = Title = tbRep.Text = await new OutlookHelper().OutlookUndeleteContactsAsync(QStatsRlsContext.Create());
    void onDbIni(object s, RoutedEventArgs e) { } //  => DBInitializer.SetDbInitializer();
  }
}