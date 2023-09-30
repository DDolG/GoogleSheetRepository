using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleSheetRepository.Extensions;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Models;
using System;
using System.Reflection;
using System.Text.Json;
using static Google.Apis.Requests.BatchRequest;

namespace GoogleSheetRepository
{
    public class GSRepositoryService<T> : IGSRepository<T> where T : class, IEquatable<T>, new()
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

        private async Task InitPage()
        {
            var genericType = typeof(T);
            _pageName = genericType.Name;

            var havePage = _sheetControlService.HavePage(_pageName);
            if (!havePage) _sheetControlService.Create(_pageName);

            var properties = genericType.GetProperties().OrderBy(x=>x.Name).ToList();
            var CountProperty = await GetPropertyCountFromPageAsync();

            if (CountProperty == null)
            {
                await SetPropertyCountAsync();
                await InitPropertyHeadersAsync();
                CountProperty = properties.Count();
            }
            else if(CountProperty !=  properties.Count)
            {
                throw new Exception("Error! Number of property changed.");
            }

            //check column by name
            var headerProperties = await GetPropertyFromHeaderAsync();
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

        private async Task<List<ColumnPropertyHeader>> GetPropertyFromHeaderAsync()
        {
            var properties = GetProperties();
            var finishRange = properties.Count().GetFinishColumn() + Constants.HeaderPropertyNameRow;
            var range = $"{_pageName}!{Constants.HeaderPropertyStartNameCell}:{finishRange}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = new ValueRange();
            try
            {
                response = await request.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get count property {ex.Message}");
            }
            var propertiHeaders = response?.Values?.FirstOrDefault();
            var result = propertiHeaders.Select(x => x.GetColumnProperty()).ToList();
            return result;
        }

        private async Task<int?> GetPropertyCountFromPageAsync()
        {
            var range = $"{_pageName}!{Constants.NumberOfPropertyCell}";
            var request = _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = new ValueRange();
            try
            {
                response = await request.ExecuteAsync();
            }catch(Exception ex){
                Console.WriteLine($"Error when get property headers: {ex.Message}");
            }
            var result = response?.Values?.FirstOrDefault()?.FirstOrDefault()?.ToString();
            if(result == null) return null;
            int.TryParse(result, out var countClassProperties);
            return countClassProperties;
        }

        private async Task<int?> GetLastRowNumberAsync()
        {
            var range = $"{_pageName}!{Constants.ColumnForSearchLastRecord}";
            var request = _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = new ValueRange();
            try
            {
                response = await request.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get count property {ex.Message}");
            }

            var result = response?.Values?.Count;
            if (result == null) return null;
            return result;
        }

        private async Task  SetPropertyCountAsync()
        {
            var range = $"{_pageName}!{Constants.NumberOfPropertyCell}";
            var valueRange = new ValueRange();
            var countProperty = GetProperties().Count;
            var oblist = new List<object>() {countProperty};
            valueRange.Values = new List<IList<object>> { oblist };
            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _settings.SheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            try
            {
                var appendReponse = await updateRequest.ExecuteAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error write count property: {ex.Message}");
                return;
            }
            Console.WriteLine($"Success write property count: {countProperty.ToString()}");
        }

        private async Task InitPropertyHeadersAsync()
        {
            var genericType = typeof(T);
            var properties = genericType.GetProperties().OrderBy(x => x.Name).ToList();
            var finishRange = properties.Count().GetFinishColumn() + Constants.HeaderPropertyNameRow;
            var range = $"{_pageName}!{Constants.HeaderPropertyStartNameCell}:{finishRange}";
            var valueRange = new ValueRange();
            var oblList = properties.Select(x => x.GetPropertyDescription()).ToList();
            valueRange.Values = new List<IList<object>> {oblList};
            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _settings.SheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            UpdateValuesResponse appendReponse = new UpdateValuesResponse();
            try
            {
                appendReponse = await updateRequest.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error try write: {ex.Message}");
            }
             Console.WriteLine($"Write count property response: {appendReponse.ToString()}");
        }

        public async Task<long> AddAsync(T item)
        {
            var lastRowNumber = await GetLastRowNumberAsync();
            var numberRowForSave = lastRowNumber + 1;
            var properties = GetProperties();
            var finishRange = properties.Count().GetFinishColumn() + numberRowForSave.ToString();
            var range = $"{_pageName}!{Constants.ColumnForBeginWriteData}{numberRowForSave}:{finishRange}";
            var valueRange = new ValueRange();
            var oblList = properties.Select(x => (object)x.GetValue(item)).ToList();
            valueRange.Values = new List<IList<object>> { oblList };
            var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _settings.SheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            AppendValuesResponse appendReponse = new AppendValuesResponse();
            try
            {
                appendReponse = await appendRequest.ExecuteAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error try write record: {ex.Message}");
            }
            Console.WriteLine($"Succes write record response: {appendReponse.ToString()}");
            return (long)numberRowForSave;
        }

        private List<PropertyInfo> GetProperties()
        {
            var genericType = typeof(T);
            return genericType.GetProperties().OrderBy(x => x.Name).ToList();
        }


        public async Task<bool> DeleteAsync(long itemId)
        {
            throw new NotImplementedException();
        }
        
        public async Task<List<T>> GetAsync()
        {
            var lastRow = await GetLastRowNumberAsync();
            var properties = GetProperties();
            var finishRange = properties.Count().GetFinishColumn() + lastRow.ToString();
            var range = $"{_pageName}!{Constants.ColumnForBeginWriteData}{Constants.RowForBeginWriteData}:{finishRange}";
            var request = _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = new ValueRange();
            try
            {
                response = await request.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when read data range {range} message: {ex.Message}");
            }
            if (!response.Values.Any())
            {
                throw new ArgumentOutOfRangeException("Readed range is empty");
            };
            var objects = response.Values.First().ToList();
            var rows = response.Values.Select(x=>x.ToList().GetObjectFromProperty<T>());
            return rows.ToList();
        }

        public async Task<int> GetRowNumber(T item)
        {
            var values = await GetAsync();
            var countRow = 0;
            var indexes = new List<int>();
            foreach (var row in values)
            {
                countRow++;
                if (row.Equals(item))
                {
                    indexes.Add(countRow);
                }
            }

            if (indexes.Count() > 1)
            {
                throw new ArgumentException($"More than two search results for {JsonSerializer.Serialize(item)}");
            }

            if (!indexes.Any())
            {
                throw new ArgumentException($"No search result for {JsonSerializer.Serialize(item)}");
            }

            return indexes.First() + Constants.RowHeaderNumber;
        }

        private async Task<bool> UpdateRow(int rowNumber, T item)
        {
            var properties = GetProperties();
            var finishRange = properties.Count().GetFinishColumn() + rowNumber.ToString();
            var range = $"{_pageName}!{Constants.ColumnForBeginWriteData}{rowNumber}:{finishRange}";
            var valueRange = new ValueRange();
            var oblList = properties.Select(x => (object)x.GetValue(item)).ToList();
            valueRange.Values = new List<IList<object>> { oblList };
            var request = _sheetsService.Spreadsheets.Values.Update(valueRange, _settings.SheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var appendReponse = new UpdateValuesResponse();
            try
            {
                appendReponse = await request.ExecuteAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error try update record: {ex.Message}");
            }
            Console.WriteLine($"Succes update record response: {appendReponse.ToString()}");
            return true;
        }


        public async Task<bool> UpdateAsync(T oldItem,T item)
        {
            var updateItemIndex = await GetRowNumber(oldItem);   
            var result = await UpdateRow(updateItemIndex, item);
            return result;
        }
    }
}
