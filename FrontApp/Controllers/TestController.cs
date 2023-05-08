using FrontApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrontApp.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private string Test()
        {
            Article article = new()
            {
                Title = "Back to the future",
                Published = DateTime.Now.AddYears(-40),
                Subtitle = "BTF",
                Author = "Marty"
            };

            return "";
        }
    }
}
