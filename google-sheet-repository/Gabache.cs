namespace google_sheet_repository
{
    public class Gabache
    {
        /*static Program()
    {
       GoogleCredential credential;
       //Reading Credentials File...
       using (var stream = new FileStream("app_client_secret.json", FileMode.Open, FileAccess.Read))
       {
           credential = GoogleCredential.FromStream(stream)
               .CreateScoped(Scopes);
       }

       // Creating Google Sheets API service...
       service = new SheetsService(new BaseClientService.Initializer()
       {
           HttpClientInitializer = credential,
           ApplicationName = ApplicationName,
       });

       var test = service.Spreadsheets.Get(sheet);
       Console.WriteLine($"Page - {sheet} have: {(test != null)}");
       var googleSheet = new Spreadsheet();
      AddSheet(SpreadsheetId, "Test");
    }
        
         static void AddSheet(string spreadsheetId, string newSheetTitle)
    {
        // Define the new sheet
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

        // Create the batch update request
        BatchUpdateSpreadsheetRequest batchUpdateRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = new List<Request> { request }
        };

        // Execute the request
        service.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId).Execute();
    }


    static void ReadSheet()
    {
        // Specifying Column Range for reading...
        var range = $"{sheet}!A:E";
        SpreadsheetsResource.ValuesResource.GetRequest request =
            service.Spreadsheets.Values.Get(SpreadsheetId, range);
        // Ecexuting Read Operation...
        var response = request.Execute();
        // Getting all records from Column A to E...
        IList<IList<object>> values = response.Values;
        if (values != null && values.Count > 0)
        {
            foreach (var row in values)
            {
                // Writing Data on Console...
                Console.WriteLine("{0} | {1} | {2} | {3}", row[0], row[1], row[2], row[3]);
            }
        }
        else
        {
            Console.WriteLine("No data found.");
        }
    }

    static void AddRow()
    {
        // Specifying Column Range for reading...
        var range = $"{sheet}!A:E";
        var valueRange = new ValueRange();
        // Data for another Student...
        var oblist = new List<object>() { "Harry", "80", "77", "62", "98" };
        valueRange.Values = new List<IList<object>> { oblist };
        // Append the above record...
        var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        var appendReponse = appendRequest.Execute();
    }

    static void UpdateCell()
    {
        // Setting Cell Name...
        var range = $"{sheet}!C5";
        var valueRange = new ValueRange();
        // Setting Cell Value...
        var oblist = new List<object>() { "32" };
        valueRange.Values = new List<IList<object>> { oblist };
        // Performing Update Operation...
        var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        var appendReponse = updateRequest.Execute();
    }

         
         
         
         
         
         
         
         
         
         
         
         
         
         */




    }
}