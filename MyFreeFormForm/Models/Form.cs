using MyFreeFormForm.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFreeFormForm.Models
{
    public class Form
    {
        [Key]
        public int FormId { get; set; }

        [ForeignKey("AspNetUsers")]
        public string UserId { get; set; }

        public string FormName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ICollection<FormField> FormFields { get; set; }
        public ICollection<FormNotes> FormNotes { get; set; }


        // Consider adding a navigation property to the User entity if applicable.
        // This depends on your User model and if you're using Entity Framework for ORM.
        public virtual MyIdentityUsers User { get; set; }

    }

}
