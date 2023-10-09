using GoogleSheetRepository;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Services;
using GppgleSheetRepository.Sample.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
                .AddNewtonsoftJson();

builder.Services.AddSingleton<ISettings, Settings>();
builder.Services.AddSingleton<IRepository<Product>, Repository<Product>>();
builder.Services.AddSingleton<IRepository<Category>, Repository<Category>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapGet("/categories", (IRepository<Category> repository) =>
{
    return Results.Ok(repository.Get());
})
.WithName("categories")
.WithOpenApi();

app.Run();
