using Microsoft.AspNetCore.Mvc;
using MyFreeFormForm.Data;
using MyFreeFormForm.Helpers;
using MyFreeFormForm.Models;
using Newtonsoft.Json;

namespace MyFreeFormForm.Controllers
{

    [Route("forms")]
    public class FormsController : Controller
    {
        private readonly FileParser _fileParser;
        private readonly ApplicationDbContext _context;

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
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("SubmitDynamicForm: " + JsonConvert.SerializeObject(model));

            if (ModelState.IsValid)
            {
                 var form = new Form { FormName = model.FormName, Description = model.Description, CreatedDate = DateTime.Now };
                 _context.Forms.Add(form);
                 _context.SaveChanges();
                 foreach (var field in model.Fields)
                 {
                     var formField = new FormField { FormId = form.FormId, FieldName = field.FieldName, FieldType = field.FieldType.ToString(), FieldValue = field.FieldValue.ToString() };
                     _context.FormFields.Add(formField);
                 }
                 var formNotes = new FormNotes { FormId = form.FormId, Note = "Form submitted", CreatedDate = DateTime.Now };
                 _context.FormNotes.Add(formNotes);
                 _context.SaveChanges();
                return Json(new { success = true, message = "Form submitted successfully" });
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Validation failed", errors = errors });
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

        [HttpPost("UploadedDocument")]
        public IActionResult UploadedDocument(DynamicFormModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the dynamic form data here
                return RedirectToAction("SuccessPage"); // Redirect to a success notification page
            }            

            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("UploadedDocuments: " + model);

            return View("UploadedDocument", model);
        }


    }

}
