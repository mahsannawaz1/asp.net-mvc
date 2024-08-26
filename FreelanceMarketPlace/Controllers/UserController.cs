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
        public IActionResult Profile()
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            string email = HttpContext.Request.Cookies["AuthToken"];
            if(email == null)
            {
                return RedirectToAction("Index", "Home");
            }
           
            Users user = _userRepository.GetUserProfile(email);
            ViewData["user"] = user;
            
            if (role == "freelancer")
            {
               Freelancer freelancer = _userRepository.GetFreelancerProfile(user.UserId);
               ViewData["freelancer"] = freelancer;
               
            }
            else if (role == "client")
            {
                Client client = _userRepository.GetClientProfile(user.UserId);
                ViewData["client"] = client;
                
            }
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


        [Route("User/EditFreelancerProfile/{id}")]
        public ViewResult EditFreelancerProfile(int id)
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            string email = HttpContext.Request.Cookies["AuthToken"];
            ViewBag.userId = id;
            return View();
        }

        [HttpPost]
        public IActionResult EditFreelancerProfile(UserFreelancer data)
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            string email = HttpContext.Request.Cookies["AuthToken"];
            Users user = new Users
            {
                UserId = data.UserId,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Availability = (int)data.Availability == 1 ? true : false,
                CNIC = data.CNIC,
                PaypalEmail = data.PaypalEmail,
                Phone = data.Phone
            };

            Freelancer freelancer = new Freelancer {
                Title  = data.Title,
                Intro = data.Intro,
                GithubLink = data.GithubLink,
                LinkedInLink = data.LinkedInLink,
                PerHourRate = data.PerHourRate,
            };
            _userRepository.EditFreelancerProfile(user, freelancer);

            return RedirectToAction("Profile", "User");
        }

        [Route("User/EditClientProfile/{id}")]
        public ViewResult EditClientProfile(int id)
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            string email = HttpContext.Request.Cookies["AuthToken"];
            ViewBag.userId = id;
            return View();
        }

        [HttpPost]
        public IActionResult EditClientProfile(UserClient data)
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            string email = HttpContext.Request.Cookies["AuthToken"];
            Console.WriteLine(data.PaypalEmail);
            Console.WriteLine(data.UserId);
            Console.WriteLine(data.FirstName);
            Console.WriteLine(data.LastName);
            Console.WriteLine(data.Phone);
            Console.WriteLine(data.CNIC);
            Users user = new Users
            {
                UserId = data.UserId,
                FirstName = data.FirstName,
                LastName = data.LastName,       
                Availability = (int)data.Availability == 1 ? true : false,
                CNIC = data.CNIC,
                PaypalEmail = data.PaypalEmail,
                Phone = data.Phone
            };


            _userRepository.EditClientProfile(user);
            return RedirectToAction("Profile", "User");
        }

        [HttpPost]
        public async Task<IActionResult> EditProfilePicture(IFormFile ProfilePicture)
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            string email = HttpContext.Request.Cookies["AuthToken"];
            if (email == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                // Define the directory path to save the file
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

                // Ensure the directory exists
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Get the file name and create the full path
                string fileName = Path.GetFileName(ProfilePicture.FileName);
                string filePath = Path.Combine(path, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(stream); // No need to await, since we're doing it synchronously
                }

                // Save the file path to the database
                string fileUrl = $"/Uploads/{fileName}";

                // Use the method to update the database
                _userRepository.EditProfilePicture(email, fileUrl);

                return Json(new { success = true, message = "Profile picture updated and saved successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "No file was uploaded." });
            }
        }
    }
}
