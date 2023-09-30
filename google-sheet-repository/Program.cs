using GoogleSheetRepository;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Models;
using GoogleSheetRepository.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Repository;

public class Moq
{
    public int Id { get; set; }

    public string Name { get; set; }
}

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
        serviceCollection.AddScoped<IGSSheetControl, IGSSheetControlService>();
        serviceCollection.AddScoped<IGSRepository<Moq>, GSRepositoryService<Moq>>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var myService = scope.ServiceProvider.GetRequiredService<IGSService>();
            var test = myService.GetSettings();
            var mworker = scope.ServiceProvider.GetRequiredService<IGSRepository<Moq>>();
            var testMoq = new Moq
            {
                Id = 2,
                Name = "Test1"
            };
            
            var test2 = mworker.GetAsync();


        }


        Console.ReadKey();
    }
    
    
}