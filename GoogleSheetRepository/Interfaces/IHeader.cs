using GoogleSheetRepository.Models;

namespace GoogleSheetRepository.Interfaces
{
    public interface IHeader
    {
        List<ColumnPropertyHeader> GetPropertyFromHeader();

        int? GetPropertyCountFromPage();

        void SetPropertyCount();

        void InitPropertyHeaders();
    }
}
