/*using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFreeFormForm.Helpers;
using MyFreeFormForm.Models;
using System.Text.RegularExpressions;

namespace MyFreeFormForm
{
    public class Old_code
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("dynamic")]
        public async Task<IActionResult> SubmitMultipleDynamicForms([FromBody] List<DynamicFormModel> models)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            //Console.WriteLine("SubmitDynamicForm: " + JsonConvert.SerializeObject(model));

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

        *//*  [HttpPost("dynamic")]
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
          }*//*
    }
}
*/