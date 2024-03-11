using Microsoft.AspNetCore.Mvc;
using MyFreeFormForm.Helpers;
using MyFreeFormForm.Models;

namespace MyFreeFormForm.Controllers
{

    [Route("forms")]
    public class FormsController : Controller
    {
        private readonly FileParser _fileParser;

        // Use constructor injection to get a FileParser instance
        public FormsController(FileParser fileParser)
        {
            _fileParser = fileParser;
        }

        
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
                try
                {
                    // Assuming you want to handle Excel files specifically
                    // and fall back to CSV for other cases
                    List<Dictionary<string, string>> parsedData = null; // To store parsed data
                    if (fileUpload.FileName.EndsWith(".xlsx"))
                    {
                        // Parse Excel file
                        parsedData = await _fileParser.ParseExcelFile(fileUpload);

                        //Send data to the view
                        //return View("ViewData", parsedData);
                    }
                    else if (fileUpload.FileName.EndsWith(".csv"))
                    {
                        // Parse CSV file
                        parsedData = await _fileParser.ParseCsvFile(fileUpload);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Unsupported file format" });
                    }

                    // Optionally process parsedData here or add it to the ViewBag/ViewData if needed for the return view
                    // For example, you could store it in session or temp data to display in the view
                    // HttpContext.Session.SetObject("UploadedData", parsedData); // Ensure session is configured in Startup.cs

                    return Json(new { success = true, message = "File processed successfully", fields = parsedData });
                }
                catch (Exception ex)
                {
                    // Log the error
                    return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
                }
            }
            return Json(new { success = false, message = "File upload failed" });
        }

    }

}
