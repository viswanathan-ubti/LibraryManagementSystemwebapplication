using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Mvc.Controllers
{
    public class SubscriberController : Controller
    {
        private readonly UserService _userService;
        private readonly BookService _bookService;
        private readonly BorrowService _borrowService;

        public SubscriberController(UserService userService, BookService bookService, BorrowService borrowService)
        {
            _userService = userService;
            _bookService = bookService;
            _borrowService = borrowService;
        }

        // GET: /Subscriber/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Subscriber/Login
        [HttpPost]
        public async Task<IActionResult> Login(string libraryCode)
        {
            try
            {
                var (success, message) = await _userService.ValidateUser(libraryCode);
                if (success)
                {
                    HttpContext.Session.SetString("LibraryCode", libraryCode);
                    return RedirectToAction("Index");
                }
                ViewBag.Error = message;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return View();
            }
        }

        // GET: /Subscriber/Index (Subscriber Dashboard)
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("LibraryCode")))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var books = await _bookService.GetAllBooks();
                return View(books);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading books: {ex.Message}";
                return View(new List<Book>());
            }
        }

        // GET: /Subscriber/BorrowBook
        public IActionResult BorrowBook()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("LibraryCode")))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // POST: /Subscriber/BorrowBook
        [HttpPost]
        public async Task<IActionResult> BorrowBook(string bookCode)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("LibraryCode")))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var libraryCode = HttpContext.Session.GetString("LibraryCode");
                var (success, message) = await _borrowService.BorrowBook(libraryCode, bookCode);
                if (success)
                {
                    TempData["Message"] = message;
                    return RedirectToAction("Index");
                }
                ViewBag.Error = message;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return View();
            }
        }

        // GET: /Subscriber/ReturnBook
        public IActionResult ReturnBook()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("LibraryCode")))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // POST: /Subscriber/ReturnBook
        [HttpPost]
        public async Task<IActionResult> ReturnBook(string bookCode)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("LibraryCode")))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var libraryCode = HttpContext.Session.GetString("LibraryCode");
                var (success, message) = await _borrowService.ReturnBook(libraryCode, bookCode);
                TempData["Message"] = message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return View();
            }
        }

        // GET: /Subscriber/SubscriptionDetails
        public async Task<IActionResult> SubscriptionDetails()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("LibraryCode")))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var libraryCode = HttpContext.Session.GetString("LibraryCode");
                var user = await _userService.GetUserDetails(libraryCode);
                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading subscription details: {ex.Message}";
                return View();
            }
        }

        // POST: /Subscriber/RenewSubscription
        [HttpPost]
        public async Task<IActionResult> RenewSubscription()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("LibraryCode")))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var libraryCode = HttpContext.Session.GetString("LibraryCode");
                var (success, message) = await _userService.RenewSubscription(libraryCode);
                TempData["Message"] = message;
                return RedirectToAction("SubscriptionDetails");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error renewing subscription: {ex.Message}";
                return RedirectToAction("SubscriptionDetails");
            }
        }

        // GET: /Subscriber/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}