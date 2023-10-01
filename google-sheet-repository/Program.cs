﻿using GoogleSheetRepository;
using GoogleSheetRepository.Helpers;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Models;
using GoogleSheetRepository.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Repository;

public class Moq : IEquatable<Moq>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool Equals(Moq? other)
    {
        return Id == other.Id && Name == other.Name;
    }
}

public class Moq2 : IEquatable<Moq2>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public bool Equals(Moq2? other)
    {
        return Id == other.Id && Name == other.Name;
    }
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
        serviceCollection.AddScoped<ISettings, Settings>();
        serviceCollection.AddScoped<IRepository<Moq>, Repository<Moq>>();
        serviceCollection.AddScoped<IRepository<Moq2>, Repository<Moq2>>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var myService = scope.ServiceProvider.GetRequiredService<ISettings>();
            var test = myService.GetSettings();
            var mworker = scope.ServiceProvider.GetRequiredService<IRepository<Moq>>();
            var testMoq = new Moq
            {
                Id = 2,
                Name = "Test1"
            };
            
            var test2 = mworker.Get();            
            var updateOld = test2.First();
            var updateTestObject = new Moq();
            updateTestObject.Id = updateOld.Id + 1;
            updateTestObject.Name = updateOld.Name + updateTestObject.Id;

            var test3 = mworker.Update(updateOld, updateTestObject);
            Console.WriteLine($"Result update value: {test3}");

            //var test4 = Task.Run(async () => await mworker.DeleteAsync(test2.Result[2]));
            //Console.WriteLine($"Result delete row: {test4.Result}");

            var test5 =  mworker.Get(1, 1);
            Console.WriteLine($"Result row read: {JsonSerializer.Serialize(test5)}");

            var mworker2 = scope.ServiceProvider.GetRequiredService<IRepository<Moq2>>();
            var testMoq2 = new Moq2
            {
                Id = 2,
                Name = "Test Moq2",
                Price = 2.8M,
            };
            mworker2.Add(testMoq2);
        }


        Console.ReadKey();
    }
    


    
}