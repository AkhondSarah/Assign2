using Microsoft.AspNetCore.Mvc;

namespace Lab4N.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View("Views/shared/Error.cshtml"); // Do you need to modify this line? Use your judgement based on the app you developed so far        
        }

    }
}
