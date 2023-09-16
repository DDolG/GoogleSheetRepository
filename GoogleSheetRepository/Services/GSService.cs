using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using GoogleSheetRepository.Interfaces;
using GoogleSheetRepository.Models;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace GoogleSheetRepository.Services
{
    public class GSService : IGSService
    {
        private readonly GoogleSheetSettings _settings;
        private readonly SheetsService _service;
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private readonly GoogleCredential _credential;
        private readonly IConfiguration _configuration;

        public GSService(IConfiguration configuration)
        {
            _configuration = configuration;
 
            _settings = new GoogleSheetSettings()
            {
                SheetName = _configuration["GoogleSheetSettings:SheetName"],
                FCredencialFile = _configuration["GoogleSheetSettings:FCredencialFile"],
                SheetId = _configuration["GoogleSheetSettings:SheetId"]
            };
            
            using (var stream = new FileStream(_settings.FCredencialFile, FileMode.Open, FileAccess.Read))
            {
                _credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            var entryAssembly = Assembly.GetEntryAssembly();
            _service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = entryAssembly?.GetName().Name
            });
        }
        

        public SheetsService GetService()
        {
            return _service;
        }

        public GoogleSheetSettings GetSettings()
        {
            return _settings;
        }
    }
}