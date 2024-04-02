using Microsoft.AspNetCore.Mvc;
using MyFreeFormForm.Models;

namespace MyFreeFormForm.Controllers
{
    [Route("log")]
    public class JsLogging : Controller
    {
        private readonly ILogger<JsLogging> _logger;

        public JsLogging(ILogger<JsLogging> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index([FromBody] LogEntry logEntry)
        {
            // You can now access logEntry.Timestamp, logEntry.Level, and logEntry.Message
            System.Console.WriteLine($"{logEntry.Timestamp} [{logEntry.Level}] {logEntry.Message}");
            _logger.LogInformation($"{logEntry.Timestamp} [{logEntry.Level}] {logEntry.Message}");
            return Ok();
        }

    }


}
