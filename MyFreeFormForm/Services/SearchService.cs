using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
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

            string sqlQuery = query.ToQueryString();
            Console.WriteLine("Console Logging the Query here: ",sqlQuery); // Or use any logging mechanism

            var result = await query.ToListAsync();
            // Now, since we have the results, can we do something with them? Like query the form fields? for the correct dates information since they are stored as strings?
            // We can do that here, or in a separate method, depending on the complexity of the query.
            // For example, we can query the form fields for the correct date information here, or in a separate method.
            // So, I would just need to parse the formfield value, that has a field type of date, to a DateTime object. and compare it to the date information in the criteria. Is that possible?
            


            return result;
        }

        private IQueryable<Form> ApplyCriterion(IQueryable<Form> query, SearchCriteria criterion)
        {
            if (!string.IsNullOrEmpty(criterion.DateField))
            {
                DateTime startDate = criterion.StartDate ?? DateTime.MinValue;
                DateTime endDate = criterion.EndDate?.AddDays(1).AddTicks(-1) ?? DateTime.MaxValue;
                if (criterion.DateField.Equals("CreatedDate", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(f => f.CreatedDate >= startDate && f.CreatedDate <= endDate);
                }
                // Assuming "UpdatedDate" is also a direct property of the Form
                else if (criterion.DateField.Equals("UpdatedDate", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(f => f.UpdatedDate.HasValue && f.UpdatedDate >= startDate && f.UpdatedDate <= endDate);
                }
                // If criterion.DateField is not UpdatedDate or CreatedDate, it must be a field in FormFields
                else
                {
                    if (!string.IsNullOrEmpty(criterion.DateField) && criterion.DateField != "CreatedDate" && criterion.DateField != "UpdatedDate")
                    {
                        //Now I've added a FieldDateValue for dates, and a FieldDateTimeOffsetValue for DateTime to the FormFields table, so I can query the date information from there. Both will need to be searched.
                        query = query.Where(f => f.FormFields.Any(ff => ff.FieldName == criterion.DateField && ff.FieldType == FieldType.Date.ToString() && ff.FieldDateValue >= startDate && ff.FieldDateValue <= endDate));
                    };
                    /*query = query.Where(f => f.FormFields.Any(ff => ff.FieldName == criterion.DateField && ff.FieldType == FieldType.DateTime.ToString() && ff.FieldDateTimeOffsetValue >= startDate && ff.FieldDateTimeOffsetValue <= endDate));*/
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
            return query;
        }
    }
}
