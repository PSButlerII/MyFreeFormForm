using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFreeFormForm.Data;
using MyFreeFormForm.Helpers;
using MyFreeFormForm.Models;
using MyFreeFormForm.Services;
using Newtonsoft.Json;
using Serilog.Sinks.SystemConsole.Themes;
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
        private readonly ILogger<FormsController> _logger;
        private FormsDbc _formsDbc;

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
        /// <returns></returns>
        [HttpGet("dynamic")]
        public IActionResult CreateDynamicForm()
        {
            var model = new DynamicFormModel();
            // Pre-populate model.Fields with some example fields
          /*  model.Fields.Add(new DynamicField { FieldName = "First Name", FieldType = FieldType.Text });
            model.Fields.Add(new DynamicField { FieldName = "Last Name", FieldType = FieldType.Text });
            model.Fields.Add(new DynamicField { FieldName = "Email", FieldType = FieldType.Email });
            model.Fields.Add(new DynamicField { FieldName = "Date of Birth", FieldType = FieldType.Date });*/
            //TODO: add the submit button


            // Get the form instance from the database using the FormsDbc service getAllForms method
            //var formInstance =  _formsDbc.GetForms();

         /*  TODO:Need to find a way to keep this updated with the latest form instances, so when I add a new form, it will be added to the list of forms.
             One way to do this is to use SignalR to update the formInstance list when a new form is added.
             Another way is to use a service to get the formInstance list from the database and update the list when a new form is added.
             A third way is to use a service to get the formInstance list from the database and update the list when a new form is added, then use SignalR to update the list on the client side.*/

            var formInstance = _context.Forms
                .AsEnumerable() // AsEnumerable or ToList, depending on your context's capabilities
                .GroupBy(f => f.FormName)
                .ToDictionary(g => g.Key, g => g.Select(f => f.FormId).ToList());

            ViewBag.FormInstance = formInstance;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("multipleDynamic")]
        public async Task<IActionResult> SubmitMultipleDynamicForms([FromBody] List<DynamicFormModel> models)
        {
         /*   //TODO: Need to wiring this method to the client side to submit multiple forms at once. Will need to deal with throttling and rate limiting to prevent abuse.  Since this will be going straight to a database, a rate limiter will be necessary to prevent abuse. To do this you will need to use a service to handle the rate limiting and throttling.  An example of this would be: 
            var rateLimiter = new RateLimiter();
            var result = rateLimiter.SubmitMultipleDynamicForms(models);
            if (result.Success)
            {
                return Json(new { success = true, message = $"{models.Count} forms submitted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "An error occurred", errors = result.Errors });
            }*/
            
            Console.BackgroundColor = ConsoleColor.Green;
            //Console.WriteLine("SubmitDynamicForm: " + JsonConvert.SerializeObject(model));

            if (ModelState.IsValid)
            {
                foreach(var model in models)
                {
                    // Directly access properties from the model
                    var formName = model.FormName;
                    var description = model.Description;

                    var myForm = new Form
                    {
                        FormName = formName,
                        Description = description,
                        CreatedDate = DateTime.Now,
                        FormFields = new List<FormField>()
                    };

                    foreach (var field in model.Fields)
                    {
                        var formField = new FormField
                        {
                            FormId = myForm.FormId, // This will likely need adjustment since FormId might not be set until the form is saved
                            FieldName = field.FieldName,
                            FieldType = field.FieldType.ToString(),
                            FieldValue = field.FieldValue,
                            Required = true,
                            // FieldOptions needs to be defined or fetched appropriately
                            Form = myForm
                        };
                        myForm.FormFields.Add(formField);
                        _context.FormFields.Add(formField);
                    }

                    var formNotes = new FormNotes
                    {
                        FormId = myForm.FormId, // Again, adjust as necessary
                        Notes = new List<string> { "Form submitted" },
                        CreatedDate = DateTime.Now
                    };

                    myForm.FormNotes = new List<FormNotes> { formNotes };
                    _context.Forms.Add(myForm);

                    // Log or additional operations here
                }
               
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Validation failed", errors = errors });
        }

        /*  [HttpPost("dynamic")]
          public async Task<IActionResult> SubmitMultipleDynamicForms([FromBody] List<DynamicFormModel> models)
          {
              if (ModelState.IsValid)
              {
                  foreach (var model in models)
                  {
                      // Directly access properties from the model
                      var formName = model.FormName;
                      var description = model.Description;

                      var myForm = new Form
                      {
                          FormName = formName,
                          Description = description,
                          CreatedDate = DateTime.Now,
                          FormFields = new List<FormField>()
                      };

                      foreach (var field in model.Fields)
                      {
                          var formField = new FormField
                          {
                              FormId = myForm.FormId, // This will likely need adjustment since FormId might not be set until the form is saved
                              FieldName = field.FieldName,
                              FieldType = field.FieldType.ToString(),
                              FieldValue = field.FieldValue,
                              Required = true,
                              // FieldOptions needs to be defined or fetched appropriately
                              Form = myForm
                          };
                          myForm.FormFields.Add(formField);
                          _context.FormFields.Add(formField);
                      }

                      var formNotes = new FormNotes
                      {
                          FormId = myForm.FormId, // Again, adjust as necessary
                          Notes = new List<string> { "Form submitted" },
                          CreatedDate = DateTime.Now
                      };

                      myForm.FormNotes = new List<FormNotes> { formNotes };
                      _context.Forms.Add(myForm);

                      // Log or additional operations here
                  }

                  await _context.SaveChangesAsync();
                  return Json(new { success = true, message = $"{models.Count} forms submitted successfully" });
              }

              // Handle validation errors
              var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
              return Json(new { success = false, message = "Validation failed", errors = errors });
          }*/

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

                    var formNotes = new FormNotes { FormId = myForm.FormId, Notes =new List<string> { "Form submitted" }, CreatedDate = DateTime.Now };
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

        /// <summary>
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
        }

        // Add a method to update the form
        /// <summary>
        ///  
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("UpdateForm")]
        public async Task<IActionResult> UpdateForm([FromForm] DynamicFormModel model)
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

                    var formNotes = new FormNotes { FormId = myForm.FormId, Notes = new List<string> { "Form submitted" }, CreatedDate = DateTime.Now };
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
                    selectedData.Add(new Dictionary<string, string>
                    {
                        { "FormName", form.FormName },
                        { "Description", form.Description },
                        { "Fields", JsonConvert.SerializeObject(formFields) }
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

    }

}
