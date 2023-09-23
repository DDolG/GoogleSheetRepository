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
            AddSheet(name);
        }

        public bool HavePage(string name)
        {
            return CanReadSheet(name);
        }

        private bool CanReadSheet(string name)
        {
            var settings = _googleSheetService.GetSettings();
            var range = $"{name}!A1";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(settings.SheetId, range);
            try
            {
                var response = request.Execute();
                return response != null;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        private void AddSheet(string newSheetTitle)
        {
            var settings = _googleSheetService.GetSettings();
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
            BatchUpdateSpreadsheetRequest batchUpdateRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request> { request }
            };
            try
            {
                _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, settings.SheetId).Execute();
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Create page error: {ex.Message}");
            }
        }
    }
}