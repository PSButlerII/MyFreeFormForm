using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MyFreeFormForm.Models;
using MyFreeFormForm.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MyFreeFormForm.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace MyFreeFormForm.Controllers
{
    [ApiController]
    [Route("search")]
    public class FormsSearchController : Controller
    {
        private readonly ILogger<FormsSearchController> _logger;
        private readonly SearchService _searchService;
        private readonly ApplicationDbContext _context;

        public FormsSearchController(ILogger<FormsSearchController> logger, SearchService searchService, ApplicationDbContext context)
        {
            _logger = logger;
            _searchService = searchService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View("SearchResults");
        }

        [HttpGet("GetFieldNames")]
        public async Task<IActionResult> GetFieldNames(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            // Fetch field names only for forms owned by the given user
            // Since FormFields does not contain a circular reference to Form, we must join on FormId and UserId.
            // TODO: May need to add the formname to the fieldnames query so that it is an option to search on.
            var fieldNames = await _context.FormFields
                .Where(ff => _context.Forms.Any(f => f.FormId == ff.FormId && f.UserId == userId))
                .Select(ff => ff.FieldName)
                .Distinct()
                .ToListAsync();

            return Ok(fieldNames);
        }

        [HttpGet("GetDateFields")]
        public async Task<IActionResult> GetDateFields(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            List<string> formDateFields = new List<string> { "CreatedDate", "UpdatedDate" };

            // Get distinct date field names from the FormFields table
            var formFieldDateNames = await _context.FormFields
                .Where(ff => _context.Forms.Any(f => f.FormId == ff.FormId && f.UserId == userId) && ff.FieldType == Helpers.FieldType.Date.ToString())
                .Select(ff => ff.FieldName)
                .Distinct()
                .ToListAsync();

            // Combine the lists, eliminating any duplicates that may arise
            var allDateFields = formDateFields.Concat(formFieldDateNames)
                                               .Distinct()
                                               .ToList();

            return Ok(allDateFields);
        }

        [HttpGet("GetFormNames")]
        public async Task<IActionResult> GetFormNames(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            // Fetch form names only for forms owned by the given user.
            var formNames = await _context.Forms
                .Where(f => f.UserId == userId)
                .Select(f => f.FormName)
                .Distinct()
                .ToListAsync();

            return Ok(formNames);
        }

        [HttpGet("SearchFormsAsync")]
        //TODO: Need to add the form name to the search criteria
        public async Task<IActionResult> SearchFormsAsync(string userId, string? searchTerm, DateTime? startDate, DateTime? endDate, string? fieldName, string? minValue, string? maxValue, string? dateField, string? formName)
        {
            try
            {
                var criteria = new List<SearchCriteria>();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    criteria.Add(new SearchCriteria { FieldName = fieldName, FieldType = Helpers.FieldType.Text, FieldValue = searchTerm });
                }

                if (!string.IsNullOrEmpty(formName))
                {
                    criteria.Add(new SearchCriteria { FormName = formName, FieldType = Helpers.FieldType.None });
                }

                if (startDate.HasValue && endDate.HasValue)
                {
                    // Assume we have a predefined way to handle date range in SearchCriteria
                    criteria.Add(new SearchCriteria { FieldName = fieldName, DateField = dateField, FieldType = Helpers.FieldType.Date, FieldValue = $"{startDate.Value.ToShortDateString()} to {endDate.Value.ToShortDateString()}",StartDate=startDate, EndDate = endDate }); 
                }

                else if (startDate.HasValue)
                {
                    // Assume we have a predefined way to handle date range in SearchCriteria
                    criteria.Add(new SearchCriteria { FieldName = fieldName, DateField = dateField, FieldType = Helpers.FieldType.Date, FieldValue = $"{startDate.Value.ToShortDateString()}", StartDate=startDate });
                }
                // Similar for fieldName, minValue, and maxValue
                if (fieldName != null)
                {
                    criteria.Add(new SearchCriteria { FieldName = fieldName, FieldType = Helpers.FieldType.Text, FieldValue = minValue, AdditionalValue = maxValue });
                }

                if (minValue != null && maxValue != null)
                {
                    criteria.Add(new SearchCriteria { FieldName = fieldName, FieldType = Helpers.FieldType.Number, FieldValue = minValue, AdditionalValue = maxValue });
                }
                // Now call your search service with these criteria
                var forms = await _searchService.SearchFormsAsync(userId, criteria);

                // return a json response with the forms
                return Json(new { success = true, message = "File processed successfully", data= forms });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching forms.");
                return StatusCode(500, "Error searching forms.");
            }
        }
        // Add additional methods as needed for your application.
    }
}
