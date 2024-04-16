using Microsoft.AspNetCore.SignalR;

namespace MyFreeFormForm.Services
{
    public class DataService
    {
        private readonly FormsDbc _formsDbc;
        private readonly IHubContext<DataHub> _hubContext;

        public DataService(FormsDbc formsDbc, IHubContext<DataHub> hubContext)
        {
            _formsDbc = formsDbc;
            _hubContext = hubContext;
        }

        public async Task UpdateDateFieldTrends(string fieldName, DateTime startDate, DateTime endDate,string userId)
        {
            var dateFieldData = await _formsDbc.CountEntriesByDateField(fieldName, startDate, endDate,userId);
            var labels = dateFieldData.Select(d => d.Key.ToString("yyyy-MM-dd")).ToList();
            var values = dateFieldData.Select(d => d.Value).ToList();
            await _hubContext.Clients.All.SendAsync("UpdateChart", new { Labels = labels, Data = values, Title = $"Entries for {fieldName}", StartDate = startDate, EndDate = endDate });
        }

        // New method to count specific values in a field
        public async Task CountSpecificValues(string fieldName, string userId)
        {
            int count = await _formsDbc.CountSpecificFieldValue(fieldName,userId);
            await _hubContext.Clients.All.SendAsync("UpdateValue", new { FieldName = fieldName, Counts = count });
            
        }

        // New method to count entries by date field for trend analysis
        public async Task CountEntriesByDateField(string fieldName, DateTime startDate, DateTime endDate, string userId)
        {
            var dateFieldData = await _formsDbc.CountEntriesByDateField(fieldName, startDate, endDate,userId);
            // get count of entries for each date
            var counts = dateFieldData.Select(d => d.Value).ToList();
            // get the date for each entry
            var entryDates = dateFieldData.Select(d => d.Key).ToList();

            var labels = dateFieldData.Select(d => d.Key.ToString("yyyy-MM-dd")).ToList();
            var values = dateFieldData.Select(d => d.Value).ToList();
            await _hubContext.Clients.All.SendAsync("UpdateChart", new { Labels = labels, Data = values, Title = $"Entries for {fieldName}", StartDate = startDate, EndDate = endDate });
            
        }

        // New method to Count specific field values
        public async Task CountSpecificFieldValue(string fieldName, DateTime startDate, DateTime endDate, string userId)
        {
            int count = await _formsDbc.CountSpecificFieldValue(fieldName, userId);
            var title = $"Count of {fieldName}";
            var labels = fieldName;
            await _hubContext.Clients.All.SendAsync("UpdateValue", new { FieldName = fieldName, Data = count, Title=title, Labels=labels, StartDate = startDate, EndDate = endDate });
            
        }

        // Consolidated methods for counting specific values or trends
        public async Task CountFieldValueOccurrences(string fieldName, string userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var data = await _formsDbc.CountFieldValueOccurrences(fieldName, startDate, endDate,userId);
            var labels = data.Select(f=>f.FormFields.FirstOrDefault(ff=>ff.FieldName==fieldName)).ToList();
            var values = data.Select(f => f.FormFields.FirstOrDefault(ff => ff.FieldName == fieldName).FieldValue).ToList();
            var title = startDate.HasValue && endDate.HasValue ? $"Count of {fieldName} from {startDate.Value.ToShortDateString()} to {endDate.Value.ToShortDateString()}" : $"Count of {fieldName}";
            await _hubContext.Clients.All.SendAsync("UpdateChart", new {FieldName=fieldName, Labels = labels, Data = data, Title = title, StartDate = startDate, EndDate = endDate });
        }
        // New method to get entries that are expiring or have an upcoming date
        /* THIS WORKS.*/
        public async Task GetExpiringEntries(string dateField, DateTime startDate, DateTime endDate, string userId)
        {
            var expiringForms = await _formsDbc.GetExpiringEntries(dateField, startDate, endDate,userId);

            var labels = expiringForms.Select(f => $"{f.CreatedDate.ToShortDateString()} - {f.FormName}").ToList();
            var data = expiringForms.Select(f => f.FormFields.FirstOrDefault(ff => ff.FieldName == dateField).FieldDateValue).ToList();
            var title = $"Expiring forms from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}";

            await _hubContext.Clients.All.SendAsync("UpdateExpiringEntries", new { Labels = labels, Data = data, Title = title, StartDate = startDate, EndDate = endDate });
        }

        public async Task UpdateData()
        {
            // Example: Fetch form count
            var formCount = _formsDbc.GetForms().Count;

            var newData = new { Time = DateTime.Now.ToString("HH:mm"), Value = formCount };
            await _hubContext.Clients.All.SendAsync("ReceiveData", newData);
           
        }
        // Add other methods similarly...
    }

}
