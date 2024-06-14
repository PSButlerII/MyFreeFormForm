using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFreeFormForm.Controllers;
using MyFreeFormForm.Data;
using MyFreeFormForm.Helpers;
using MyFreeFormForm.Models;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MyFreeFormForm.Services
{
    public interface IFormProcessorService
    {
        Task ProcessFormAsync(string serializedFormData);
    }

    public class FormProcessorService : IFormProcessorService
    {
        private string parentId = new Guid().ToString();
        private readonly ApplicationDbContext _context;
        public FieldOptions fieldOptions;
        private readonly ILogger<FormProcessorService> ILogger = new Logger<FormProcessorService>(new LoggerFactory());
        // Inject any other dependencies you need for processing

        public FormProcessorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProcessesFormAsync(DynamicFormModel model)
        {
            if (model == null) return false;
            try
            {
                var formName = model.FormName;
                var description = model.Description;
                var userId = model.UserId;
                var myForm = new Form
                {
                    FormName = formName,
                    Description = description,
                    CreatedDate = DateTime.Now,
                    FormFields = new List<FormField>(), // Initialize the list here
                    FormNotes = new List<FormNotes>(), // Initialize the list here
                    UserId = userId,
                    ParentFormId = new Guid().ToString()
                };

                foreach (var field in model.Fields)
                {
                    var formField = new FormField
                    {
                        FieldName = field.FieldName,
                        FieldType = field.FieldType.ToString(),
                        FieldValue = field.FieldValue,
                        Required = true,
                        FieldOptions = fieldOptions,
                    };
                    if (field.FieldType == FieldType.Date)
                    {
                        try
                        {
                            // Attempt to parse field.FieldValue as a DateTime
                            if (DateTime.TryParse(field.FieldValue, out var dateValue))
                            {
                                formField.FieldDateValue = dateValue;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message, $"Could not parse date: {field.FieldValue} ");
                        }
                    }
                    else if (field.FieldType == FieldType.DateTime)
                    {
                        try
                        {
                            // Attempt to parse field.FieldValue as a DateTimeOffset
                            if (DateTimeOffset.TryParse(field.FieldValue, out var dateTimeOffsetValue))
                            {
                                formField.FieldDateTimeOffsetValue = dateTimeOffsetValue;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message, $"Could not parse date: {field.FieldValue} ");
                        }
                    }
                        myForm.FormFields.Add(formField);
                    _context.FormFields.Add(formField);
                }
                foreach (var note in model.FormNotes)
                {
                    var formNotes = new FormNotes
                    {
                        Notes = note.Notes,
                        CreatedDate = DateTime.Now,
                        Form = myForm // This sets the relationship
                    };
                    myForm.FormNotes.Add(formNotes);
                    _context.FormNotes.Add(formNotes);
                }
                // Now add the form (with fields and notes) to the context and save it once
                _context.Forms.Add(myForm);
                await _context.SaveChangesAsync();
                return true;
            }
            // catch if the form
            catch (Exception ex)
            {
                ILogger.LogError(ex, "Error processing form data");
                return false;
            }
        }

        Task IFormProcessorService.ProcessFormAsync(string serializedFormData)
        {
            var form = JsonConvert.DeserializeObject<FormCollection>(serializedFormData);
            if (form == null)
            {
                // Log an error or handle the case where the form data couldn't be deserialized
                return Task.CompletedTask;
            }
            var model = JsonConvert.DeserializeObject<DynamicFormModel>(serializedFormData);
            return ProcessesFormAsync(model);

        }     
    }
}
