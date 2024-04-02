using Microsoft.AspNetCore.Mvc.Rendering;
using MyFreeFormForm.Data;

namespace MyFreeFormForm.Core.ViewModels
{
    public class EditUserViewModel
    {
        public MyIdentityUsers User { get; set; }

        public IList<SelectListItem> Roles { get; set; }
    }
}
