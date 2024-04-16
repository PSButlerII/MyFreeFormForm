using Microsoft.AspNetCore.Mvc;

namespace MyFreeFormForm.Controllers
{
    public class StatisticsController : Controller
    {
        public IActionResult Index()
        {
            return View("Statistics");
        }
    }
}
