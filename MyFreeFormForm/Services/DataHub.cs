using Microsoft.AspNetCore.SignalR;

namespace MyFreeFormForm.Services
{
    public class DataHub:Hub
    {
        private readonly DataService _dataService;
        public readonly ILogger<DataHub> _logger;

        public DataHub(DataService dataService, ILogger<DataHub> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        public async Task SendData(object data)
        {
            await Clients.All.SendAsync("ReceiveData", data);
        }

        public async Task RequestData(string analysisType, string fieldName, DateTime startDate, DateTime endDate, string userId)
        {
            try
            {
                _logger.LogInformation($"Received RequestData with params: {analysisType}, {fieldName}, {startDate}, {endDate}");
                switch (analysisType)
                {
                    case "trendAnalysis":
                        await _dataService.UpdateDateFieldTrends(fieldName, startDate, endDate,userId);
                        break;
                    case "specificCount":
                        await _dataService.CountSpecificFieldValue(fieldName,startDate,endDate,userId);
                        break;
                    case "GetExpiringEntries":
                        await _dataService.GetExpiringEntries(fieldName, startDate, endDate,userId);
                        break;
                    case "CountSpecificFieldValue":
                        await _dataService.CountFieldValueOccurrences(fieldName, userId, startDate, endDate);
                        break;
                    case "CountEntriesByDateField":
                        await _dataService.CountEntriesByDateField(fieldName, startDate, endDate,userId);
                        break;
                    case "FormCounts":
                        await _dataService.UpdateDateFieldTrends(fieldName, startDate, endDate, userId);
                        break;
                    case "FieldUsage":
                        await _dataService.CountSpecificValues(fieldName, userId);
                        break;
                    default:
                        await _dataService.UpdateData();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RequestData");
                throw;
            }
        }
    }
}
