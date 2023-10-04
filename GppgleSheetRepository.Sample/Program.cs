using GoogleSheetRepository;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Services;
using GppgleSheetRepository.Sample.Models;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
builder.Services.AddSingleton<IConfiguration>(configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISettings, Settings>();
builder.Services.AddSingleton<IRepository<Product>, Repository<Product>>();
builder.Services.AddSingleton<IRepository<Category>, Repository<Category>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/products", (IRepository<Product> repository) =>
{
    return repository.Get();
})
.WithName("Google sheet repository")
.WithOpenApi();

app.Run();
