using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ConsoleApp1
{
  class Program__DI1
  {
    static void Main__(string[] args)
    {
      Console.WriteLine("Hello World!");

      var serviceProvider = ContainerConfiguration.Configure();
      serviceProvider.GetService<CRP>().Process();
    }
  }

  // ##3
  public static class ContainerConfiguration
  {
    public static ServiceProvider Configure()
    {
      return new ServiceCollection()
        .AddLogging(l => l.AddConsole())
        .Configure<LoggerFilterOptions>(c => c.MinLevel = LogLevel.Information)
        .AddSingleton<IType1, Type1>()
        .AddSingleton<IType2, Type2>()
        .AddSingleton<CRP>()
        .BuildServiceProvider();
    }
  }

  internal interface IType1 { }
  internal interface IType2 { }

  internal class Type1 : IType1 { }
  internal class Type2 : IType2 { }
  internal class CRP
  {
    readonly IType1 _type1;
    readonly ILogger<CRP> _logger;

    public CRP(IType1 type1, ILogger<CRP> logger) { _type1 = type1; _logger = logger; }
    internal void Process() => _logger.LogInformation("Helloe");
  }
}/*
DI1.cs
DI for .Net Core Console running in Docker
https://www.youtube.com/watch?v=2TgWRfOnOc0
*/
