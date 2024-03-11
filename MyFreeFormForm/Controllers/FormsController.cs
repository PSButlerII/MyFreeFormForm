using Microsoft.AspNetCore.Mvc;
using MyFreeFormForm.Helpers;
using MyFreeFormForm.Models;

namespace MyFreeFormForm.Controllers
{
    [Route("forms")]
    public class FormsController : Controller
    {
        private readonly FileParser _fileParser;
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

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile fileUpload)
        {
            if (fileUpload != null && fileUpload.Length > 0)
            {
                // Assuming you want to handle Excel files specifically
                // and fall back to CSV for other cases
                if (fileUpload.FileName.EndsWith(".xlsx"))
                {
                    // Parse Excel file
                    await _fileParser.ParseExcelFile(fileUpload);
                }
                else if (fileUpload.FileName.EndsWith(".csv"))
                {
                    // Parse CSV file
                    await _fileParser.ParseCsvFile(fileUpload);
                }
                else
                {
                    return View("Error", new ErrorViewModel { RequestId = "Unsupported file format" });
                }

                return RedirectToAction("FormCreatedSuccessfully"); // Redirect to a success page or action
            }
            return View("Error", new ErrorViewModel { RequestId = "File upload failed" });
        }
    }

}
