using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LibraryManagementSystem.Mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        private readonly BookService _bookService;
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public AdminController(AdminService adminService, BookService bookService, UserService userService, JwtService jwtService)
        {
            _adminService = adminService;
            _bookService = bookService;
            _userService = userService;
            _jwtService = jwtService;
        }

        // GET: /Admin/Login
        [AllowAnonymous] 
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Admin/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string adminCode, string password)
        {
            try
            {
                var (success, message) = await _adminService.ValidateAdmin(adminCode, password);
                if (success)
                {
                    var admin = await _adminService.GetAdminByCode(adminCode);
                    var token = _jwtService.GenerateToken(admin);
                    Console.WriteLine($"Generated JWT Token: {token}");
                    HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, 
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.Now.AddMinutes(60)
                    });
                    Console.WriteLine("JWT Cookie set successfully");
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
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Admin/Register
        [HttpPost]
        [AllowAnonymous]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Admin/ManageBooks
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageBooks()
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

        // GET: /Admin/AddBook
        [Authorize(Roles = "Admin")]
        public IActionResult AddBook()
        {
            return View();
        }

        // POST: /Admin/AddBook
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook(string title, string author, string bookCode)
        {
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveBook(string bookCode)
        {
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUsers()
        {
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
        [Authorize(Roles = "Admin")]
        public IActionResult AddUser()
        {
            return View();
        }

        // POST: /Admin/AddUser
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser(string name, string libraryCode)
        {
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUser(string libraryCode)
        {
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RenewUserSubscription(string libraryCode)
        {
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
        [Authorize(Roles = "Admin")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Home");
        }
    }
}