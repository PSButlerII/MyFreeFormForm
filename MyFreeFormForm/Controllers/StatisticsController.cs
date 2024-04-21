using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyFreeFormForm.Controllers
{
    public class StatisticsController : Controller
    {
        public IActionResult Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // get logged-in userId
            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return Redirect("/Identity/Account/Login");
            }

            return View("Statistics");
        }
    }
}
