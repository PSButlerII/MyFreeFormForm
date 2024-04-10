using MyFreeFormForm.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MyFreeFormForm.Models
{
    public class FormField
    {
        [Key]
        public int FieldId { get; set; }
        public int FormId { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        //added two date time fields
        public DateTime? FieldDateValue { get; set; }
        public DateTimeOffset? FieldDateTimeOffsetValue { get; set; }
        public FieldOptions FieldOptions { get; set; } // JSON or delimited list for options
        public string FieldValue { get; set; }
        public bool Required { get; set; }

        //public Form Form { get; set; }
    }
}
