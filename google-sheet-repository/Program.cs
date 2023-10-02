using GoogleSheetRepository;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Models;
using GoogleSheetRepository.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text.Json;

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

public class Product : IEquatable<Product>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    public int CategoryId { get; set; }

    public bool Equals(Product? other)
    {
        return Id == other.Id && Name == other.Name 
            && Price == other.Price && CategoryId == other.CategoryId;
    }
}

public class Category : IEquatable<Category>
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int? ParentCategoryId {  get; set; }

    public bool Equals(Category? other)
    {
        return Id == other.Id && Name == other.Name && ParentCategoryId == other.ParentCategoryId;
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
        serviceCollection.AddScoped<IRepository<Product>, Repository<Product>>();
        serviceCollection.AddScoped<IRepository<Category>, Repository<Category>>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var myService = scope.ServiceProvider.GetRequiredService<ISettings>();
            var test = myService.GetSettings();
            var productWorker = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();
            var categoryWorker = scope.ServiceProvider.GetRequiredService<IRepository<Category>>();
            var apple = new Product
            {
                Id = 1,
                Name = "Apple",
                CategoryId = 1,
                Price = 3.89M
            };
            productWorker.Add(apple);

            var fruit = new Category
            {
                Id = 1,
                Name = "Fruit",
                ParentCategoryId = 2
            };
            categoryWorker.Add(fruit);

            var food = new Category
            {
                Id = 2,
                Name = "Food"
            };
            categoryWorker.Add(food);
            /*
            var test2 = mworker.Get();            
            var updateOld = test2.First();
            var updateTestObject = new Moq();
            updateTestObject.Id = updateOld.Id + 1;
            updateTestObject.Name = updateOld.Name + updateTestObject.Id;

            var test3 = mworker.Update(updateOld, updateTestObject);
            Console.WriteLine($"Result update value: {test3}");

            var test5 =  mworker.Get(1, 1);
            Console.WriteLine($"Result row read: {JsonSerializer.Serialize(test5)}");

            var mworker2 = scope.ServiceProvider.GetRequiredService<IRepository<Moq2>>();
            var testMoq2 = new Moq2
            {
                Id = 2,
                Name = "Test Moq2",
                Price = 2.8M,
            };
            mworker2.Add(testMoq2);*/
        }


        Console.ReadKey();
    }
    


    
}