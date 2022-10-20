using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace Emailer3
{
  public partial class App : Application
  {
    readonly IHost _host;

    public App()
    {
      _host = new HostBuilder()
        .ConfigureLogging((context, logging) => // Logging in .NET Core 3.0 and Beyond - Configuration, Setup, and More. https://www.youtube.com/watch?v=oXNslgIXIbQ 
        {
          logging.ClearProviders();
          logging.AddConfiguration(context.Configuration.GetSection("Logging"));
          logging.AddConsole();
          logging.AddDebug();
          // EventSource, EventLog, TraceSource, AzureAppSerivesFile, AzureAppServixesBlob, ApplicationInsights
        })
        .ConfigureAppConfiguration((context, configurationBuilder) =>
        {
          configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
          configurationBuilder.AddJsonFile("appsettings.json", optional: true);
          
        })
        .ConfigureServices((context, services) =>
        {
          services.Configure<Settings>(context.Configuration);

          services.AddSingleton<ITextService, TextService>();
          services.AddSingleton<MainWindow>();
        })
        .Build();
    }
    async void Application_Startup(object sender, StartupEventArgs e)
    {
      await _host.StartAsync();

      var mainWindow = _host.Services.GetService<MainWindow>();
      mainWindow?.Show();
    }

    async void Application_Exit(object sender, ExitEventArgs e)
    {
      using (_host)
      {
        await _host.StopAsync(TimeSpan.FromSeconds(5));
      }
    }

    protected override void OnStartup(StartupEventArgs e) => base.OnStartup(e); // huh
  }
}/* 

WPF and .NET Generic Host with .NET Core 3.0
https://laurentkempe.com/2019/09/03/WPF-and-dotnet-Generic-Host-with-dotnet-Core-3-0/

//todo: Logging in .NET Core 3.0 and Beyond - Configuration, Setup, and More 
https://www.youtube.com/watch?v=oXNslgIXIbQ

*/
