using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using FreelanceMarketPlace.Models.Repositories;
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
            var currentUser = Request.Cookies["CurrentUser"];

            // Pass the user info to the view
            ViewBag.CurrentUser = currentUser;
            return View(currentUser);
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
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(string email, string password)
        {
            var user = new Users { UserEmail = email, UserPassword = password };
            bool isAuthenticated = _userRepository.Login(user);

            if (isAuthenticated)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                };
                HttpContext.Response.Cookies.Append("AuthToken", email, cookieOptions);

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
