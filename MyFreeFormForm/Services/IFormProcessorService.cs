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
                var myForm = new Form
                {
                    FormName = formName,
                    Description = description,
                    CreatedDate = DateTime.Now,
                    FormFields = new List<FormField>(), // Initialize the list here
                    FormNotes = new List<FormNotes>() // Initialize the list here
                };

                foreach (var field in model.Fields)
                {
                    var formField = new FormField
                    {
                        FieldName = field.FieldName,
                        FieldType = field.FieldType.ToString(),
                        FieldValue = field.FieldValue,
                        Required = true,
                        // Assuming fieldOptions is correctly set before this point or within the loop
                        FieldOptions = fieldOptions,
                        Form = myForm // This sets the relationship
                    };
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
