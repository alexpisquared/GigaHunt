namespace GigaHunt;

internal class SpeechSynthesizer
{
  public int Rate { get; internal set; }
  public int Volume { get; internal set; }

  internal void Speak(string msg) => UseSayExe(msg);
  internal void SpeakAsync(string msg) => UseSayExe(msg);
  internal void SpeakAsyncCancelAll() => Trace.WriteLine("todo");

  static void UseSayExe(string msg) => new Process { StartInfo = new ProcessStartInfo("say.exe", $"\"{msg}\"") { RedirectStandardError = true, UseShellExecute = false } }.Start();   
}