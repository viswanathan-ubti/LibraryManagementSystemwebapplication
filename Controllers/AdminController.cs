using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        private readonly BookService _bookService;
        private readonly UserService _userService;

        public AdminController(AdminService adminService, BookService bookService, UserService userService)
        {
            _adminService = adminService;
            _bookService = bookService;
            _userService = userService;
        }

        // GET: /Admin/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Admin/Login
        [HttpPost]
        public async Task<IActionResult> Login(string adminCode, string password)
        {
            try
            {
                var (success, message) = await _adminService.ValidateAdmin(adminCode, password);
                if (success)
                {
                    HttpContext.Session.SetString("AdminCode", adminCode);
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

        // GET: /Admin/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Admin/Register
        [HttpPost]
        public async Task<IActionResult> Register(string name, string adminCode, string password)
        {
            try
            {
                var (success, message) = await _adminService.RegisterAdmin(name, adminCode, password);
                if (success)
                {
                    return RedirectToAction("Login");
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

        // GET: /Admin/Index (Admin Dashboard)
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminCode")))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // GET: /Admin/ManageBooks
        public async Task<IActionResult> ManageBooks()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminCode")))
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

        // GET: /Admin/AddBook
        public IActionResult AddBook()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminCode")))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // POST: /Admin/AddBook
        [HttpPost]
        public async Task<IActionResult> AddBook(string title, string author, string bookCode)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminCode")))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var (success, message) = await _bookService.AddBook(title, author, bookCode);
                if (success)
                {
                    return RedirectToAction("ManageBooks");
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

        // POST: /Admin/RemoveBook
        [HttpPost]
        public async Task<IActionResult> RemoveBook(string bookCode)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminCode")))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var (success, message) = await _bookService.RemoveBook(bookCode);
                TempData["Message"] = message;
                return RedirectToAction("ManageBooks");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error removing book: {ex.Message}";
                return RedirectToAction("ManageBooks");
            }
        }

        // GET: /Admin/ManageUsers
        public async Task<IActionResult> ManageUsers()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminCode")))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var users = await _userService.GetAllUsers();
                return View(users);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading users: {ex.Message}";
                return View(new List<User>());
            }
        }

        // GET: /Admin/AddUser
        public IActionResult AddUser()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminCode")))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // POST: /Admin/AddUser
        [HttpPost]
        public async Task<IActionResult> AddUser(string name, string libraryCode)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminCode")))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var (success, message) = await _userService.AddUser(name, libraryCode);
                if (success)
                {
                    return RedirectToAction("ManageUsers");
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

        // POST: /Admin/RemoveUser
        [HttpPost]
        public async Task<IActionResult> RemoveUser(string libraryCode)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminCode")))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var (success, message) = await _userService.RemoveUser(libraryCode);
                TempData["Message"] = message;
                return RedirectToAction("ManageUsers");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error removing user: {ex.Message}";
                return RedirectToAction("ManageUsers");
            }
        }

        // POST: /Admin/RenewUserSubscription
        [HttpPost]
        public async Task<IActionResult> RenewUserSubscription(string libraryCode)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminCode")))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var (success, message) = await _adminService.RenewUserSubscription(libraryCode);
                TempData["Message"] = message;
                return RedirectToAction("ManageUsers");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error renewing subscription: {ex.Message}";
                return RedirectToAction("ManageUsers");
            }
        }

        // GET: /Admin/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
