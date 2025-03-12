using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LibraryManagementSystem.Mvc.Controllers
{
    public class SubscriberController : Controller
    {
        private readonly UserService _userService;
        private readonly BookService _bookService;
        private readonly BorrowService _borrowService;
        private readonly JwtService _jwtService;

        public SubscriberController(UserService userService, BookService bookService, BorrowService borrowService, JwtService jwtService)
        {
            _userService = userService;
            _bookService = bookService;
            _borrowService = borrowService;
            _jwtService = jwtService;
        }

        // GET: /Subscriber/Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Subscriber/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string libraryCode)
        {
            try
            {
                var (success, message) = await _userService.ValidateUser(libraryCode);
                if (success)
                {
                    var user = await _userService.GetUserDetails(libraryCode);
                    var token = _jwtService.GenerateToken(user);
                    HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
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
        [Authorize(Roles = "Subscriber")]
        public async Task<IActionResult> Index()
        {
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
        [Authorize(Roles = "Subscriber")]
        public IActionResult BorrowBook()
        {
            return View();
        }

        // POST: /Subscriber/BorrowBook
        [HttpPost]
        [Authorize(Roles = "Subscriber")]
        public async Task<IActionResult> BorrowBook(string bookCode)
        {
            try
            {
                var libraryCode = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
        [Authorize(Roles = "Subscriber")]
        public IActionResult ReturnBook()
        {
            return View();
        }

        // POST: /Subscriber/ReturnBook
        [HttpPost]
        [Authorize(Roles = "Subscriber")]
        public async Task<IActionResult> ReturnBook(string bookCode)
        {
            try
            {
                var libraryCode = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
        [Authorize(Roles = "Subscriber")]
        public async Task<IActionResult> SubscriptionDetails()
        {
            try
            {
                var libraryCode = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
        [Authorize(Roles = "Subscriber")]
        public async Task<IActionResult> RenewSubscription()
        {
            try
            {
                var libraryCode = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
        [Authorize(Roles = "Subscriber")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Home");
        }
    }
}