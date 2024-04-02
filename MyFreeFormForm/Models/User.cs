using Microsoft.AspNetCore.Identity;

namespace MyFreeFormForm.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public ICollection<Form> FormId { get; set; }
    }
}
