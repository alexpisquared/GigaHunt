namespace GigaHunt;

internal class SpeechSynthesizer
{
  public int Rate { get; internal set; }
  public int Volume { get; internal set; }

  internal void Speak(string v) => Trace.WriteLine("beep");
  internal void SpeakAsync(string v) => Trace.WriteLine("beep");
  internal void SpeakAsyncCancelAll() => Trace.WriteLine("beep");
}