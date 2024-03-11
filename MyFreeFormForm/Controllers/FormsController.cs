using Microsoft.AspNetCore.Mvc;
using MyFreeFormForm.Helpers;
using MyFreeFormForm.Models;

namespace MyFreeFormForm.Controllers
{
    [Route("forms")]
    public class FormsController : Controller
    {
        [HttpGet("static")]
        public IActionResult StaticForm()
        {
            return View();
        }

        [HttpPost("static")]
        public IActionResult SubmitStaticForm(StaticFormModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the static form data here
                return RedirectToAction("SuccessPage"); // Redirect to a success notification page
            }

            return View("StaticForm", model);
        }
        

        [HttpGet("dynamic")]
        public IActionResult CreateDynamicForm()
        {
            var model = new DynamicFormModel();
            // Pre-populate model.Fields with some example fields
            model.Fields.Add(new DynamicField { FieldName = "First Name", FieldType = FieldType.Text });
            model.Fields.Add(new DynamicField { FieldName = "Last Name", FieldType = FieldType.Text });
            model.Fields.Add(new DynamicField { FieldName = "Email", FieldType = FieldType.Email });
            model.Fields.Add(new DynamicField { FieldName = "Date of Birth", FieldType = FieldType.Date });
            return View(model);
        }

        [HttpPost("dynamic")]
        public IActionResult SubmitDynamicForm(DynamicFormModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the dynamic form data here
                return RedirectToAction("SuccessPage"); // Redirect to a success notification page
            }

            return View("CreateDynamicForm", model);
        }

    }

}
