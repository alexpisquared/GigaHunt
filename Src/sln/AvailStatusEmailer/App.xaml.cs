﻿using AAV.Sys.Helpers;
using AAV.WPF.Helpers;
using AgentFastAdmin;
using AvailStatusEmailer.Properties;
using OutlookToDbWpfApp;
using System;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;

namespace AvailStatusEmailer
{
  public partial class App : Application
  {
    public static DateTime Now = DateTime.Now;
    static SpeechSynthesizer _synth; static SpeechSynthesizer Synth
    {
      get
      {
        if (_synth == null)
        {
          _synth = new SpeechSynthesizer { Rate = 7, Volume = 75 };
          _synth.SelectVoiceByHints(gender: VoiceGender.Female);
        }
        return _synth;
      }
    }
    internal static void SpeakAsync(string v) { App.Synth.SpeakAsyncCancelAll(); App.Synth.SpeakAsync(v); }
    internal static void SpeakSynch(string v) { App.Synth.SpeakAsyncCancelAll(); App.Synth.Speak(v); }

    static MainSwitchboard _msb = null; public static MainSwitchboard Msb => _msb ?? (_msb = new MainSwitchboard());


    protected override void OnStartup(StartupEventArgs sea)
    {
      base.OnStartup(sea);
      Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;
      EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, re) => { (s as TextBox).SelectAll(); })); //tu: TextBox
      Tracer.SetupTracingOptions("AvailStatusEmailer", new TraceSwitch("OnlyUsedWhenInConfig", "This is the trace for all               messages... but who cares?   See ScrSvr for a model.") { Level = TraceLevel.Verbose }); // Trace.WriteLine(string.Format("*{0:MMdd HH:mm} The Start. CommandLine: '{1}'", AvailStatusEmailer.App._now, Environment.CommandLine));

      ShutdownMode = ShutdownMode.OnLastWindowClose;

#if test
      new Office365_POC().Nogo_Jun2020();
      //Msb.Show();
      Shutdown();
      return;
#else
      switch (sea.Args.Length > 0 ? sea.Args[0] : "Menu0") // Settings.Default.LastWin
      {
        default: break; //Something is not right with this one - 2020-11
        case "Menu0": try { Msb.ShowDialog();                /**/ } catch { Debug.Write("Ignoring for now ...\n"); } goto default;
        case "Leads": try { new LeadManagerWindow().ShowDialog(); } catch { Debug.Write("Ignoring for now ...\n"); } goto case "Menu0";
        case "Agnts": try { new AgentAdminnWindow().ShowDialog(); } catch { Debug.Write("Ignoring for now ...\n"); } goto case "Menu0";
        case "Broad": try { new EmailersendWindow().ShowDialog(); } catch { Debug.Write("Ignoring for now ...\n"); } goto case "Menu0";
        case "OutDb": try { new OutlookToDbWindow().ShowDialog(); } catch { Debug.Write("Ignoring for now ...\n"); } goto case "Menu0";
      }
#endif
#if FIXED //Something is not right with this one - 2020-11
      var otd = new OutlookToDbWindow().onDoReglr_();
#endif
      //Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
      base.OnExit(e);
      Settings.Default.Save();
      AAV.Sys.Helpers.Bpr.BeepEnd3();
      App.Current.Shutdown();
#if VerboseTracing
			Trace.WriteLine(string.Format("*{0:MMdd HH:mm} The End. Took {1:hh\\:mm\\:ss}.", AvailStatusEmailer.App._now, SW.Elapsed));
    //SysLgr: _sl.LogSessionEnd();
#endif
    }
    protected override void OnSessionEnding(SessionEndingCancelEventArgs e) => base.OnSessionEnding(e); /*Bpr.BeepEnd3();*/
  }
}
