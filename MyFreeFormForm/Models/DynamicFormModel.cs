using MyFreeFormForm.Helpers;


namespace MyFreeFormForm.Models
{
    public class DynamicField
    {
        public string FieldName { get; set; }
        public FieldType FieldType { get; set; } // Simple text representation for simplicity
        public Object FieldValue { get; set; }
    }

    public class DynamicFormModel
    {
       // FormName and Description properties in the Form model
       public string FormName { get; set; }
       public string Description { get; set; }

        public List<DynamicField> Fields { get; set; } = new List<DynamicField>();
    }

}
