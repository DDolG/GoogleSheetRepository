using Google.Apis.Sheets.v4;
using GoogleSheetRepository.Models;

namespace GoogleSheetRepository.Interfaces
{
    public interface ISettings
    {
        SheetsService GetService();

        GoogleSheetSettings GetSettings();
    }
}