using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyFreeFormForm.Data;
using MyFreeFormForm.Helpers;
using MyFreeFormForm.Models;
using MyFreeFormForm.Services;
using MyFreeFormForm.Views.Forms;
using Newtonsoft.Json;
using Serilog.Sinks.SystemConsole.Themes;
using System.IO;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MyFreeFormForm.Controllers
{

    [Route("forms")]
    public class FormsController : Controller
    {
        private readonly FileParser _fileParser;
        private readonly ApplicationDbContext _context;
        public FieldOptions fieldOptions;
        private readonly ILogger<FormsController> _logger;
        // TODO: Consider using the service instead of the DbContext directly.
        private FormsDbc _formsDbc;
        private static readonly Queue<DynamicFormModel> FormSubmissionQueue = new Queue<DynamicFormModel>();


        // Use constructor injection to get a FileParser instance
        public FormsController(FileParser fileParser, ApplicationDbContext context, FormsDbc formsDbc, ILogger<FormsController> logger)
        {
            _fileParser = fileParser;
            _context = context;
            _formsDbc = formsDbc;
            _logger = logger;

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("static")]
        public IActionResult StaticForm()
        {
            return View();
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>     
        [HttpPost("dynamic")]
        public async Task<IActionResult> SubmitDynamicForm([FromForm] IFormCollection form)
        {
            var validationErrors = new List<string>();
            if (ModelState.IsValid)
            {
                //var form = await Request.ReadFormAsync();
                var formName = form["FormName"];
                var description = form["Description"];
                var formNotes = form["FormNotes"];
                var userId = form["UserId"];
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
                // Initialize the DynamicFormModel
                var dynamicFormModel = new DynamicFormModel
                {
                    FormName = formName,
                    Description = description,
                    Fields = new List<DynamicField>(),
                    FormNotes = new List<FormNotes>(),
                    UserId = userId
                };             
                foreach (var index in indices)
                {
                    var fieldNameKey = $"Fields[{index}].FieldName";
                    var fieldValueKey = $"Fields[{index}].FieldValue";
                    var fieldTypeKey = $"Fields[{index}].FieldType";

                    var fieldNames = form[fieldNameKey];
                    var fieldName = fieldNames.ToString().Split(",");

                    var fieldValues = form[fieldValueKey];
                    var fieldValue = fieldValues.ToString().Split(",");

                    var fieldTypes = form[fieldTypeKey];
                    var fieldTypeee = fieldTypes.ToString().Split(",");

                    for (var i = 0; i < fieldName.Length; i++)
                    {
                        // Using Enum.TryParse with case-insensitive parsing
                        if (Enum.TryParse<FieldType>(fieldTypes[i], ignoreCase: true, out var fieldType))
                        {
                            var pattern = FieldTypePatterns.GetPattern(fieldType);
                            var errorMessage = FieldTypePatterns.GetErrorMessage(fieldType);

                            if (Regex.IsMatch(fieldValues[i], pattern))
                            {
                                dynamicFormModel.Fields.Add(new DynamicField
                                {
                                    FieldName = fieldNames[i],
                                    FieldValue = fieldValues[i],
                                    FieldType = fieldType
                                });
                            }
                            else
                            {
                                validationErrors.Add($"Field '{fieldNames[i]}' error: {errorMessage}");
                            }
                        }
                        else
                        {
                            validationErrors.Add($"Field '{fieldNames[i]}' has an unsupported field type '{fieldTypes[i]}'.");
                        }
                    }
                    // Add the notes to the formNotes list
                    // Note sure if "Note" is the correct key to use here.  Need to check the key in the form data
                    foreach (var key in form.Keys.Where(k => k.StartsWith("FormNote")))
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Note: {form[key]}, and The key from the loop: {key}");
                        var noteValue = form[key];
                        if (!string.IsNullOrEmpty(noteValue))
                        {
                            dynamicFormModel.FormNotes.Add(new FormNotes
                            {
                                Note = new List<string> { noteValue }, // Assuming each FormNotes object contains a list of notes
                                CreatedDate = DateTime.Now,
                                UpdatedDate = DateTime.Now
                            });
                        }
                    }
                    if (!validationErrors.Any())
                    {
                        await _formsDbc.EnqueueFormSubmissionAsync(dynamicFormModel);
                        return Json(new { success = true, message = "Form submission queued" });
                    }
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed", errors = errors });
            }
            return Json(new { success = false, message = "Form submission failed" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("dynamic")]
        public IActionResult CreateDynamicForm()
        {
            var model = new DynamicFormModel();
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // get logged-in userId
            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return Redirect("/Identity/Account/Login");
            }
            else
            {
                ViewData["UserId"] = loggedInUserId;
                model.UserId = loggedInUserId;
            }

            //TODO: Instead of using ViewBag, consider using a ViewModel.  This will make it easier to test and maintain the code.
            var formInstance = _context.Forms
                .AsEnumerable() // AsEnumerable or ToList, depending on your context's capabilities
                .Where(f => f.UserId == loggedInUserId)
                .GroupBy(f => f.FormName)
                .ToDictionary(g => g.Key, g => g.Select(f => f.FormId).ToList());

            ViewBag.FormInstance = formInstance;

            return View(model);
        }


        /// <summary>
        ///  
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delimiter"></param>
        /// <returns></returns>
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

        [HttpGet("patterns")]
        public IActionResult GetFieldTypePatterns()
        {
            var patterns = new Dictionary<string, FieldTypePatternViewModel>();

            foreach (FieldType fieldType in Enum.GetValues(typeof(FieldType)))
            {
                patterns.Add(fieldType.ToString(), new FieldTypePatternViewModel
                {
                    Pattern = FieldTypePatterns.GetPattern(fieldType),
                    ErrorMessage = FieldTypePatterns.GetErrorMessage(fieldType)
                });
            }

            return Ok(patterns);
        }
        /*     /// <summary>
             /// 
             /// </summary>
             /// <param name="formId"></param>
             /// <returns></returns>
             [HttpGet("MyDocuments")]
             public async Task<IActionResult> DisplayForm(int formId)
             {
                 var form = await _context.Forms
                     .Include(f => f.FormFields)
                     .Include(f => f.FormNotes)
                     .FirstOrDefaultAsync(f => f.FormId == formId);

                 if (form == null)
                 {
                     return NotFound();
                 }

                 return View(form);
             }*/

        /// <summary>
        ///  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoadForms")]
        public async Task<IActionResult> LoadForms(string ids)
        {
            var selectedData = new List<Dictionary<string, string>>(); 
            try
            {
                var idList = ids.Split(',').Select(int.Parse).ToList();

                var forms = await _context.Forms
                                          .Where(f => idList.Contains(f.FormId))
                                          .ToListAsync();
                var notes = await _context.FormNotes
                                  .Where(fn => idList.Contains(fn.FormId))
                                  .ToListAsync();

                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"{notes}");

                // Now get the fields for each form and add them to an object
                foreach (var form in forms)
                {
                    var fields = await _context.FormFields
                                              .Where(ff => ff.FormId == form.FormId)
                                              .ToListAsync();

                    var formFields = new List<Dictionary<string, string>>();

                    foreach (var field in fields)
                    {
                        var fieldData = new Dictionary<string, string>
                        {
                            { "FieldName", field.FieldName },
                            { "FieldType", field.FieldType },
                            { "FieldValue", field.FieldValue }
                        };
                        formFields.Add(fieldData);
                    }
                    var formNotes = notes.Where(n => n.FormId == form.FormId)
                                 .Select(n => n.Note) // Assuming NoteText is the note content
                                 .ToList();
                    // Will need to eventual add an entry for files
                    selectedData.Add(new Dictionary<string, string>
                    {
                        { "FormName", form.FormName },
                        { "Description", form.Description },
                        { "Fields", JsonConvert.SerializeObject(formFields) },
                        { "FormNotes", JsonConvert.SerializeObject(formNotes) }

                    });
                }
                return Json(new { success = true, message = "Forms loaded successfully", forms = selectedData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        /// <summary>
        ///  This method will return the field types from the enum
        /// </summary>
        /// <returns></returns>
        [HttpGet("FieldTypes")]
        public IActionResult GetFieldTypes()
        {
            // Get the field types from the enum
            var fieldTypes = Enum.GetNames(typeof(FieldType)).ToList();
            return Json(new { success = true, message = "Field types loaded successfully", fieldTypes });
        }

        public void UpdateViewBag()
        {
            var formInstance = _context.Forms
                .AsEnumerable() // AsEnumerable or ToList, depending on your context's capabilities
                .GroupBy(f => f.FormName)
                .ToDictionary(g => g.Key, g => g.Select(f => f.FormId).ToList());
            // Then pass the formInstance to the view
            ViewBag.FormInstance = formInstance;
        }

        [HttpGet("UpdateFormNotes")]
        public async Task<IActionResult> UpdateFormNotes(int formId)
        {
            _logger.LogInformation("Fetching notes for form ID: {FormId}", formId);
            try
            {
                var formNotes = await _formsDbc.GetNotes(formId);
                var formNotesList = new List<string>();
                //Notes is a string list.  Check is the formNotes.Notes is empty
                if (formNotes != null)
                {
                    foreach (var note in formNotes)
                    {
                        formNotesList.Add(note.Note.ToString());
                    }
                }
                return PartialView("_formNotes", formNotes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching form notes for form ID: {FormId}", formId);
                throw;
            }
        }

        [HttpPost("UpdateFormNotes")]
        public async Task<IActionResult> AddFormNotes(int formId, string note)
        {// TODO: Implement logic that will allow you to add notes to a form without submitting the form
            try
            {
                var form = await _formsDbc.GetFormByIdAsync(formId);

                if (form == null)
                {
                    return NotFound();
                }

                if (form.FormNotes == null)
                {
                    form.FormNotes = new List<FormNotes>();
                }

                form.FormNotes.Add(new FormNotes
                {
                    Note = new List<string> { note },
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                });

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Note added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding note to form ID: {FormId}", formId);
                return Json(new { success = false, message = "An error occurred while adding the note" });
            }
        }

        [HttpGet("ManageForms/{userId}")]
        public async Task<IActionResult> ManageForms(string userId)
        {
            var forms = await _context.Forms
                .Where(f => f.UserId == userId)
                .ToListAsync();
            var GroupedForms = forms.GroupBy(f => f.FormName)
                            .ToDictionary(g => g.Key, g => g.ToList());

            var viewModel = new ManageModel
            {
                GroupedForms = GroupedForms
            };

            return View(viewModel);
        }

        /// <summary>
        /// Deletes a static form based on its identifier.
        /// </summary>
        /// <param name="id">The identifier of the form to delete.</param>
        /// <returns></returns>
        [HttpPost("delete-static")]
        public IActionResult DeleteStaticForm(int id)
        {
            if (_formsDbc.DeleteStaticForm(id)) // Assuming _formsDb.DeleteStaticForm handles the deletion logic
            {
                return RedirectToAction("DeletionSuccessPage"); // Redirect to a success notification page
            }

            return View("ErrorPage"); // Redirect to an error page if deletion fails
        }

        /// <summary>
        /// Deletes a dynamic form based on its identifier.
        /// </summary>
        /// <param name="id">The identifier of the form to delete.</param>
        /// <returns></returns>
        [HttpPost("delete-dynamic")]
        public async Task<IActionResult> DeleteDynamicForm(int id)
        {
            //Can remove the todo comment completed: 4/23/2024
            //TODO: Implement the logic to delete a dynamic form
            var form = await _formsDbc.GetFormByIdAsync(id); // Fetch the form data using the id
            if (form != null)
            {
                var deleteResult = await _formsDbc.DeleteFormAsync(id); // Assuming DeleteFormAsync handles the deletion logic
                if (deleteResult)
                {
                    return Json(new { success = true, message = "Form successfully deleted" });
                }
            }

            return Json(new { success = false, message = "Form deletion failed" });
        }

      /*  /// <summary>
        /// Deletes all forms under a specific form name.
        [HttpDelete("delete-form")]*/

    }
}