using System.ComponentModel.DataAnnotations;

namespace MyFreeFormForm.Models
{
    public class Form
    {
        [Key]
        public int FormId { get; set; }
        public string FormName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<FormField> FormFields { get; set; }
        public ICollection<FormNotes> FormNotes { get; set; }
    }

}
