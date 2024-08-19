using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketPlace.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet]
        public ViewResult Profile()
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            return View();
        }

        [HttpGet]
        public ViewResult JoinAs()
        {
            return View();
        }

        [HttpGet]
        public ViewResult SignUp()
        {
            return View();
        }

        [HttpGet]
        public ViewResult SignIn()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Logout()
        {
            HttpContext.Response.Cookies.Delete("AuthToken");
            HttpContext.Response.Cookies.Delete("Role");
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(string email, string password)
        {
            var user = new Users { UserEmail = email, UserPassword = password };
            var result = _userRepository.Login(user);

            if (result.isAuthenticated)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                };
                HttpContext.Response.Cookies.Append("AuthToken", email, cookieOptions);
                HttpContext.Response.Cookies.Append("Role", result.role, cookieOptions);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Invalid username or password.";
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }
        }

        [HttpPost]
        public IActionResult SignUp(Users user, [FromQuery] string role)
        {
            try
            {
                if (string.IsNullOrEmpty(role))
                {
                    ModelState.AddModelError("", "Role is required.");
                    return View(user);
                }

                if (user != null)
                {                    
                    Users NewUser = new Users {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserEmail = user.UserEmail,
                        UserPassword = user.UserPassword,
                    };
                    bool result = _userRepository.SignUp(NewUser, role);
                    Console.WriteLine($"Role: {role}, Result: {result}");

                    if (result)
                    {
                        return RedirectToAction("SignIn");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error during registration. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }

            return View(user);
        }
    }
}
