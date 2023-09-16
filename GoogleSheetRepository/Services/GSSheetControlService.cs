using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleSheetRepository.Interfaces;

namespace GoogleSheetRepository.Services
{
    public class IGSSheetControlService : IGSSheetControl
    {
        private readonly SheetsService _sheetsService;
        private readonly IGSService _googleSheetService;

        public IGSSheetControlService(IGSService googleSheetService)
        {
            _sheetsService = googleSheetService.GetService();
            _googleSheetService = googleSheetService;
        }

        public void Create(string name)
        {
            var settings = _googleSheetService.GetSettings();
            AddSheet(settings.SheetId, name);
        }

        public bool HavePage(string name)
        {
            var sheet = _sheetsService.Spreadsheets.Get(name);
            return sheet != null;
        }

        private static void AddSheet(string spreadsheetId, string newSheetTitle)
        {
            Request request = new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = newSheetTitle
                    }
                }
            };
        }
    }
}