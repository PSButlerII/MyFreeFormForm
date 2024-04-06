using Microsoft.EntityFrameworkCore;
using MyFreeFormForm.Data;
using MyFreeFormForm.Helpers;
using MyFreeFormForm.Models;

namespace MyFreeFormForm.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Form>> SearchFormsAsync(string userId, List<SearchCriteria> criteria)
        {
            IQueryable<Form> query = _context.Forms
                .Include(f => f.FormFields) // Ensure related FormFields are included
                .Where(f => f.UserId == userId);

            foreach (var criterion in criteria)
            {
                query = ApplyCriterion(query, criterion);
                // Add more cases as necessary for your application
            }

            return await query.ToListAsync();
        }


        private IQueryable<Form> ApplyCriterion(IQueryable<Form> query, SearchCriteria criterion)
        {
            if (criterion.StartDate.HasValue && string.IsNullOrEmpty(criterion.DateField))
            {
                if (criterion.DateField.ToLower() == "CreatedDate".ToLower())
                {
                    query = query.Where(f => f.CreatedDate >= criterion.StartDate.Value);
                }
              
            }

            if (criterion.EndDate.HasValue && string.IsNullOrEmpty(criterion.DateField))
            {
                if (criterion.DateField.ToLower() == "CreatedDate".ToLower())
                {
                    // Adjust endDate to ensure it includes the entire day
                    DateTime adjustedEndDate = criterion.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(f => f.CreatedDate <= adjustedEndDate);
                }
                else
                {
                    DateTime adjustedEndDate = criterion.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(f => f.FormFields.Any(ff => ff.FieldName == criterion.FieldName &&
                                        EF.Functions.Like(ff.FieldValue, $"%{criterion.FieldValue}%")));
                }
            }

            if (criterion.FieldType == FieldType.Text)
            {
                // This handles text fields; extend with additional conditions as needed
                query = query.Where(f => f.FormFields.Any(ff =>
                    ff.FieldName == criterion.FieldName &&
                    EF.Functions.Like(ff.FieldValue, $"%{criterion.FieldValue}%")));
            }

            else if (criterion.FieldType == FieldType.Number)
            {
                // Attempt to parse the criterion value outside of the query.
                if (int.TryParse(criterion.FieldValue, out int intValue))
                {
                    query = query.Where(f => f.FormFields.Any(ff =>
                        ff.FieldName == criterion.FieldName &&
                        ff.FieldValue == criterion.FieldValue));
                }
            }

            else if (criterion.FieldType == FieldType.Date)
            {
                // Direct comparison using a known format, e.g., "yyyyMMdd" for dates.
                query = query.Where(f => f.FormFields.Any(ff =>
                    ff.FieldName == criterion.FieldName &&
                    ff.FieldValue == DateTime.Parse(criterion.FieldValue).ToString("yyyyMMdd"))); // Ensure format matches your data
            }

            // Add more cases as necessary for your application
            return query;
        }
    }
}
