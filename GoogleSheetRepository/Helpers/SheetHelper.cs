using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Models;

namespace GoogleSheetRepository.Helpers
{
    public class SheetHelper : ISheet
    {
        private readonly SheetsService _sheetsService;
        private readonly GoogleSheetSettings _settings;
        private readonly string _sheetName;

        public SheetHelper(ISettings googleSheetService,
                                        string sheetName)
        {
            _sheetsService = googleSheetService.GetService();
            _settings = googleSheetService.GetSettings();
            _sheetName = sheetName;
        }

        public void Create()
        {
            AddSheet();
        }

        public bool HavePage()
        {
            return CanReadSheet();
        }

        public int GetSheetId()
        {
            var spreadsheet = _sheetsService.Spreadsheets.Get(_settings.SheetId).Execute();
            var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == _sheetName);

            if (sheet != null)
            {
                return sheet.Properties.SheetId ?? 0;
            }
            throw new InvalidOperationException($"Sheet '{_sheetName}' not found in the spreadsheet.");
        }

        public int? GetLastRowNumber()
        {
            var range = $"{_sheetName}!{Constants.ColumnForSearchLastRecord}";
            var request = _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = new ValueRange();
            try
            {
                response = request.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get count property {ex.Message}");
            }
            var result = response?.Values?.Count;
            if (result == null) return null;
            return result;
        }

        private void AddSheet()
        {
            var request = new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = _sheetName
                    }
                }
            };
            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request> { request }
            };
            try
            {
                _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, _settings.SheetId).Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create page name {_sheetName} error: {ex.Message}");
            }
        }

        private bool CanReadSheet()
        {
            var range = $"{_sheetName}!A1";
            var request = _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            try
            {
                var response = request.Execute();
                return response != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}