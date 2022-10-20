using GigaHunt.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GigaHunt
{
  public class AppSettings
  {
    public WindowPlace[] WP { get => window; set => window = value; }
    WindowPlace[] window = new WindowPlace[5];
    static readonly List<WindowPlace> lst = new List<WindowPlace>();

    private static WindowPlace getWP(string key)
    {
      var wp = lst.FirstOrDefault(r => r.WinName == key);
      if (wp == null)
        lst.Add(new WindowPlace { WinName = key, windowLeft = 200, windowTop = 200, windowWidth = 960, windowHeight = 540 });

      return wp;
    }

    [Obsolete("Use the other one from ...", true)]
    public static void StoreWinPos(Window wdw, string key)
    {
      var wp = getWP(key);
      wp.windowTop = wdw.Top;
      wp.windowLeft = wdw.Left;
      wp.windowWidth = wdw.Width;
      wp.windowHeight = wdw.Height;
      Settings.Default.AppSettings = Serializer.SaveToString(wp);
      Settings.Default.Save();
    }
    [Obsolete("Use the other one from ...", true)]
    public static void StoreWinPos(Window wdw, int w)
    {
      var stgs = (string.IsNullOrEmpty(Settings.Default.AppSettings) || null == Serializer.LoadFromString<AppSettings>(Settings.Default.AppSettings) as AppSettings) ? new AppSettings() : Serializer.LoadFromString<AppSettings>(Settings.Default.AppSettings) as AppSettings;
      stgs.WP[w] = new WindowPlace
      {
        windowTop = wdw.Top,
        windowLeft = wdw.Left,
        windowWidth = wdw.Width,
        windowHeight = wdw.Height
      };
      Settings.Default.AppSettings = Serializer.SaveToString(stgs);
      Settings.Default.Save();
    }
    [Obsolete("Use the other one from ...", true)]
    public static void PositionWindow(Window wdw, string key)
    {
      var wp = getWP(key);

      wdw.Top = wp.windowTop;
      wdw.Left = wp.windowLeft;
      wdw.Width = wp.windowWidth;
      wdw.Height = wp.windowHeight;
    }

    [Obsolete("Use the other one from ...", true)]
    public static void PositionWindow(Window wdw, int w)
    {
      if (!string.IsNullOrEmpty(Settings.Default.AppSettings))
      {
        if (Serializer.LoadFromString<AppSettings>(Settings.Default.AppSettings) is AppSettings stgs && stgs.WP[w] != null)
        {
          wdw.Top = stgs.WP[w].windowTop;
          wdw.Left = stgs.WP[w].windowLeft;
          wdw.Width = stgs.WP[w].windowWidth;
          wdw.Height = stgs.WP[w].windowHeight;
        }
      }
    }
  }

  public class WindowPlace
  {
    public string? WinName { get; set; }
    public double windowLeft = 200;
    public double windowTop = 200;
    public double windowWidth = 960;
    public double windowHeight = 540;
  }
}
