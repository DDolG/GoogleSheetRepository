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

        private void InitRepository()
        {

            var havePage = _sheetHelper.HavePage();
            if (!havePage) _sheetHelper.Create();
 
            var CountProperty = _headerHelper.GetPropertyCountFromPage();

            if (CountProperty == null)
            {
                _headerHelper.SetPropertyCount();
                _headerHelper.InitPropertyHeaders();
                CountProperty = _properties.Count();
            }
            else if(CountProperty !=  _properties.Count)
            {
                throw new Exception("Error! Number of property changed.");
            }

            //check column by name
            var headerProperties = _headerHelper.GetPropertyFromHeader();
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
        
        public long Add(T item)
        {
            var lastRowNumber = _sheetHelper.GetLastRowNumber();
            var numberRowForSave = lastRowNumber + 1;
            var finishRange = _properties.Count().GetColumnAddressWithHeaderShift() + numberRowForSave.ToString();
            var range = $"{_sheetName}!{Constants.ColumnForBeginWriteData}{numberRowForSave}:{finishRange}";
            var valueRange = new ValueRange();
            var oblList = _properties.Select(x => (object)x.GetValue(item)).ToList();
            valueRange.Values = new List<IList<object>> { oblList };
            var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _settings.SheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            AppendValuesResponse appendReponse = new AppendValuesResponse();
            try
            {
                appendReponse = appendRequest.Execute();
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error try write record: {ex.Message}");
            }
            Console.WriteLine($"Succes write record response: {appendReponse.ToString()}");
            return (long)numberRowForSave;
        }

        public bool Delete(T item)
        {
            var deleteRowIndex = GetRowNumber(item);
            var result = DeleteRow(deleteRowIndex);
            return result;
        }
        
        public List<T> Get()
        {
            var lastRow = _sheetHelper.GetLastRowNumber();
            var finishRange = _properties.Count().GetColumnAddressWithHeaderShift() + lastRow.ToString();
            var range = $"{_sheetName}!{Constants.ColumnForBeginWriteData}{Constants.RowForBeginWriteData}:{finishRange}";
            var request = _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = new ValueRange();
            try
            {
                response = request.Execute();
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

        public List<T> Get(int skip, int take)
        {
            skip += Constants.RowHeaderNumber;
            var lastRow = _sheetHelper.GetLastRowNumber();
            var beginData = skip > lastRow ? lastRow : skip;
            var endData = (skip + take) > lastRow ? lastRow : (skip + take - 1);
            var finishRange = _properties.Count().GetColumnAddressWithHeaderShift() + endData.ToString();
            var range = $"{_sheetName}!{Constants.ColumnForBeginWriteData}{beginData}:{finishRange}";
            var request = _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = new ValueRange();
            try
            {
                response = request.Execute();
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


        public int GetRowNumber(T item)
        {
            var values = Get();
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
        
        public bool Update(T oldItem,T item)
        {
            var updateItemIndex = GetRowNumber(oldItem);   
            var result = UpdateRow(updateItemIndex, item);
            return result;
        }

        private bool UpdateRow(int rowNumber, T item)
        {
            var finishRange = _properties.Count().GetColumnAddressWithHeaderShift() + rowNumber.ToString();
            var range = $"{_sheetName}!{Constants.ColumnForBeginWriteData}{rowNumber}:{finishRange}";
            var valueRange = new ValueRange();
            var oblList = _properties.Select(x => (object)x.GetValue(item)).ToList();
            valueRange.Values = new List<IList<object>> { oblList };
            var request = _sheetsService.Spreadsheets.Values.Update(valueRange, _settings.SheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var appendReponse = new UpdateValuesResponse();
            try
            {
                appendReponse = request.Execute();
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error try update record: {ex.Message}");
            }
            Console.WriteLine($"Succes update record response: {appendReponse.ToString()}");
            return true;
        }


        private bool DeleteRow(int rowIndex)
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
