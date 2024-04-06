using MyFreeFormForm.Helpers;

namespace MyFreeFormForm.Models
{
    public class SearchCriteria
    {
        public string FieldName { get; set; }
        public FieldType FieldType { get; set; }
        public string Condition { get; set; }
        public string FieldValue { get; set; }
        public string AdditionalValue { get; set; } // For range queries, etc.
        // I will need to handle a date range, so I will add a property for that.
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DateField { get; set; }

    }
}
