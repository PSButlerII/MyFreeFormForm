using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyFreeFormForm.Services;

namespace MyFreeFormForm.Controllers
{
    public class DataController : ControllerBase
    {
        private readonly IHubContext<DataHub> _hubContext;
        private readonly FormsDbc _formsDbc;

        public DataController(IHubContext<DataHub> hubContext, FormsDbc formsDbc)
        {
            _hubContext = hubContext;
            _formsDbc = formsDbc;
        }

        public async Task<IActionResult> UpdateData()
        {
            // Example: Fetch form count
            var formCount = _formsDbc.GetForms().Count;

            var newData = new { Time = DateTime.Now.ToString("HH:mm"), Value = formCount };
            await _hubContext.Clients.All.SendAsync("ReceiveData", newData);
            return Ok();
        }

        public async Task<IActionResult> UpdateDateFieldTrends(string fieldName, DateTime startDate, DateTime endDate,string userId)
        {
            var dateFieldData = await _formsDbc.CountEntriesByDateField(fieldName, startDate, endDate,userId);
            var labels = dateFieldData.Select(d => d.Key.ToString("yyyy-MM-dd")).ToList();
            var values = dateFieldData.Select(d => d.Value).ToList();
            await _hubContext.Clients.All.SendAsync("UpdateChart", new { Labels = labels, Data = values, Title = $"Entries for {fieldName}" });
            return Ok();
        }

        // New method to count specific values in a field
        public async Task<IActionResult> CountSpecificValues(string fieldName,string userId)
        {
            int count = await _formsDbc.CountSpecificFieldValue(fieldName,userId);
            await _hubContext.Clients.All.SendAsync("UpdateValue", new { FieldName = fieldName, Count = count });
            return Ok();
        }

        // New method to count entries by date field for trend analysis
        public async Task<IActionResult> CountEntriesByDateField(string fieldName, DateTime startDate, DateTime endDate,string userId)
        {
            var dateFieldData = await _formsDbc.CountEntriesByDateField(fieldName, startDate, endDate, userId);
            var labels = dateFieldData.Select(d => d.Key.ToString("yyyy-MM-dd")).ToList();
            var values = dateFieldData.Select(d => d.Value).ToList();
            await _hubContext.Clients.All.SendAsync("UpdateChart", new { Labels = labels, Data = values, Title = $"Entries for {fieldName}" });
            return Ok();
        }

        // New method to Count specific field values
        public async Task<IActionResult> CountSpecificFieldValue(string fieldName, string userId)
        {
            int count = await _formsDbc.CountSpecificFieldValue(fieldName, userId);
            await _hubContext.Clients.All.SendAsync("UpdateValue", new { FieldName = fieldName, Count = count });
            return Ok();
        }

        // New method to get entries that are expiring or have an upcoming date
        public async Task<IActionResult> GetExpiringEntries(string dateField, DateTime startDate, DateTime endDate,string userId)
        {
            var expiringEntries = await _formsDbc.GetExpiringEntries(dateField, startDate, endDate, userId);
            await _hubContext.Clients.All.SendAsync("UpdateExpiringEntries", expiringEntries);
            return Ok();
        }

    }


}
