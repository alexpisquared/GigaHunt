using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Emailer3_DI_WebApp
{
  public class Program
  {
    public static void Main(string[] args)
    {

      // https://www.youtube.com/watch?v=oXNslgIXIbQ&feature=youtu.be&t=3071
      //         CreateHostBuilder(args).Build().Run();  ==>
      var host = CreateHostBuilder(args).Build();
      var logger = host.Services.GetRequiredService<ILogger<Program>>();
      logger.LogInformation(" ** Logging before everything else: The app has started!!!");
      host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
          .ConfigureLogging((context, logging) => // Logging in .NET Core 3.0 and Beyond - Configuration, Setup, and More. https://www.youtube.com/watch?v=oXNslgIXIbQ     
          {
            logging.ClearProviders();
            logging.AddConfiguration(context.Configuration.GetSection("Logging"));
            logging.AddConsole();
            logging.AddDebug();
            // EventSource, EventLog, TraceSource, AzureAppSerivesFile, AzureAppServixesBlob, ApplicationInsights
          })
          .ConfigureWebHostDefaults(webBuilder =>
          {
            webBuilder.UseStartup<Startup>();
          });
  }
}