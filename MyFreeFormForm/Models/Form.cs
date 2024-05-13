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
        //TODO: Since every form has it own formId, we should add a parent formId to the form model. you will need to add a new column to the database table for this, and update the database schema. You will also need to update the form model and the submit form page to include this new field.
        //public string ParentFormId { get; set; }

        // Consider adding a navigation property to the User entity if applicable.
        // This depends on your User model and if you're using Entity Framework for ORM.
        public virtual MyIdentityUsers User { get; set; }

    }

}
