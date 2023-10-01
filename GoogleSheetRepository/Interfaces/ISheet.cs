namespace GoogleSheetRepository.Interfaces
{
    public interface ISheet
    {
        bool HavePage();

        void Create();

        int GetSheetId();

        int? GetLastRowNumber();
    }
}