namespace MyFreeFormForm.Models
{
    public class FormFieldModel    
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; } // Could be enum or string representation of types
                                              // Additional properties as needed (e.g., IsRequired, MaxLength)
    }

    public class DynamicFormModel
    {
        public List<FormFieldModel> Fields { get; set; } = new List<FormFieldModel>();
    }


}
