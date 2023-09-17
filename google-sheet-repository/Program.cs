using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Models;
using GoogleSheetRepository.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Repository;

public static class Program
{
    public static void Main()
    {
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        serviceCollection.AddSingleton<IConfiguration>(configuration);

        var settings = new GoogleSheetSettings(); 
        serviceCollection.AddScoped<IGSService, GSService>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var myService = scope.ServiceProvider.GetRequiredService<IGSService>();
            var test = myService.GetSettings();
        }


        Console.ReadKey();
    }
    
    
}