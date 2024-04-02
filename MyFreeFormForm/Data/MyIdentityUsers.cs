using Microsoft.AspNetCore.Identity;
using MyFreeFormForm.Models;

namespace MyFreeFormForm.Data
{
    public class MyIdentityUsers : IdentityUser
    {
        public string FirstName { get; set; }


        public string LastName { get; set; }

        //[PersonalData]
        //[Column(TypeName = "nvarchar(100)")]
        //public string UserName { get; set; }

        public string City { get; set; }


        public string State { get; set; }


        public string Zip { get; set; }

        //[PersonalData]
        //[Column(TypeName = "nvarchar(100)")]
        //public string Phone { get; set; }
        // Computed Full Name property
        public string FullName => $"{FirstName} {LastName}";
        public virtual ICollection<Form> Forms { get; set; }
    }
}
