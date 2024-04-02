using Microsoft.AspNetCore.Mvc;
using Markdig;

namespace MyFreeFormForm.Controllers
{
    public class MarkDownController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult MarkdownPage()
        {
            var markdown = System.IO.File.ReadAllText("C:\\Users\\prest\\source\\repos\\MyFreeFormForm\\MyFreeFormForm\\wwwroot\\documents\\helpDoc.md");
            var html = Markdown.ToHtml(markdown);
            return View(model: html);
        }

    }
}
