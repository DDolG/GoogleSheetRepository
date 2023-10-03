namespace GppgleSheetRepository.Sample.Models;

public class Category : IEquatable<Category>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? ParentCategoryId { get; set; }

    public bool Equals(Category? other)
    {
        return Id == other.Id && Name == other.Name && ParentCategoryId == other.ParentCategoryId;
    }
}
