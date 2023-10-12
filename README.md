# google-sheet-repository

# Badge Statuses
[![NuGet Downloads](https://img.shields.io/nuget/dt/GoogleSheetRepository.svg)](https://www.nuget.org/packages/GoogleSheetRepository/)

# GoogleSheetRepository

Allows execute CRUD operation in google sheet without having to have knowledge on the Google Sheets API methods and protocols.

The following Google Sheets API operations are suported:
- Pagination reading records
- Reading all records
- Appending new records
- Deleting records
- Updating records

The library is written in such a way as to make working with Google spreadsheets as easy as possible. You don't need to know the line number where the object you're interested in is located in order to delete it or update it. The library will do this for you.

# Installation

Add nuget package in your project by enter .net CLI
```
dotnet add package GoogleSheetRepository
```
You need to setup a Google API Service Account before you can use this library.

1. Create a service account. Steps to do that are documented below,

https://cloud.google.com/docs/authentication/production#create_service_account

2. After you download the JSON key, you need to decide how you want to store it and load it into the application.
In appsettings.json you need write path to file, with name of file like this:
```
"FCredencialFile": "app_client_secret.json"
```
in project tree:

![google credintial file in project tree](https://github.com/DDolG/google-sheet-repository/blob/main/Images/CredantsFile.jpg)

4. Create google sheet document or use exist. Get from browser sheetId:
![where can I find the SheetId in the browser](https://github.com/DDolG/google-sheet-repository/blob/main/Images/BrowserSheetId.jpg)
Enter SheetId to the appsettings.json
```
"SheetId": "XXX-XXX-XXX-YOUR-CODE-SET-IN-SECRET-XXX-XXX-XXX",
```
5. Use the service account identity that is created and add that email address to grant it permissions to the Google Sheets Spreadsheet you want to interact with.
6. Make sure you have completed all settings in:
```
"GoogleSheetSettings": {
  "SheetName": "roles",
  "SheetId": "XXX-XXX-XXX-YOUR-CODE-SET-IN-SECRET-XXX-XXX-XXX",
  "FCredencialFile": "app_client_secret.json"
}
```
7. Create a class.
```
public class Product : IEquatable<Product>
{
    public int Id { get; set; }
    public string Name { get; set; }    
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    public bool Equals(Product? other)
    {
        return Id == other.Id
              && Name == other.Name
              && Price == other.Price
              && CategoryId == other.CategoryId;
    }
}
```
The class must implement the interface IEquatable.

8. Create depedency injection:
```
builder.Services.AddSingleton<IRepository<Product>, Repository<Product>>();
```
9. Start project.
    In google sheet is created table "Product".
![New table Product is created](https://github.com/DDolG/google-sheet-repository/blob/main/Images/TableGoogleCreateInstance.jpg)   

# How to read paginated list records
```
private readonly IRepository<Product> _repository;
```
var products = _repository.Get(1,1);

# How to read all records
```
private readonly IRepository<Product> _repository;
```
var products = _repository.Get();
```
```

# How add record
```
private readonly IRepository<Product> _repository;
```
```
var products = _repository.Add(new Product());
```

# How delete record
```
private readonly IRepository<Product> _repository;
```
```
var items = _repository.Get();
var item = items.FirstOrDefault(x=>x.Id == id);
if (item == null)
{
    return NotFound();
}
_repository.Delete(item);
```

# How update record
```
private readonly IRepository<Product> _repository;
```
```
var items = _repository.Get();
var existingItem = items.FirstOrDefault(x => x.Id == id);
if (existingItem == null)
{
    return NotFound();
}
_repository.Update(existingItem, updatedItem);
```

# Sample project
In repository sample project.
