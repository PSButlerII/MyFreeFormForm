using MyFreeFormForm.Helpers;

namespace MyFreeFormForm.Models
{
    public class DynamicField
    {
        public string FieldName { get; set; }
        public FieldType FieldType { get; set; } // Simple text representation for simplicity
    }

    public class DynamicFormModel
    {
        public List<DynamicField> Fields { get; set; } = new List<DynamicField>();
    }

}
