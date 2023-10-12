using Google.Apis.Sheets.v4;
using GoogleSheetRepository.Models;

namespace GoogleSheetRepository.Interfaces
{
    public interface ISettings
    {
        /// <summary>
        /// Get a service to interact with Google Sheet
        /// </summary>
        /// <returns></returns>
        SheetsService GetService();

        /// <summary>
        /// Get setting for connect to google sheet
        /// </summary>
        /// <returns></returns>
        GoogleSheetSettings GetSettings();
    }
}