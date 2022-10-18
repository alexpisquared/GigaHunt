namespace GigaHunt
{
  abstract class C1
  {
    public abstract void PuM();
    protected abstract void PrM();
  }

  class C2 : C1
  {
    public override void PuM() { }
    protected override void PrM() { }
  }

  class Test
  {
    readonly C2 c2 = new C2();
  }
}
