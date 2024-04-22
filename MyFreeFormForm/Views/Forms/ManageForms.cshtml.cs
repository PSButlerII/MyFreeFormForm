using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyFreeFormForm.Models;
using System.Collections.Generic;

namespace MyFreeFormForm.Views.Forms
{
    public class ManageModel : PageModel
    {
        public Dictionary<string, List<Form>> GroupedForms { get; set; } = new Dictionary<string, List<Form>>();

        public void OnGet()
        {
        }
    }
}
