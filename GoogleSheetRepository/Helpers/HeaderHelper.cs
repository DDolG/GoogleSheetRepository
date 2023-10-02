using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleSheetRepository.Extensions;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Models;
using System.Reflection;

namespace GoogleSheetRepository.Helpers
{
    public class HeaderHelper : IHeader
    {
        private readonly SheetsService _sheetsService;
        private readonly GoogleSheetSettings _settings;
        private readonly string _sheetName;
        private IList<PropertyInfo> _properties;

        public HeaderHelper(ISettings googleSheetService,
                                string sheetName,
                                IList<PropertyInfo> properties)
        {
            _sheetsService = googleSheetService.GetService();
            _settings = googleSheetService.GetSettings();
            _sheetName = sheetName;
            _properties = properties;
        }

        public int? GetPropertyCountFromPage()
        {
            var range = $"{_sheetName}!{Constants.NumberOfPropertyCell}";
            var request = _sheetsService.Spreadsheets.Values.Get(_settings.SheetId, range);
            var response = new ValueRange();
            try
            {
                response = request.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get property headers: {ex.Message}");
            }
            var result = response?.Values?.FirstOrDefault()?.FirstOrDefault()?.ToString();
            if (result == null) return null;
            int.TryParse(result, out var countClassProperties);
            return countClassProperties;
        }

        public List<ColumnPropertyHeader> GetPropertyFromHeader()
        {
            var finishRange = _properties.Count().GetColumnAddressWithHeaderShift() + Constants.HeaderPropertyNameRow;
            var range = $"{_sheetName}!{Constants.HeaderPropertyStartNameCell}:{finishRange}";
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
            var propertiHeaders = response?.Values?.FirstOrDefault();
            var result = propertiHeaders.Select(x => x.GetColumnProperty()).ToList();
            return result;
        }

        public void InitPropertyHeaders()
        {
            var finishRange = _properties.Count().GetColumnAddressWithHeaderShift() + Constants.HeaderPropertyNameRow;
            var range = $"{_sheetName}!{Constants.HeaderPropertyStartNameCell}:{finishRange}";
            var valueRange = new ValueRange();
            var oblList = _properties.Select(x => x.GetPropertyDescription()).ToList();
            valueRange.Values = new List<IList<object>> { oblList };
            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _settings.SheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            UpdateValuesResponse appendReponse = new UpdateValuesResponse();
            try
            {
                appendReponse = updateRequest.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error try write: {ex.Message}");
            }
            Console.WriteLine($"Write count property response: {appendReponse.ToString()}");
        }

        public void SetPropertyCount()
        {
            var range = $"{_sheetName}!{Constants.NumberOfPropertyCell}";
            var valueRange = new ValueRange();
            var countProperty = _properties.Count;
            var oblist = new List<object>() { countProperty };
            valueRange.Values = new List<IList<object>> { oblist };
            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _settings.SheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            try
            {
                var appendReponse = updateRequest.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error write count property: {ex.Message}");
                return;
            }
            Console.WriteLine($"Success write property count: {countProperty.ToString()}");
        }
    }
}
