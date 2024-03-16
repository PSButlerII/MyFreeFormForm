using Microsoft.EntityFrameworkCore;
using MyFreeFormForm.Data;
using MyFreeFormForm.Models;

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
    }
}
