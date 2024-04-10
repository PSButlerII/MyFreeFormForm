using Microsoft.Data.SqlClient;
using MyFreeFormForm.Data;
using MyFreeFormForm.Models;
using Newtonsoft.Json;
using MyFreeFormForm.Controllers;
using MyFreeFormForm.Helpers;
using static MyFreeFormForm.Services.QueueProcessorHealthCheck;

namespace MyFreeFormForm.Services
{
    public class QueueProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
     
        private FormProcessorService iFormProcessorService;

        private readonly IQueueProcessorMonitor _monitor;

        public QueueProcessor(IServiceProvider serviceProvider, IQueueProcessorMonitor monitor)
        {
            _serviceProvider = serviceProvider;
            _monitor = monitor;
        }
        // If you don't want the application to stop, the token you should use 
        /// <summary>
        /// This method is called when the <see cref="QueueProcessor"/> starts.
        /// This type of method is called when the application starts, `ExecuteAsync` is called when the service starts.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    iFormProcessorService = new FormProcessorService(dbContext);

                    var itemsToProcess = dbContext.FormSubmissionQueue
                                                   .Where(q => !q.IsProcessed)
                                                   .ToList();

                    foreach (var item in itemsToProcess)
                    {
                        try
                        {
                            //var form = new FormCollection(JsonConvert.DeserializeObject<Dictionary<string, Microsoft.Extensions.Primitives.StringValues>>(item.FormModelData));

                            var model = JsonConvert.DeserializeObject<DynamicFormModel>(item.FormModelData);

                            // Create a serializedFormData string from the form
                           // var serializedFormData = JsonConvert.SerializeObject(form);

                            if (model != null)
                            {
                                // Adapt this method to work with DynamicFormModel directly
                                item.IsProcessed = await iFormProcessorService.ProcessesFormAsync(model);
                                //item.IsProcessed = true;
                                dbContext.Update(item);
                                await dbContext.SaveChangesAsync(stoppingToken);
                                _monitor.IsHealthy = true;
                            }
                            /*
                            dbContext.Update(item);
                            await dbContext.SaveChangesAsync(stoppingToken);
                            */
                        }
                        // Catch sqlExceptions
                        catch (SqlException sqle)
                        {
                            _monitor.IsHealthy = false;
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.WriteLine(sqle);
                        }
                        catch (Exception ex)
                        {
                            _monitor.IsHealthy = false;
                            // Log or handle the exception
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.WriteLine(ex.Message);
                            //throw;
                        }
                    }
                }

                await Task.Delay(10000, stoppingToken); // Wait for 10 seconds before checking the queue again
            }
        }
    }
}