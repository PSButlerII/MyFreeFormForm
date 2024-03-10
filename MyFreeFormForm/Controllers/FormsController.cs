using Microsoft.AspNetCore.Mvc;
using MyFreeFormForm.Models;

namespace MyFreeFormForm.Controllers
{
    public class FormsController : Controller
    {
        [HttpGet]
        public IActionResult SampleForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitForm(FormFieldModel model)
        {
            if (ModelState.IsValid)
            {
                // Process form data
                return RedirectToAction("Success"); // Redirect to a success action/page
            }

            return View("SampleForm", model); // Return the view with validation messages
        }
    }

}
