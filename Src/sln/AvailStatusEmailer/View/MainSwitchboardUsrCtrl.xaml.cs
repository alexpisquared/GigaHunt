using AAV.WPF.Extension;
using AgentFastAdmin;
using AvailStatusEmailer.Properties;
using OutlookToDbWpfApp;
using System.Windows;
using System.Windows.Controls;

namespace AvailStatusEmailer.View
{
  public partial class MainSwitchboardUsrCtrl : UserControl
  {
    public Window ContainerWindow => _owner ?? (_owner = this.FindParentWindow()); Window _owner;
    void popIt_modal(Window popWind, string lastWin) { ContainerWindow.Hide(); popWind.ShowDialog(); ContainerWindow.Show(); Settings.Default.LastWin = lastWin; }
    void popIt(Window popWind, string lastWin) { ContainerWindow.Hide(); popWind.Show(); ContainerWindow.Close(); Settings.Default.LastWin = lastWin; }

    public MainSwitchboardUsrCtrl() => InitializeComponent();

    public static readonly DependencyProperty VizLeadsProperty = DependencyProperty.Register("VizLeads", typeof(Visibility), typeof(MainSwitchboardUsrCtrl)); public Visibility VizLeads { get => (Visibility)GetValue(VizLeadsProperty); set => SetValue(VizLeadsProperty, value); }
    public static readonly DependencyProperty VizAgentProperty = DependencyProperty.Register("VizAgent", typeof(Visibility), typeof(MainSwitchboardUsrCtrl)); public Visibility VizAgent { get => (Visibility)GetValue(VizAgentProperty); set => SetValue(VizAgentProperty, value); }
    public static readonly DependencyProperty VizBroadProperty = DependencyProperty.Register("VizBroad", typeof(Visibility), typeof(MainSwitchboardUsrCtrl)); public Visibility VizBroad { get => (Visibility)GetValue(VizBroadProperty); set => SetValue(VizBroadProperty, value); }
    public static readonly DependencyProperty VizOu2DbProperty = DependencyProperty.Register("VizOu2Db", typeof(Visibility), typeof(MainSwitchboardUsrCtrl)); public Visibility VizOu2Db { get => (Visibility)GetValue(VizOu2DbProperty); set => SetValue(VizOu2DbProperty, value); }

    void onLeads(object s, RoutedEventArgs e) => popIt(new LeadManagerWindow(), "Leads");
    void onAgent(object s, RoutedEventArgs e) => popIt(new AgentAdminnWindow(), "Agnts");
    void onBroad(object s, RoutedEventArgs e) => popIt(new EmailersendWindow(), "Broad");
    void onOu2Db(object s, RoutedEventArgs e) => popIt(new OutlookToDbWindow(), "OutDb"); // maybe ... do not auto start to Out->Db: there could be edits up in the air.
  }
}
