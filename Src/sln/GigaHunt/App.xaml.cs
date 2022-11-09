namespace GigaHunt;
public partial class App : Application
{
static SpeechSynthesizer? _synth; static SpeechSynthesizer Synth { get { _synth ??= new SpeechSynthesizer { Rate = 7, Volume = 75 }; return _synth; } }
  internal static void Speak(string v) => Synth.Speak(v);

  static MainSwitchboard? _msb; public static MainSwitchboard Msb => _msb ??= new MainSwitchboard();

  protected override void OnStartup(StartupEventArgs sea)
  {
    base.OnStartup(sea);
    Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;
    EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, re) => { (s as TextBox)?.SelectAll(); })); //tu: TextBox
    //Tracer.SetupTracingOptions("GigaHunt", new TraceSwitch("OnlyUsedWhenInConfig", "This is the trace for all               messages... but who cares?   See ScrSvr for a model.") { Level = TraceLevel.Verbose }); // Trace.WriteLine(string.Format("*{0:MMdd HH:mm} The Start. CommandLine: '{1}'", GigaHunt.BPR___._now, Environment.CommandLine));

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
