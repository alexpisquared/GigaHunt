using System;
using System.Media;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace EF
{
  public partial class IsLoadingAnimo : UserControl
  {
    readonly SoundPlayer _spWater = new SoundPlayer(GigaHunt.Properties.Resources.WaterDroplet);
    readonly DispatcherTimer t = new DispatcherTimer();

    public IsLoadingAnimo()
    {
      InitializeComponent();
      _spWater.Load();
      t.Tick += (s, e) => _spWater.Play();
      t.Interval = TimeSpan.FromSeconds(4);
    }
    internal void Start()
    {
      (FindResource("sbLoading") as Storyboard).Begin();
      t.Start();
      //            Task.Factory.StartNew(() => Thread.Sleep(4000)).ContinueWith(_ => _spWater.Play(), TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith(_ => Thread.Sleep(4000)).ContinueWith(_ => _spWater.Play(), TaskScheduler.FromCurrentSynchronizationContext());
    }
    internal void Stop() { (FindResource("sbHide") as Storyboard).Begin(); t.Stop(); }
  }
}




