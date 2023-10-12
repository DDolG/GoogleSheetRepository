namespace GoogleSheetRepository.Models
{
    /// <summary>
    /// Table header object
    /// </summary>
    public class ColumnPropertyHeader : IEquatable<ColumnPropertyHeader>
    {
        public string Name { get; set; }

        public string PropertyType { get; set; }

        public bool Equals(ColumnPropertyHeader? other)
        {
            return this.Name == other.Name && this.PropertyType == other.PropertyType;
        }

        public override string ToString()
        {
            return $"{Name}[{PropertyType}] ";
        }
    }
}
