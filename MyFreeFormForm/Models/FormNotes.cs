using System.ComponentModel.DataAnnotations;

namespace MyFreeFormForm.Models
{
    public class FormNotes
    {
        [Key]
        public int NoteId { get; set; }
        public int FormId { get; set; }
        public List<string> Note { get; set; }
        public DateTime CreatedDate { get; set; }

        public Form Form { get; set; }
        public DateTime UpdatedDate { get; internal set; }
    }
}
