using Microsoft.Extensions.Logging;
using System.Windows;

namespace Emailer3
{
  public partial class MainWindow : Window
  {
    readonly ILogger<MainWindow> _logger;
    readonly ILogger _logger2;

    public MainWindow(ITextService textService, ILogger<MainWindow> logger)
    {
      InitializeComponent();

      Label1.Content = textService.GetText();
      _logger = logger;
    }
    //public MainWindow(ILoggerFactory loggerFactory)
    //{
    //  InitializeComponent();

    //  _logger2 = loggerFactory.CreateLogger("Custom_Category");
    //}

    void onMain(object sender, RoutedEventArgs e) => Close();
    void onExit(object sender, RoutedEventArgs e) => Close();

    void onLoad(object sender, RoutedEventArgs e)
    {
      _logger.LogTrace      /**/ ("== loaded - LogTrace      ");
      _logger.LogDebug      /**/ (1,"== loaded - LogDebug      ");
      _logger.LogInformation/**/ (2,"== loaded - LogInformation");
      _logger.LogWarning    /**/ (3,"== loaded - LogWarning    ");
      _logger.LogError      /**/ (4,"== loaded - LogError      ");
      _logger.LogCritical   /**/ (5,"== loaded - LogCritical   ");
    }
  }
}
