namespace GigaHunt.View;
internal class BPR
{
  static readonly Bpr _b = new();
  internal static void Start() => _b.Start();
  internal static void Finish() => _b.Finish();
  internal static void AppStart() => _b.AppStart();
  internal static void AppFinish() => _b.AppFinish();
  internal static void BeepClk() => _b.Click();
  internal static void BeepShort() => _b.Tick();
}