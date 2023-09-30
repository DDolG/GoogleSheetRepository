namespace GoogleSheetRepository.Interfaces
{
    public interface IGSSheetControl
    {
        bool HavePage(string name);

        void Create(string name);

        int GetSheetId(string spreadsheetId, string sheetName);
    }
}