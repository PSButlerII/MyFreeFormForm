using Microsoft.AspNetCore.Mvc;
using MyFreeFormForm.Data;
using MyFreeFormForm.Helpers;
using MyFreeFormForm.Models;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace MyFreeFormForm.Controllers
{

    [Route("forms")]
    public class FormsController : Controller
    {
        private readonly FileParser _fileParser;
        private readonly ApplicationDbContext _context;
        public FieldOptions fieldOptions;

        // Use constructor injection to get a FileParser instance
        public FormsController(FileParser fileParser, ApplicationDbContext context)
        {
            _fileParser = fileParser;
            _context = context;
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
        public async Task<IActionResult> SubmitDynamicForm([FromForm] DynamicFormModel model)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("SubmitDynamicForm: " + JsonConvert.SerializeObject(model));

            if (ModelState.IsValid)
            {
                var form = await Request.ReadFormAsync();
                var formName = form["FormName"];
                var description = form["Description"];
                var indices = new HashSet<int>();

                // Regular expression to match field indices
                var regex = new Regex(@"Fields\[(\d+)\]");

                foreach (var key in form.Keys)
                {
                    var match = regex.Match(key);
                    if (match.Success)
                    {
                        // If the key matches the pattern, add the index to the set
                        indices.Add(int.Parse(match.Groups[1].Value));
                    }
                }
                /*   var fields = form["Fields[0].FieldName"];
                   var fieldValues = form["Fields[0].FieldValue"];
                   var fieldTypes = form["Fields[0].FieldType"];*/
                foreach (var index in indices)
                {
                    var fieldNameKey = $"Fields[{index}].FieldName";
                    var fieldValueKey = $"Fields[{index}].FieldValue";
                    var fieldTypeKey = $"Fields[{index}].FieldType";

                    var fieldName = form[fieldNameKey];
                    var fieldValue = form[fieldValueKey];
                    var fieldType = form[fieldTypeKey];

                    // Process each field here
                    // For example, you might log them or add them to a list


                    var myForm = new Form { FormName = formName, Description = description, CreatedDate = DateTime.Now };
                    myForm.FormFields = new List<FormField>();

                    for (int i = 0; i < fieldName.Count; i++)
                    {
                        var formField = new FormField
                        {
                            // if the FieldId is not set, it will be set to 0. check if the FieldId is 0, if it is, then set the FormId to the FormId of the form being created
                            FormId = myForm.FormId,
                            FieldName = fieldName[i],
                            FieldType = fieldType[i],
                            FieldValue = fieldValue[i],
                            Required = true,
                            FieldOptions = fieldOptions,
                            Form = myForm
                        };
                        myForm.FormFields.Add(formField);
                        _context.FormFields.Add(formField);

                    }

                    var formNotes = new FormNotes { FormId = myForm.FormId, Note = "Form submitted", CreatedDate = DateTime.Now };
                    _context.FormNotes.Add(formNotes);

                    myForm.FormNotes = new List<FormNotes> { formNotes };
                    //await _context.SaveChangesAsync();

                    _context.Forms.Add(myForm);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Form submitted successfully" });
                }
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
                        // May need to set the FieldOption to json when parsing the excel file
                        fieldOptions = FieldOptions.JSON;
                    }
                    else if (fileUpload.FileName.EndsWith(".csv"))
                    {
                        // Parse CSV file
                        parsedData = await _fileParser.ParseCsvFile(fileUpload);
                        // Get the delimiter from the parsedData using the key "Delimiter" and set the FieldOption to the delimiter in the parsedData(eg. "," or ";" or "|" or ":")
                        // Assuming 'delimiter' is a string containing the symbol like "," or ";"
                        string delimiterSymbol = parsedData.Last().ContainsKey("Delimiter") ? parsedData.Last()["Delimiter"].ToString() : string.Empty;
                        fieldOptions = GetFieldOptionFromDelimiter(delimiterSymbol);
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

        public FieldOptions GetFieldOptionFromDelimiter(string delimiter)
        {
            switch (delimiter)
            {
                case ",":
                    return FieldOptions.Comma;
                case "|":
                    return FieldOptions.Pipe;
                case ";":
                    return FieldOptions.Semicolon;
                case ":":
                    return FieldOptions.Colon;
                case " ":
                    return FieldOptions.Space;
                default:
                    return FieldOptions.JSON; // Default or if no match is found
            }
        }

    }

}
