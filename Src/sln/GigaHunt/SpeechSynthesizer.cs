namespace GigaHunt;

internal class SpeechSynthesizer
{
  public int Rate { get; internal set; }
  public int Volume { get; internal set; }

  internal void Speak(string v) => throw new NotImplementedException();
  internal void SpeakAsync(string v) => throw new NotImplementedException();
  internal void SpeakAsyncCancelAll() => throw new NotImplementedException();
}