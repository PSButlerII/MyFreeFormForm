using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyFreeFormForm.Data;
using MyFreeFormForm.Models;
using Newtonsoft.Json;

namespace MyFreeFormForm.Services
{
    public class FormsDbc
    {
        private ApplicationDbContext _context;
        private readonly ILogger<FormsDbc> _logger;

        public FormsDbc(ApplicationDbContext context, ILogger<FormsDbc> logger )
        {
            _context = context;
            _logger = logger;
        }

        public Form? GetForm(int formId)
        {
            try
            {
                return _context.Forms
                    .Include(f => f.FormFields)
                    .Include(f => f.FormNotes)
                    .FirstOrDefault(f => f.FormId == formId);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error getting form {formId}", formId);

                return null;
            }
        }

        public List<Form> GetForms()
        {
            try
            {
                return _context.Forms
                    .Include(f => f.FormFields)
                    .Include(f => f.FormNotes)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error getting forms");

                return new List<Form>();
            }
        }

        public Form? CreateForm(DynamicFormModel formModel)
        {
            try
            {
                var form = new Form
                {
                    FormName = formModel.FormName,
                    Description = formModel.Description,
                    CreatedDate = DateTime.Now,
                    FormFields = new List<FormField>(),
                    FormNotes = new List<FormNotes>()
                };

                foreach (var field in formModel.Fields)
                {
                    form.FormFields.Add(new FormField
                    {
                        FieldName = field.FieldName,
                        FieldType = field.FieldType.ToString(),
                        FieldValue = field.FieldValue
                    });
                }

                _context.Forms.Add(form);
                _context.SaveChanges();

                return form;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error creating form");

                return null;
            }
        }

        public bool UpdateForm(int formId, DynamicFormModel formModel)
        {
            try
            {
                var form = _context.Forms
                    .Include(f => f.FormFields)
                    .Include(f => f.FormNotes)
                    .FirstOrDefault(f => f.FormId == formId);

                if (form == null)
                {
                    return false;
                }

                form.FormName = formModel.FormName;
                form.Description = formModel.Description;
                form.UpdatedDate = DateTime.Now;

                // Remove all existing fields
                _context.FormFields.RemoveRange(form.FormFields);

                // Add new fields
                foreach (var field in formModel.Fields)
                {
                    form.FormFields.Add(new FormField
                    {
                        FieldName = field.FieldName,
                        FieldType = field.FieldType.ToString(),
                        FieldValue = field.FieldValue
                    });
                }

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error updating form {formId}", formId);

                return false;
            }
        }

        public bool DeleteForm(int formId)
        {
            try
            {
                var form = _context.Forms
                    .Include(f => f.FormFields)
                    .Include(f => f.FormNotes)
                    .FirstOrDefault(f => f.FormId == formId);

                if (form == null)
                {
                    return false;
                }

                _context.Forms.Remove(form);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error deleting form {formId}", formId);

                return false;
            }
        }

        public bool AddFormNotes(int formId, List<string> notes)
        {
            try
            {
                var form = _context.Forms
                    .Include(f => f.FormNotes)
                    .FirstOrDefault(f => f.FormId == formId);

                if (form == null)
                {
                    return false;
                }

                form.FormNotes.Add(new FormNotes
                {
                    Notes = notes,
                    CreatedDate = DateTime.Now
                });

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error adding notes to form {formId}", formId);

                return false;
            }
        }

        public bool DeleteFormNotes(int formId, int notesId)
        {
            try
            {
                var form = _context.Forms
                    .Include(f => f.FormNotes)
                    .FirstOrDefault(f => f.FormId == formId);

                if (form == null)
                {
                    return false;
                }

                var notes = form.FormNotes.FirstOrDefault(n => n.NoteId == notesId);

                if (notes == null)
                {
                    return false;
                }

                form.FormNotes.Remove(notes);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error deleting notes {notesId} from form {formId}", notesId, formId);

                return false;
            }
        }

        public bool UpdateFormNotes(int formId, int notesId, List<string> notes)
        {
            try
            {
                var form = _context.Forms
                    .Include(f => f.FormNotes)
                    .FirstOrDefault(f => f.FormId == formId);

                if (form == null)
                {
                    return false;
                }

                var note = form.FormNotes.FirstOrDefault(n => n.NoteId == notesId);

                if (note == null)
                {
                    return false;
                }

                note.Notes = notes;
                note.UpdatedDate = DateTime.Now;

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error updating notes {notesId} for form {formId}", notesId, formId);

                return false;
            }
        }

        public async Task EnqueueFormSubmissionAsync(DynamicFormModel model)
        {
            var serializedModel = JsonConvert.SerializeObject(model);

            _context.FormSubmissionQueue.Add(new Models.FormSubmissionQueue
            {
                FormModelData = serializedModel,
                IsProcessed = false,
                CreatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();

        }
    }
}
