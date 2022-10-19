namespace GigaHunt.View;
internal class BPR
{
  static readonly Bpr _b = new();
  internal static void Beep1of2() => _b.Start();
  internal static void Beep2of2() => _b.Finish();
  internal static void AppStart() => _b.AppStart();
  internal static void BeepDone() => _b.AppFinish();
  internal static void BeepClk() => _b.Click();
  internal static void BeepShort() => _b.Tick();
}