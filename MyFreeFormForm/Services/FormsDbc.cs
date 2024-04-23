using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
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

        public async Task<Form?> GetFormByIdAsync(int formId)
        {
            try
            {
                return await _context.Forms
                    .Include(f => f.FormFields)
                    .Include(f => f.FormNotes)
                    .FirstOrDefaultAsync(f => f.FormId == formId);
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

        public List<Form> GetFormsByUser(string userId)
        {
            try
            {
                return _context.Forms
                    .Include(f => f.FormFields)
                    .Include(f => f.FormNotes)
                    .Where(f => f.UserId == userId)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error getting forms");

                return new List<Form>();
            }
        }

        public async Task<List<FormNotes>> GetNotes(int formid)
        {
            try
            {
                return await _context.FormNotes
                    .Where(n => n.FormId == formid)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error getting notes for form {formid}", formid);

                return new List<FormNotes>();
            }
        }

        public Task<List<string>> GetFieldNames(string userId)
        {
            try
            {
                return _context.FormFields
                .Where(ff => _context.Forms.Any(f => f.FormId == ff.FormId && f.UserId == userId))
                .Select(ff => ff.FieldName)
                .Distinct()
                .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error getting forms");

                return null;
            }
        }

        public Task<List<string>> GetDateFields(string userId)
        {
            try
            {
                return _context.FormFields
                .Where(ff => _context.Forms.Any(f => f.FormId == ff.FormId && f.UserId == userId) && ff.FieldType == Helpers.FieldType.Date.ToString())
                .Select(ff => ff.FieldName)
                .Distinct()
                .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error getting forms");

                return null;
            }
        }

        public Task<List<string>> GetFormNames(string userId)
        {
            try
            {
                return _context.Forms
                .Where(f => f.UserId == userId)
                .Select(f => f.FormName)
                .Distinct()
                .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error getting forms");

                return null;
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

        public bool DeleteStaticForm(int id)
        {
            try
            {
                var form = _context.Forms
                    .Include(f => f.FormFields)
                    .Include(f => f.FormNotes)
                    .FirstOrDefault(f => f.FormId == id);

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
                _logger.LogError(ex, "Error deleting form {formId}", id);

                return false;
            }
        }

        public async Task<bool> DeleteFormAsync(int id)
        {
            try
            {
                var form = await _context.Forms
                    .Include(f => f.FormFields)
                    .Include(f => f.FormNotes)
                    .FirstOrDefaultAsync(f => f.FormId == id);

                if (form == null)
                {
                    return false;
                }

                _context.Forms.Remove(form);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error deleting form {formId}", id);

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
                    Note = notes,
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

                note.Note = notes;
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




        // Section for Statistics
        // Get form counts by user
        public async Task<Dictionary<string, int>> GetFormCountsByUser(string userId)
        {
            return await _context.Forms
                .Where(f => f.UserId == userId)
                .GroupBy(f => f.UserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.UserId, g => g.Count);
        }

        // Get field usage statistics
        public async Task<Dictionary<string, int>> GetFieldUsageStats(string userId)
        {
            //need to add userId
            return await _context.FormFields
                .Where(predicate => _context.Forms.Any(f => f.FormId == predicate.FormId && f.UserId == userId))
                .GroupBy(ff => ff.FieldName)
                .Select(g => new { FieldName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.FieldName, g => g.Count);
        }

        // Get average fields per form
        public async Task<double> GetAverageFieldsPerForm(string userId)
        {
            //need to add userId
            return await _context.Forms
                .Where(f => f.UserId == userId)
                .Select(f => f.FormFields.Count)
                .AverageAsync();
        }

        // Get form submissions over time
        public async Task<List<KeyValuePair<DateTime, int>>> GetFormSubmissionsOverTime(TimeGrouping grouping, string userId)
        {
            //need to add userId
            var query = _context.Forms.AsQueryable().Where(f=>f.UserId==userId);

            switch (grouping)
            {
                case TimeGrouping.Daily:
                    query = (IQueryable<Form>)query.GroupBy(f => f.CreatedDate.Date);
                    break;
                case TimeGrouping.Weekly:
                    query = (IQueryable<Form>)query.GroupBy(f => EF.Functions.DateDiffDay(DateTime.MinValue, f.CreatedDate) / 7);
                    break;
                case TimeGrouping.Monthly:
                    query = (IQueryable<Form>)query.GroupBy(f => new { f.CreatedDate.Year, f.CreatedDate.Month });
                    break;
            }

            return await query
                .Select(g => new KeyValuePair<DateTime, int>(g.CreatedDate, g.FormName.Count())) // may need to adjust this
                .ToListAsync();
        }

        // Add an enum for time grouping if needed
        public enum TimeGrouping { Daily, Weekly, Monthly }
        // Count entries by date field for trend analysis
        public async Task<List<KeyValuePair<DateTime, int>>> CountEntriesByDateField(string fieldName, DateTime startDate, DateTime endDate, string userId)
        {
            return await _context.FormFields
                .Where(ff => ff.FieldName == fieldName && ff.FieldDateValue >= startDate && ff.FieldDateValue <= endDate && _context.Forms.Any(f => f.FormId == ff.FormId && f.UserId == userId))
                .GroupBy(ff => ff.FieldDateValue.Value.Date)
                .Select(g => new KeyValuePair<DateTime, int>(g.Key, g.Count()))
                .ToListAsync();
        }

        // Count specific field values
        public async Task<int> CountSpecificFieldValue(string fieldName, string userId)
        {
            return await _context.FormFields
                .Where(ff => ff.FieldName == fieldName && _context.Forms.Any(f => f.FormId == ff.FormId && f.UserId == userId))
                .CountAsync();
        }

        // Get entries that are expiring or have an upcoming date in a range
        public async Task<List<Form>> GetExpiringEntries(string dateField, DateTime startDate, DateTime endDate, string userId)
        {
            return await _context.Forms
                .Include(f => f.FormFields)
                .Where(f => f.UserId == userId && f.FormFields.Any(ff => ff.FieldName == dateField && ff.FieldDateValue >= startDate && ff.FieldDateValue <= endDate))
                .ToListAsync();
        }

        public async Task<List<Form>> CountFieldValueOccurrences(string fieldName, DateTime? startDate, DateTime? endDate, string userId)
        {
            return await _context.Forms
                .Include(f => f.FormFields)
                .Where(f => f.UserId == userId && f.FormFields.Any(ff => ff.FieldName == fieldName && ff.FieldDateValue >= startDate && ff.FieldDateValue <= endDate))
                .ToListAsync();
        }
    }
}
