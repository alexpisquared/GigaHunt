namespace GigaHunt.View;
public partial class MainSwitchboard : WpfUserControlLib.Base.WindowBase
{
  public MainSwitchboard() { InitializeComponent(); themeSelector1.ThemeApplier = ApplyTheme; tbver.Text = DevOps.IsDbg ? @"DBG" : "rls"; }
  async void OnLoaded(object s, RoutedEventArgs e)
  {
    themeSelector1.SetCurThemeToMenu(Thm);
    BPR.Start();
    await Task.Yield(); // allows to draw a window!!!

    try
    {
      var sw = Stopwatch.StartNew();
      await QStatsRlsContext.Create().LkuLeadStatuses.LoadAsync(); // preload to ini the EF for faster loads in views.
      WriteLine($"../> preloaded in {sw.Elapsed.TotalSeconds,6:N1}s   (Net 4/6:  Cold Start 11/5s,  Next 5.4/3.8s)");
      BPR.Finish();
    }
    catch (Exception ex) { ex.Pop(); }
  }
  //async void OnClose2(object sender, RoutedEventArgs e) { }//{ BPR___.AppFinish(); await Task.Delay(333); BPR___.Finish(); Close(); await Task.Delay(333); await Task.Delay(11000); Application.Current.Shutdown(); await Task.Delay(333); }
  void onDb2Ou(object s, RoutedEventArgs e) => tbver.Text = Title = new OutlookHelper6().SyncDbToOutlook(QStatsRlsContext.Create());
  async void onUndel(object s, RoutedEventArgs e) => tbver.Text = Title = await new OutlookHelper6().OutlookUndeleteContactsAsync(QStatsRlsContext.Create());
  void onDbIni(object s, RoutedEventArgs e) { } //  => DBInitializer.SetDbInitializer();

}