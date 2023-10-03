namespace GppgleSheetRepository.Sample.Models;

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
