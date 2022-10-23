namespace GigaHunt.View;
public partial class MainSwitchboard : WpfUserControlLib.Base.WindowBase
{
  public MainSwitchboard()  {    InitializeComponent();    themeSelector1.ThemeApplier = ApplyTheme; tbver.Text = DevOps.IsDbg ? @"DBG" : "rls"; }
  void OnLoaded(object s, RoutedEventArgs e)
  {
    _ = Task.Run(() =>
    {
      try
      {
        var sw = Stopwatch.StartNew();
        QStatsRlsContext.Create().LkuLeadStatuses.Load();
        Trace.WriteLine($"../> preloaded in {sw.Elapsed.TotalSeconds,6:N1}s   (Net 4/6:  Cold Start 11/5s,  Next 5.4/3.8s)");
        BPR.AppStart();
      }
      catch (Exception ex) { ex.Pop(); }
    }); // preload to ini the EF for faster loads in views.

    themeSelector1.SetCurThemeToMenu(Thm);
    BPR.AppStart();
  }
  void onDb2Ou(object s, RoutedEventArgs e) => tbver.Text = Title = new OutlookHelper6().SyncDbToOutlook(QStatsRlsContext.Create());
  async void onUndel(object s, RoutedEventArgs e) => tbver.Text = Title = await new OutlookHelper6().OutlookUndeleteContactsAsync(QStatsRlsContext.Create());
  void onDbIni(object s, RoutedEventArgs e) { } //  => DBInitializer.SetDbInitializer();
  void OnClose(object s, RoutedEventArgs e) { Close(); Application.Current.Shutdown(); }
}