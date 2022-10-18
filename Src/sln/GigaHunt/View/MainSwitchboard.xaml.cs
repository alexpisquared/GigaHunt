using AAV.Sys.Helpers;
using AvailStatusEmailer.View;
using Db.QStats.DbModel;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;

namespace AvailStatusEmailer
{
  public partial class MainSwitchboard : WpfUserControlLib.Base.WindowBase
  {
    public MainSwitchboard()
    {
      InitializeComponent(); 
      themeSelector1.ApplyTheme = ApplyTheme;

      tbver.Text = $"Db: {Db.QStats.DbModel.QStatsRlsContext.DbNameOnly}        Ver: ???";

      Task.Run(() => QStatsRlsContext.Create().lkuLeadStatus.Load()); // preload to ini the EF for faster loads in views.

      Loaded += async (s, e) => { await Task.Yield(); themeSelector1.SetCurTheme(Thm); Bpr.BeepBgn3(); };

#if DEBUG_
      if (checkNewEmail)
      {
        var hasNewAgens = new OutlookToDbWindow().ShowDialog();
        if (hasNewAgens == true)
          new AgentAdminnWindow().ShowDialog();
      }
#endif
    }
    void onClose(object s, RoutedEventArgs e) { Close(); Application.Current.Shutdown(); }

    void onDb2Ou(object s, RoutedEventArgs e) => tbkTitle.Text = Title = tbRep.Text = new OutlookHelper().SyncDbToOutlook(QStatsRlsContext.Create());
    async void onUndel(object s, RoutedEventArgs e) => tbkTitle.Text = Title = tbRep.Text = await new OutlookHelper().OutlookUndeleteContactsAsync(QStatsRlsContext.Create());
    void onDbIni(object s, RoutedEventArgs e) => DBInitializer.SetDbInitializer();
  }
}