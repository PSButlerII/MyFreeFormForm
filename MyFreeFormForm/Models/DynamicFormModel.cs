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
       //public List<FormNotes> FormNotes { get; set; } = new List<FormNotes>();

       //public Form FormInstance { get; set; }
    }

}
