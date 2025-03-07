using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Mvc.Controllers
{
    public class HomeController : Controller
    {
        // Landing Page
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult Todo()
        {
            return View();
        }
    }
}