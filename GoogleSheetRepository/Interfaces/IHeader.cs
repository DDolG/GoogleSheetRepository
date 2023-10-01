using GoogleSheetRepository.Models;

namespace GoogleSheetRepository.Interfaces
{
    public interface IHeader
    {
        Task<List<ColumnPropertyHeader>> GetPropertyFromHeaderAsync();

        Task<int?> GetPropertyCountFromPageAsync();

        Task SetPropertyCountAsync();

        Task InitPropertyHeadersAsync();
    }
}
