using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleSheetRepository.Extensions;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Models;
using System;
using static Google.Apis.Requests.BatchRequest;

namespace GoogleSheetRepository
{
    public class GSRepositoryService<T> : IGSRepository<T> where T : class, new()
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
            InitPage();
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
                SetPropertyCount();
                InitPropertyHeaders();
                CountProperty = properties.Count();
            }
            else if(CountProperty !=  properties.Count)
            {
                throw new Exception("Error! Number of property changed.");
            }

            //check column by name
            var headerProperties = GetPropertyFromHeader();
            var objectProperties = properties.Select(x => new ColumnPropertyHeader
            {
                Name = x.Name,
                PropertyType = x.PropertyType.ToString()
            }).ToList();

            var exceptProperties = headerProperties.Where(x=>!objectProperties.Contains(x)).ToList();
            if (exceptProperties.Any())
            {
                throw new ArgumentException($"Сhanged the property name or type: {string.Join(',', exceptProperties.Select(x => x.ToString()))}");
            }

        }

        private List<ColumnPropertyHeader> GetPropertyFromHeader()
        {
            var genericType = typeof(T);
            var properties = genericType.GetProperties().OrderBy(x => x.Name).ToList();
            var finishRange = properties.Count().GetFinishRangeCell();
            var range = $"{_pageName}!{Constants.HeaderPropertyStartNameCell}:{finishRange}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = new ValueRange();
            try
            {
                response = request.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get count property {ex.Message}");
            }
            var propertiHeaders = response?.Values?.FirstOrDefault();
            var result = propertiHeaders.Select(x => x.GetColumnProperty()).ToList();
            return result;
        }

        private int? GetPropertyCount()
        {
            var range = $"{_pageName}!{Constants.NumberOfPropertyCell}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = new ValueRange();
            try
            {
                response = request.Execute();
            }catch(Exception ex){
                Console.WriteLine($"Error when get count property {ex.Message}");
            }

            var result = response?.Values?.FirstOrDefault()?.FirstOrDefault()?.ToString();
            if(result == null) return null;
            int.TryParse(result, out var countClassProperties);
            return countClassProperties;
        }

        private void SetPropertyCount()
        {
            var range = $"{_pageName}!{Constants.NumberOfPropertyCell}";
            var valueRange = new ValueRange();
            var genericType = typeof(T);
            var countProperty = genericType.GetProperties().ToList().Count;
            var oblist = new List<object>() {countProperty};
            valueRange.Values = new List<IList<object>> { oblist };
            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _settings.SheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            try
            {
                var appendReponse = updateRequest.Execute();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error write count property: {ex.Message}");
                return;
            }
            Console.WriteLine($"Success write property count: {countProperty}");
        }



        private void InitPropertyHeaders()
        {
            var genericType = typeof(T);
            var properties = genericType.GetProperties().OrderBy(x => x.Name).ToList();
            var finishRange = properties.Count().GetFinishRangeCell();
            var range = $"{_pageName}!{Constants.HeaderPropertyStartNameCell}:{finishRange}";
            var valueRange = new ValueRange();
            var oblList = properties.Select(x => x.GetPropertyDescription()).ToList();
            valueRange.Values = new List<IList<object>> {oblList};
            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _settings.SheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = updateRequest.Execute();
            Console.WriteLine($"Write count property response: {appendReponse}");
        }



        async Task<long> IGSRepository<T>.AddAsync(T item)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IGSRepository<T>.DeleteAsync(long itemId)
        {
            throw new NotImplementedException();
        }

        async Task<T> IGSRepository<T>.GetAsync()
        {
            var range = $"{_settings.SheetName}!A2:B2";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = await request.ExecuteAsync();
            IList<IList<object>> values = response.Values;
            T result = new T();
            return result;
        }

        async Task<bool> IGSRepository<T>.UpdateAsync(T item)
        {
            throw new NotImplementedException();
        }
    }
}
