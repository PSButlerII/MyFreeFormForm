using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;

namespace MyFreeFormForm.Models
{
    public class FormSubmissionQueue
    {
        [Key]
        public int Id { get; set; }
        public string FormModelData { get; set; }
        public bool IsProcessed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
