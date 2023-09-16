using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Models;

namespace GoogleSheetRepository
{
    public class GSRepositoryService<T> : IGSRepository<T> where T : class
    {
        private readonly SheetsService _sheetsService;
        private readonly IGSSheetControl _sheetControlService;
        private readonly GoogleSheetSettings _settings;
        private string _pageName;
        private int? CountProperty { get; set; }

        public GSRepositoryService(IGSService googleSheetService, 
            IGSSheetControl sheetControlService)
        {
            _sheetsService = googleSheetService.GetService();
            _sheetControlService = sheetControlService;
            _settings = googleSheetService.GetSettings();
        }

        private void InitPage()
        {
            var genericType = typeof(T);
            _pageName = genericType.Name;

            var havePage = _sheetControlService.HavePage(_pageName);
            if (!havePage) _sheetControlService.Create(_pageName);

            var properties = genericType.GetProperties().OrderBy(x=>x.Name).ToList();
            var CountProperty = GetPropertyCount();

            if (CountProperty == null)
            {
                //init
                SetPropertyCount();
                InitPropertyHeaders();
            }
            else if(CountProperty !=  properties.Count)
            {
                //exeception dto update
            }

            //check column by name
        }

        private int? GetPropertyCount()
        {
            var range = $"{_pageName}!A1";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = request.Execute();
            var result = response.Values.FirstOrDefault().ToString();
            int.TryParse(result, out var countClassProperties);
            return countClassProperties;
        }

        private void SetPropertyCount()
        {
            var range = $"{_pageName}!A1";
            var valueRange = new ValueRange();
            var oblist = new List<object>() { CountProperty };
            valueRange.Values = new List<IList<object>> { oblist };
            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _settings.SheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = updateRequest.Execute();
            Console.WriteLine($"Write count property response: {appendReponse}");
        }

        private void InitPropertyHeaders()
        {
            var genericType = typeof(T);
            var properties = genericType.GetProperties().OrderBy(x => x.Name).ToList();
            var range = $"{_pageName}!A2:A{properties.Count()+1}";
            var valueRange = new ValueRange();
            var oblList = new List<object>() { properties.Select(x => x.Name).ToList() };
            valueRange.Values = new List<IList<object>> { oblList };
            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _settings.SheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = updateRequest.Execute();
            Console.WriteLine($"Write count property response: {appendReponse}");
        }


        Task<long> IGSRepository<T>.AddAsync(T item)
        {
            throw new NotImplementedException();
        }

        Task<bool> IGSRepository<T>.DeleteAsync(long itemId)
        {
            throw new NotImplementedException();
        }

        Task<T> IGSRepository<T>.GetAsync()
        {
            throw new NotImplementedException();
        }

        Task<bool> IGSRepository<T>.UpdateAsync(T item)
        {
            throw new NotImplementedException();
        }
    }
}
