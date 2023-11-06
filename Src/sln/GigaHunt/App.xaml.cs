using System.Reflection;

namespace GigaHunt;
public partial class App : Application
{
  static MainSwitchboard? _msb; public static MainSwitchboard Msb => _msb ??= new MainSwitchboard();

  protected override void OnStartup(StartupEventArgs sea)
  {
    base.OnStartup(sea);
    Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;
    EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, re) => { (s as TextBox)?.SelectAll(); })); //tu: TextBox
#if !EmergencyTroubleshooting
        Listeners.Add(new TextWriterTraceListener(@$"C:\temp\Logs\{Assembly.GetExecutingAssembly().GetName().Name}.log"));
        AutoFlush = true;
        WriteLine("");
#endif

#if test
      new Office365_POC().Nogo_Jun2020();
      //Msb.Show();
      Shutdown();
      return;
#else
    switch (sea.Args.Length > 0 ? sea.Args[0] : /*Settings.Default.LastWin ??*/ "Menu0")
    {
      default: break; //Something is not right with this one - 2020-11
      case "Menu0": try { Msb.ShowDialog();                /**/ } catch { Write("Ignoring for now ...\n"); } goto default;
      case "Leads": try { new LeadManagerWindow().ShowDialog(); } catch { Write("Ignoring for now ...\n"); } goto case "Menu0";
      case "Agnts": try { new AgentAdminnWindow().ShowDialog(); } catch { Write("Ignoring for now ...\n"); } goto case "Menu0";
      case "Broad": try { new EmailersendWindow().ShowDialog(); } catch { Write("Ignoring for now ...\n"); } goto case "Menu0";
      case "OutDb": try { new OutlookToDbWindow().ShowDialog(); } catch { Write("Ignoring for now ...\n"); } goto case "Menu0";
    }
#endif
#if FIXED //Something is not right with this one - 2020-11
      var otd = new OutlookToDbWindow().onDoReglr_();
#endif
  }
  protected override void OnExit(ExitEventArgs e) // if async - does not get to the end!!!
  {
    WriteLine($",,, BPR___  000");
    View.BPR.AppFinish(); System.Threading.Thread.Sleep(1200); // if async - does not get to the end!!!  ... still, plays only when debugger.isAttached!!!
    WriteLine($",,, BPR___  111");
    //BPR___.AppFinishAsync();
    WriteLine($",,, BPR___  222");
    //await BPR___.AppFinishAsync();
    WriteLine($",,, BPR___  333");
    //await Task.Delay(1333);
    base.OnExit(e);
    WriteLine($",,, BPR___  444");
    Settings.Default.Save();
    WriteLine($",,, BPR___  555 - The End."); // if async - does not get to the end!!!
  }
}
