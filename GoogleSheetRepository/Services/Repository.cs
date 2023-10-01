using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleSheetRepository.Extensions;
using GoogleSheetRepository.Helpers;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Models;
using System.Reflection;
using System.Text.Json;

namespace GoogleSheetRepository
{
    public class Repository<T> : IRepository<T> where T : class, IEquatable<T>, new()
    {
        private readonly SheetsService _sheetsService;
        private readonly ISheet _sheetHelper;
        private readonly GoogleSheetSettings _settings;
        private readonly IHeader _headerHelper;
        private string _sheetName;
        private IList<PropertyInfo> _properties;

        public Repository(ISettings googleSheetService)
        {
            _sheetsService = googleSheetService.GetService();
            _settings = googleSheetService.GetSettings();
            var genericType = typeof(T);
            _sheetName = genericType.Name;
            _properties = genericType.GetProperties().OrderBy(x => x.Name).ToList();
            _sheetHelper= new SheetHelper(googleSheetService, _sheetName);
            _headerHelper = new HeaderHelper(googleSheetService, _sheetName, _properties);

            InitRepository();
        }

        private async Task InitRepository()
        {

            var havePage = _sheetHelper.HavePage();
            if (!havePage) _sheetHelper.Create();
 
            var CountProperty = await _headerHelper.GetPropertyCountFromPageAsync();

            if (CountProperty == null)
            {
                await _headerHelper.SetPropertyCountAsync();
                await _headerHelper.InitPropertyHeadersAsync();
                CountProperty = _properties.Count();
            }
            else if(CountProperty !=  _properties.Count)
            {
                throw new Exception("Error! Number of property changed.");
            }

            //check column by name
            var headerProperties = await _headerHelper.GetPropertyFromHeaderAsync();
            var objectProperties = _properties.Select(x => new ColumnPropertyHeader
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
        
        public async Task<long> AddAsync(T item)
        {
            var lastRowNumber = await _sheetHelper.GetLastRowNumberAsync();
            var numberRowForSave = lastRowNumber + 1;
            var finishRange = _properties.Count().GetFinishColumn() + numberRowForSave.ToString();
            var range = $"{_sheetName}!{Constants.ColumnForBeginWriteData}{numberRowForSave}:{finishRange}";
            var valueRange = new ValueRange();
            var oblList = _properties.Select(x => (object)x.GetValue(item)).ToList();
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

        public async Task<bool> DeleteAsync(T item)
        {
            var deleteRowIndex = await GetRowNumber(item);
            var result = await DeleteRow(deleteRowIndex);
            return result;
        }
        
        public async Task<List<T>> GetAsync()
        {
            var lastRow = await _sheetHelper.GetLastRowNumberAsync();
            var finishRange = _properties.Count().GetFinishColumn() + lastRow.ToString();
            var range = $"{_sheetName}!{Constants.ColumnForBeginWriteData}{Constants.RowForBeginWriteData}:{finishRange}";
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

        public async Task<List<T>> GetAsync(int skip, int take)
        {
            skip += Constants.RowHeaderNumber;
            var lastRow = await _sheetHelper.GetLastRowNumberAsync();
            var beginData = skip > lastRow ? lastRow : skip;
            var endData = (skip + take) > lastRow ? lastRow : (skip + take - 1);
            var finishRange = _properties.Count().GetFinishColumn() + endData.ToString();
            var range = $"{_sheetName}!{Constants.ColumnForBeginWriteData}{beginData}:{finishRange}";
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
            var rows = response.Values.First().ToList();
            var objects = response.Values.Select(x => x.ToList().GetObjectFromProperty<T>());
            return objects.ToList();
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
        
        public async Task<bool> UpdateAsync(T oldItem,T item)
        {
            var updateItemIndex = await GetRowNumber(oldItem);   
            var result = await UpdateRow(updateItemIndex, item);
            return result;
        }

        private async Task<bool> UpdateRow(int rowNumber, T item)
        {
            var finishRange = _properties.Count().GetFinishColumn() + rowNumber.ToString();
            var range = $"{_sheetName}!{Constants.ColumnForBeginWriteData}{rowNumber}:{finishRange}";
            var valueRange = new ValueRange();
            var oblList = _properties.Select(x => (object)x.GetValue(item)).ToList();
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


        private async Task<bool> DeleteRow(int rowIndex)
        {
            var request = new Request
            {
                DeleteDimension = new DeleteDimensionRequest
                {
                    Range = new DimensionRange
                    {
                        SheetId = _sheetHelper.GetSheetId(),
                        Dimension = "ROWS",
                        StartIndex = rowIndex - 1,
                        EndIndex = rowIndex
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
                Console.WriteLine($"Error when try delete row with index: {rowIndex}");
                throw;
            }

            return true;
        }
    }
}
