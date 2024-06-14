using MyFreeFormForm.Helpers;

namespace MyFreeFormForm.Models
{
    public class DynamicField
    {
        public string FieldName { get; set; }
        public FieldType FieldType { get; set; } // Simple text representation for simplicity
        public string FieldValue { get; set; }
    }

    public class DynamicFormModel
    {
       // FormName and Description properties in the Form model
       public string FormName { get; set; }
       public string Description { get; set; }
       public List<DynamicField> Fields { get; set; } = new List<DynamicField>();
        //Maybe I need to add the FormNotes property here.  This way I can pass the notes to the FormNotes model
        public List<FormNotes> FormNotes { get; set; } = new List<FormNotes>();
        public string UserId { get; set; }
        public string ParentFormId { get; set; }
        internal bool Validate(out List<string> errors)
        {
            errors = new List<string>();
            if (string.IsNullOrWhiteSpace(FormName))
            {
                errors.Add("Form Name is required.");
            }
            if (string.IsNullOrWhiteSpace(Description))
            {
                errors.Add("Description is required.");
            }
            if (Fields == null || Fields.Count == 0)
            {
                errors.Add("At least one field is required.");
            }
            return errors.Count == 0;
        }
        //TODO: Need to be able to attach files to the form
        //public List<IFormFile> Files { get; set; } = new List<IFormFile>();
        //public Form FormInstance { get; set; }
    }
}