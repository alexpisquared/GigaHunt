using AAV.Sys.Helpers;
using AAV.WPF.Ext;
using AvailStatusEmailer.View;
using Db.QStats.DbModel;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace AvailStatusEmailer
{
  public partial class MainSwitchboard : AAV.WPF.Base.WindowBase
  {
    public MainSwitchboard()
    {
      InitializeComponent();
      themeSelector1.ApplyTheme = ApplyTheme;

      tbver.Text = $"Db: {Db.QStats.DbModel.A0DbContext.DbNameOnly}        Ver: {AAV.Sys.Helpers.VerHelper.CurVerStr(".net 4.8")}";


      Loaded += async (s, e) =>
      {
        _ = Task.Run(() =>
        {
          try
          {
            var sw = Stopwatch.StartNew();
            A0DbContext.Create().lkuLeadStatus.Load();
            Trace.WriteLine($"../> preloaded in {sw.Elapsed.TotalSeconds,6:N1}s   (Net4: Cold Start 11s, next 5s)");
            Bpr.BeepBgn3();
          }
          catch (Exception ex) { ex.Pop(); }
        }); // preload to ini the EF for faster loads in views.
        
        await Task.Yield();
        themeSelector1.SetCurTheme(Thm);
        Bpr.BeepBgn3();
      };

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

    void onDb2Ou(object s, RoutedEventArgs e) => tbkTitle.Text = Title = tbRep.Text = new OutlookHelper().SyncDbToOutlook(A0DbContext.Create());
    async void onUndel(object s, RoutedEventArgs e) => tbkTitle.Text = Title = tbRep.Text = await new OutlookHelper().OutlookUndeleteContactsAsync(A0DbContext.Create());
    void onDbIni(object s, RoutedEventArgs e) => DBInitializer.SetDbInitializer();
  }
}